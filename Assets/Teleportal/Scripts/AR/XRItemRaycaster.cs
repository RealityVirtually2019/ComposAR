// Teleportal SDK
// Code by C. Dillon Martin Hall and Thomas Suarez
// Copyright 2018 WiTag Inc

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// A raycaster that can selectively target XRItems from the screen center.
/// </summary>
public class XRItemRaycaster : MonoBehaviour
{

	public static XRItemRaycaster Shared;

	/// <summary>
	/// The current targeted/focused XRItem.
	/// </summary>
	public XRItem ItemFocus;

	/// <summary>
	/// The Player Item that is currently being targeted by the reticle.
	/// </summary>
	public string PlayerFocus;

	/// <summary>
	/// Whether this Raycaster should be enabled right now.
	/// Can be changed during runtime.
	/// </summary>
	public bool ShouldRaycast = true;

	/// <summary>
	/// Called before Start()
	/// </summary>
	void Awake() {
		XRItemRaycaster.Shared = this;
	}

	/// <summary>
	/// Standard MonoBehaviour Start function.
	/// </summary>
	void Start() { }

	/// <summary>
	/// Updates on each frame.
	/// </summary>
	void Update() {
		// Attempt Raycast
		if (ShouldRaycast) {
			Raycast();
		}
	}

	/// <summary>
	/// Raycasts from the attached camera center, outward.
	/// </summary>
	private void Raycast() {
		// Raycast from attached camera center outward
		Vector3 position = TeleportalAr.Shared.CurrentCamera.transform.position;
		Vector3 direction = TeleportalAr.Shared.CurrentCamera.transform.forward;
		Ray ray = new Ray(position, direction);
		// CAMERA VERSION - Ray ray = TeleportalAr.Shared.CurrentCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

		RaycastHit hit;
		int layerMask = 1 << LayerMask.NameToLayer("XR");
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
			// Test if collided object is an XRItem (has the XRItem component)
			if (hit.collider.gameObject.GetComponent(typeof(XRItem)) != null) {
				
				// Get the raycasted item
				XRItem item = hit.collider.gameObject.GetComponent<XRItem>();

				// If this XRItem already has focus, skip.
				if (item == ItemFocus) {
					// return;
				}

				// Current focus was changed (if it exists)
				if (ItemFocus != null){
					ItemFocus.LostFocus();
				}

				// Store new item focus
				ItemFocus = item;

				// Tell item it got focus
				ItemFocus.GotFocus();

				// Set reticle to ready
				TeleportalUi.Shared.SetReticle(ReactiveReticle.ReticleType.Ready);

				// If the item is a player...
				if (item.Type == XRItem.XRItemType.Player) {
					// Save their username
					PlayerFocus = item.Title;

					// Set the Reactive Reticle to FireMiss (blue)
					TeleportalUi.Shared.SetReticle(ReactiveReticle.ReticleType.FireMiss);
				}

                // If this item was an inventory item...
                else if (item.Type == XRItem.XRItemType.Inventory) {
                    // Hold item (press E key)
                    if (Input.GetKeyDown(KeyCode.E)) {
                        TeleportalAr.Shared.HoldItem(item);
                    }

                    // Release item & reposition it (press E key again)
                    else if (Input.GetKeyUp(KeyCode.E)) {
                        TeleportalAr.Shared.ReleaseItem(item);
                    }
                }

				return;
			}

			// Stop here
			return;
		}

		// Current focus was lost (if it exists)
		if (ItemFocus != null) {
			ItemFocus.LostFocus();
		}

		// Clear out stored focuses
		ItemFocus = null;
		PlayerFocus = null;

		// Set reticle to not ready
		TeleportalUi.Shared.SetReticle(ReactiveReticle.ReticleType.NotReady);
	}

	/// <summary>
	/// Uses the currently-selected Inventory Item.
	/// </summary>
	public void Tap() {
		/*
		// If there is a menu in view...
		if (MenuFocus != null) {
			MenuFocus.gameObject.GetComponent<Button>().onClick.Invoke();
		}*/

		// If there is a prompt currently on the screen...
		if (TeleportalUi.Shared.CurrentPrompt != null) {
			TeleportalUi.Shared.CurrentPrompt();
			TeleportalUi.Shared.CurrentPrompt = null;
		}

		// Otherwise, use the currently-selected Inventory Item.
		else {
			TeleportalInventory.Shared.UseCurrent();
		}
	}

	/// <summary>
	/// Alternate tap of the hovered AR menu (usually right click).
	/// </summary>
	public void TapAlt() {
		// If there is an item in focus...
		if (ItemFocus != null) {
			// Delete it from the world (server-side)
			TeleportalAr.Shared.DeleteItem(ItemFocus.Id);
		}
	}

}
