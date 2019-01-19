// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;

/// <summary>
/// A 3D object that is shown and controlled in the XR world.
/// </summary>
public class XRItem: MonoBehaviour {

  /* Enumerated types */

  /// <summary>
  /// Each possible type of XR Item.
  /// </summary>
  public enum XRItemType {
    Inventory, World, Player
  };

  /* Properties */

  /// <summary>
  /// The current type of XR Item.
  /// </summary>
  public XRItemType Type;

  /// <summary>
  /// This XR Item's server-assigned unique ID.
  /// </summary>
  public string Id;

  [HideInInspector]
  /// <summary>
  /// This XR Item's title.
  /// </summary>
  public string Title;

  [HideInInspector]
  /// <summary>
  /// The current latitude of this XR Item.
  /// </summary>
  public double Latitude = 0.0;
  
  [HideInInspector]
  /// <summary>
  /// The current Y position of this XR Item.
  /// </summary>
  public double PosY = 0.0;

  [HideInInspector]
  /// <summary>
  /// The current longitude of this XR Item.
  /// </summary>
  public double Longitude = 0.0;

  [HideInInspector]
  /// <summary>
  /// The current compass heading of this XR Item.
  /// </summary>
  public double Heading = 0.0;

  [HideInInspector]
  /// <summary>
  /// The current pitch rotation (euler) of this XR Item.
  /// </summary>
  public double Pitch = 0.0;

  [HideInInspector]
  /// <summary>
  /// The current distance from the current player to this XR Item.
  /// </summary>
  public double Distance = 0.0;

  [HideInInspector]
  /// <summary>
  /// The most recent reported property of the networked XR Item state.
  /// </summary>
  public String LastStateProperty;

  [HideInInspector]
  /// <summary>
  /// The most recent value of the networked XR Item state's last property.
  /// </summary>
  public String LastStateValue;

  /// <summary>
  /// The rigidbody physics component that is attached to this XR Item GameObject.
  /// </summary>
  private Rigidbody rb;

  /// <summary>
  /// Add a callback function to this Action to be notified
  /// when this XR Item has been repositioned. Example:
  /// `item.OnRepositioned += YourFunction;`
  /// </summary>
  public UnityAction OnRepositioned;

  /// <summary>
  /// Add a callback function to this Action to be notified
  /// when this XR Item has a state change waiting to be applied.
  /// Example: `item.OnStateChange += YourFunction;`
  /// You can then get the property/value combination from the following
  /// two instance variables: `item.LastStateProperty` and `item.LastStateValue`.
  /// </summary>
  public UnityAction OnStateChange;

  /// <summary>
  /// Standard MonoBehaviour Start() function.
  /// </summary>
  virtual public void Start() {
    // Parent this item gameobject to the Modules container object
    transform.SetParent(GameObject.Find("Floor").transform);
    rb = GetComponent<Rigidbody>();

    // Move the transform out of sight for now
    transform.position = new Vector3(0, -50, 0);

    CustomItemTypeLogic();
  }

  public virtual void CustomItemTypeLogic() {
      // unused in main class
  }

  /// <summary>
  /// Called by Teleportal.
  /// Sets the ID on this Item.
  /// </summary>
  /// <param name="newId">The new ID for this Item.</param>
  public void SetId(string newId) {
    // Set object variable to new value
    this.Id = newId;
  }

  /// <summary>
  /// Called by Teleportal.
  /// Sets the local rotation variables of this Item, locally.
  /// Only modifies for this client's viewport.
  /// To rotate this Item in the world, see RepositionItem().
  /// </summary>
  /// <param name="heading"></param>
  /// <param name="pitch"></param>  
  public void SetRotation(double heading, double pitch) {
    // Modify object variables
    Heading = heading;
    Pitch = pitch;
  }

  /// <summary>
  /// Called by Teleportal.
  /// Sets the absolute location of this Item, locally.
  /// Only modifies for this client's viewport.
  /// To change this Item's position in the world, see RepositionItem().
  /// </summary>
  /// <param name="newLat">New absolute latitude for this Item.</param>
  /// <param name="newLon">New absolute longitude for this Item.</param>
  public void SetLocation(double newLat, double newY, double newLon) {
    // Modify object variables
    Latitude = newLat;
    PosY = newY;
    Longitude = newLon;
  }

  public void SetState(String property, String value) {
    // Send state change to Teleportal
    TeleportalNet.Shared.Send(TeleportalCmd.ITEM_STATE, this.Id, property, value);

    // Loopback (only this client)
    SetStateCallback(property, value);
  }

  /// <summary>
  /// Called by Teleportal.
  /// This is the server callback for XRItem.SetState().
  /// It is typically received by all other devices in the local area/scene.
  /// </summary>
  /// <param name="property">The developer-defined state property of the Item.</param>
  /// <param name="value">The value for the Item state property.</param>
  public void SetStateCallback(String property, String value) {
    // TODO add comment
    this.LastStateProperty = property;
    this.LastStateValue = value;

    // Notify listeners (if there are any)
    if (OnStateChange != null) {
      OnStateChange();
    }
  }

  /// <summary>
  /// Asks Teleportal to reposition the Item.
  /// </summary>
  public void Reposition() {
    // Ask the server to reposition this Item
    TeleportalNet.Shared.Send(TeleportalCmd.ITEM_REPOSITION, this.Id, this.Latitude.ToString(), this.PosY.ToString(), this.Longitude.ToString(), this.Heading.ToString(), this.Pitch.ToString());
  }
  
  /// <summary>
  /// Handles the callback from Teleportal for repositioning this Item.
  /// </summary>
  /// <param name="position">The new position for this Item.</param>
  /// <param name="euler">The new rotation for this Item.</param>
  public void RepositionCallback(Vector3 position, Vector3 euler) {
    if (Type == XRItemType.Player) {
      // Animate to new position & rotation
      StartCoroutine(MovementHelper.AnimateMoveRotateTo(transform, position, euler, 0.5f));
    }
    else {
      // Set new position & rotation
      transform.position = position;
      transform.eulerAngles = euler;
    }

    // Notify listeners (if there are any)
    if (OnRepositioned != null) {
      OnRepositioned();
    }
  }

  /// <summary>
  /// Removes this Item and its GameObject from the world.
  /// </summary>
  public void RemoveFromGame() {
    // If this GameObject still exists...
    if (gameObject != null) {
      // TODO: animate fade out

      // Destroy this gameobject
      Destroy(gameObject);
    }
  }

  /// <summary>
  /// Called by XRItemRaycaster when the current Player starts gazing at this XRItem.
  /// </summary>
  public void GotFocus() {
    // nothing yet
	}

	/// <summary>
	///  Called by XRItemRaycaster when the current Player stops gazing at this XRItem.
	/// </summary>
	public void LostFocus() {
    // nothing yet
	}

}
