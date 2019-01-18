// WiTag Unity Client
// Code by C. Dillon Martin Hall
// Copyright 2018 WiTag Inc

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Deprecated.
/// A 3D user-guided menu that is shown and controlled in the AR world.
/// </summary>
public class ARMenu : MonoBehaviour {

	/* Properties */

  /// <summary>
  /// Whether this AR menu is currently active and being shown.
  /// </summary>
	public bool IsActive = true;

  /// <summary>
  /// Whether this AR menu is fully implemented and ready to be used.
  /// </summary>
  public bool IsImplemented = true;

  /// <summary>
  /// The target object for executing the specified script method.
  /// </summary>
  public GameObject TargetObject;

  /// <summary>
  /// The target script for executing the specified method.
  /// </summary>
  public String TargetScript;

  /// <summary>
  /// The target method to execute.
  /// </summary>
  public string TargetMethod;

  /// <summary>
  /// Whether this menu is simply a menu choice (false) or runs some ancillary function (true, default)
  /// </summary>
  public bool Executable = true;

  /// <summary>
  /// Whether this menu has children (submenus).
  /// </summary>
  private bool HasChildren = true;

  /// <summary>
  /// Whether this menu has a title.
  /// </summary>
  public bool HasTitle = false;

  /// <summary>
  /// Constant active opacity for an AR menu.
  /// </summary>
	public float ActiveOpacity = 1.0f;

  /// <summary>
  /// Constant inactive opacity for an AR menu.
  /// </summary>
	public float InactiveOpacity = 0.4f;

  /// <summary>
  /// GUI menu label gameobject.
  /// </summary>
  private GameObject MenuLabel;

  /// <summary>
  /// GUI menu title gameobject.
  /// </summary>
  private GameObject MenuTitle;

  /// <summary>
  /// For storing the button component attached to this game object.
  /// </summary>
  Selectable buttonComponent;

  /// <summary>
  /// For setting opacities.
  /// </summary>
  RawImage rawImageComponent;

  /* Menu Item Colors */

  /// <summary>
  /// Constant active color for menu items.
  /// </summary>
  /// <returns>The color value.</returns>
  private Color activeColor = new Color(1f, 1f, 1f, 1.0f);

  /// <summary>
  /// Constant inactive color for menu items.
  /// </summary>
  /// <returns>The color value.</returns>
  private Color inactiveColor = new Color(1f, 1f, 1f, 0.4f);

	/* Menu Tree Properties */

  /// <summary>
  /// The parents of this AR menu.
  /// </summary>
	public List<ARMenu> Parents;

  /// <summary>
  /// The siblings (with shared parents) of this AR menu.
  /// </summary>
	public List<ARMenu> Siblings;

  /// <summary>
  /// The children (submenus) of this AR menu.
  /// </summary>
	public List<ARMenu> Children;

  /// <summary>
  /// The layer of this menu in the menu hierarchy, starting at 0 for the root menu.
  /// </summary>
	public float MenuDepth = 0f;

  /// <summary>
  /// This menu button's position among its siblings, starting at 0 for the first among siblings.
  /// </summary>
	public float RelativePosition = 0f;

  /// <summary>
  /// The direction this menu extends in x, defaults to +1 (to the right).
  /// </summary>
	public float MenuDirectionX = 1f;

  /// <summary>
  /// The direction this menu extends in y, defaults to 0 (no vertical change).
  /// </summary>
	public float MenuDirectionY = 0f;

  /// <summary>
  /// The height of this menu, defaults to 1
  /// </summary>
  public float MenuYPosition = 0f;

  /// <summary>
  /// The Z position of buttons, defaults to 2
  /// </summary>
	public float MenuZPosition = 2f;

  /// <summary>
  /// The size of buttons in this menu
  /// </summary>
	public float ButtonSize = 1f;

  /// <summary>
  /// The spacing between buttons in menus
  /// </summary>
	public float ButtonPadding = 0.05f;

	/* IAP Properties */

  /// <summary>
  /// IAP property: whether the option given in this AR menu is purchable.
  /// </summary>
	public bool IsPurchasable = false;

  /// <summary>
  /// IAP property: whether the option given in this AR menu was purchased.
  /// </summary>
	public bool WasPurchased = false;

  /// <summary>
  /// MonoBehaviour awake.
  /// </summary>
  private void Awake()
  {
    // Assign raw image component for alpha UI indications
    rawImageComponent = gameObject.GetComponent<RawImage>();

    // Set proper title & label positions to start
		RepositionLabel();
		RepositionTitle();

    buttonComponent = gameObject.GetComponent<Selectable>(); // Grab button component for future use
    if (!IsImplemented)
    {
        buttonComponent.interactable = false; // Disable this button if marked as not implemented
        rawImageComponent.color = inactiveColor;
    }

    // Check for children for proper menu tree handling
    if (Children.Count > 0)
    {
        HasChildren = true;
    }
    else
    {
        HasChildren = false;
    }
  }

  /// <summary>
  /// MonoBehaviour load.
  /// </summary>
  private void Start()
  {

  }

  /// <summary>
  /// Repositions the AR menu's title UI (if present).
  /// </summary>
	private void RepositionTitle() {
		if (HasTitle)
		{
	    // Reposition Menu title
	    float TitleXPos = (((this.Siblings.Count + 1) * ButtonSize) / 2) - (ButtonSize / 2) + (this.Siblings.Count * ButtonPadding);
			print("Setting Button " + transform.name + " title x pos = " + TitleXPos + ".");
      MenuTitle.transform.localPosition = new Vector3(TitleXPos, 0.5f, 0);
		}
  }

  /// <summary>
  /// Places menu button's title correctly relative to button
  /// </summary>
  private void RepositionLabel() {
    MenuLabel = gameObject.transform.Find("MenuLabel").gameObject;
    // MenuLabel.transform.SetPositionAndRotation(gameObject.transform.position + new Vector3(0, -0.6f, 0), new Quaternion(0, 0, 0, 0));
  }

  /// <summary>
  /// Run when user is looking at this button
  /// </summary>
  public void GotFocus() {
    //print(gameObject.transform.name + " got focus!");
    if (IsActive) { //Only show menu hint if item is the active menu layer
      MenuLabel.SetActive(true);
    }
	}

	/// <summary>
	/// Run when user stops looking at this button
	/// </summary>
	public void LostFocus(){
		//print(gameObject.transform.name + " lost focus!");
    MenuLabel.gameObject.SetActive(false);
	}

	/// <summary>
	/// Activate (and unhide) this AR Menu button
	/// </summary>
	public void Activate(){
		// Reactivate self
    gameObject.SetActive (true);
		IsActive = true;

    // Reposition self
    ToInitialPosition();

    // Set to active view
    rawImageComponent.color = activeColor;

    // Activate Menu Title (if present)
    if (MenuTitle != null) {
        MenuTitle.SetActive(true);
    }
	}

	/// <summary>
	/// Deactivate (but don't hide) this AR Menu button
	/// </summary>
	public void Deactivate (){
		IsActive = false;
    // Move to root position for this menu level
		ToInactivePosition();
    rawImageComponent.color = inactiveColor;

		// Deactivate Menu Title (if present)
		if (MenuTitle != null) {
			MenuTitle.SetActive(false);
		}
	}

	/// <summary>
	/// Hide this menu (and any Children)
	/// </summary>
	public void Hide(){
		// Reference ARMenu component of this object
		ARMenu ThisMenu = gameObject.GetComponent (typeof(ARMenu)) as ARMenu;

    // Hide this menu button and tell children to do the same
		gameObject.SetActive (false);
    if (ThisMenu.Children.Count != 0)
    {
        foreach (ARMenu child in ThisMenu.Children)
        {
            child.Hide();
        }
    }
	}

	/// <summary>
	/// Hide when other option is chosen, but don't hide children
	/// </summary>
	public void NotChosen(){
		gameObject.SetActive (false);
	}

	/// <summary>
	/// Send this menu button to its initial position at the proper depth and among its siblings
	/// </summary>
	public void ToInitialPosition () {
		// TODO fix ar menu positioning
		return;
		
		float TotalButtonWidth = ButtonSize + ButtonPadding;
		Vector3 InitialPos = new Vector3 ( (MenuDirectionX * ((MenuDepth * TotalButtonWidth) + (RelativePosition * TotalButtonWidth))) , (MenuDirectionY * ((MenuDepth * TotalButtonWidth) + (RelativePosition * TotalButtonWidth))) + MenuYPosition - TotalButtonWidth, MenuZPosition);
		Quaternion InitialRot = new Quaternion(0,0,0,0);

		// Set position and rotation
		gameObject.GetComponent<RectTransform>().localPosition = InitialPos;
		gameObject.GetComponent<RectTransform>().rotation = InitialRot;

		// Reposition title
  	RepositionTitle();
	}

	/// <summary>
	/// Send this menu button to the top of its depth once selected
	/// </summary>
	public void ToInactivePosition () {
		// TODO fix ar menu positioning
		return;

		float TotalButtonWidth = ButtonSize + ButtonPadding;
    Vector3 InactivePos = new Vector3 ( (MenuDirectionX * (MenuDepth * TotalButtonWidth)) , (MenuDirectionY * (MenuDepth * TotalButtonWidth)) + MenuYPosition - TotalButtonWidth, MenuZPosition);
		Quaternion InactiveRot = new Quaternion(0,0,0,0);

		// Set position and rotation
		gameObject.GetComponent<RectTransform>().localPosition = InactivePos;
		gameObject.GetComponent<RectTransform>().rotation = InactiveRot;

		// Reposition title
  	RepositionTitle();
	}

	/// <summary>
	/// When triggered by clicking on a menu item
	/// </summary>
	public void Interact () {

		// Reference ARMenu component of this object
		ARMenu ThisMenu = gameObject.GetComponent (typeof(ARMenu)) as ARMenu;

		// If this button is in the Active state
    if (IsActive && IsImplemented) {

				// Play select/tap sound
				TeleportalUi.Shared.PlaySound(TeleportalUi.SoundType.Select);

        // Make sure button target is set
        if (Executable && TargetObject != null && TargetMethod != null && TargetScript != null)
        {
            // Run the targeted method on target script
            (TargetObject.GetComponent(TargetScript) as MonoBehaviour).Invoke(TargetMethod, 0f);
        }
        // Otherwise, throw an error
        else if (Executable)
        {
            print("Button " + transform.name + " is missing target associations and cannot execute.");
        }

        if (HasChildren)
        {
            // Activate this menu's children
            foreach (ARMenu child in ThisMenu.Children)
            {
                child.Activate();
            }

            // Hide this menu's siblings (but not children)
            foreach (ARMenu sibling in ThisMenu.Siblings)
            {
                sibling.NotChosen();
            }

            // Set this button to inactive
            ThisMenu.Deactivate();

            // Hide menu text now that this has become inactive
            LostFocus();
        }
		}

    // If this button is in the Inactive state
    else if (!IsActive && IsImplemented){
			// Hide this menu's children
			foreach (ARMenu child in ThisMenu.Children) {
				child.Hide ();
			}

			// Activate this menu's siblings
			foreach (ARMenu sibling in ThisMenu.Siblings) {
				sibling.Activate();
			}

			// Set this button to active
			ThisMenu.Activate();

            // Display menu button title text
            GotFocus();
		}
	}
}
