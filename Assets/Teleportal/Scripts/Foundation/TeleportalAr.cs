// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using System;
using UnityEngine;
using UnityEngine.XR.iOS;
using GoogleARCore;
using GoogleARCoreInternal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Shared module for controlling the Augmented Reality (AR) world.
/// </summary>
public class TeleportalAr : MonoBehaviour {

  /* Shared Object References */

  public static TeleportalAr Shared;

  [HideInInspector]
  /// <summary>
  /// The camera that is currently showing to the user.
  /// </summary>
  public Camera CurrentCamera;

  /* GameObject references */

  [HideInInspector]
  /// <summary>
  /// The 3D virtual landscape, consisting of terrain, water, etc.
  /// Replaces the background of the camera input (if applicable on platform).
  /// Does not include XR Items.
  /// </summary>
  public GameObject VirtualWorld;

  /// <summary>
  /// The ARCore camera gameobject.
  /// </summary>
  public Camera GoogleArcoreCamera;

  /// <summary>
  /// The SteamVR camera gameobject.
  /// </summary>
  public Camera SteamVrCamera;

  /* Prefab references */

  /// <summary>
  /// The Player Item prefab.
  /// </summary>
  public GameObject _PlayerItem;

  /* XR data */

  /// <summary>
  /// Dictionary (itemID, XR World Item) of all world items and their rendered XR objects.
  /// </summary>
  public Dictionary<String, XRWorldItem> WorldItems = new Dictionary<string, XRWorldItem>();

  /// <summary>
  /// Array of all rendered XR Items currently in the game.
  /// </summary>
  public Dictionary<String, XRItem> Items = new Dictionary<string, XRItem>();

  /// <summary>
  /// Dictionary (username, XR Player Item) of all players and their rendered XR selves.
  /// </summary>
  /// <returns></returns>
  Dictionary<String, XRPlayerItem> PlayerItems = new Dictionary<string, XRPlayerItem>();

  ///// LIFECYCLE /////

  void Awake() {
    // Set static self reference
    TeleportalAr.Shared = this;

    // Find the necessary scene objects to control
    FindObjects();
  }

  void Start() {
    // Connect Teleportal callbacks
    TeleportalActions.Shared.OnLocationUpdate += OnLocationUpdate;

    // Global signal
    if (TeleportalActions.Shared.SignalFindObjects != null) {
      TeleportalActions.Shared.SignalFindObjects();
    }
  }

  /// <summary>
  /// Finds and links the immersive camera objects from Teleportal.
  /// </summary>
  private void FindObjects() {
    // Set the immersive camera, based on current platform
    // ARCore, SteamVR, or other (such as Desktop, iOS)
    switch (TeleportalPlatformInfo.CurrentXRDevice) {
      case TeleportalPlatformInfo.XRDeviceModel.GoogleArCore:
        CurrentCamera = GoogleArcoreCamera;
        break;
      case TeleportalPlatformInfo.XRDeviceModel.OculusRift:
        CurrentCamera = SteamVrCamera;
        break;
      default:
        CurrentCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        break;
    }
  }

  /// <summary>
  /// Calculates the newest distances to XR Items.
  /// </summary>
  private void OnLocationUpdate() {
    // Calculate newest approximate distances to XR Items
    CalculateItemDistances(TeleportalGps.Shared.Latitude, TeleportalGps.Shared.Longitude);
  }

  /// <summary>
  /// Updates on every rendered frame.
  /// </summary>
  void Update() {
    TeleportalUi.Shared.DetermineTouches();
  }

  /// <summary>
  /// Resets ARKit or ARCore tracking to position (0,0,0) and rotation (0,0,0).
  /// </summary>
  public void ResetTracking() {
    // Apple iOS (ARKit)
    if (TeleportalPlatformInfo.IsMobileApple) {
      ARKitWorldTrackingSessionConfiguration sessionConfig = new ARKitWorldTrackingSessionConfiguration ( UnityARAlignment.UnityARAlignmentGravity, UnityARPlaneDetection.Horizontal);
      UnityARSessionNativeInterface.GetARSessionNativeInterface().RunWithConfigAndOptions(sessionConfig, UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors | UnityARSessionRunOption.ARSessionRunOptionResetTracking);
    }

    // Google Android (ARCore)
    else if (TeleportalPlatformInfo.IsMobileGoogle) {
      LifecycleManager.Instance.DestroySession();
      LifecycleManager.Instance.EnableSession();
    }
  }

  ///// XR ITEMS /////

  /// <summary>
  /// Toggles the virtual CG rendered world on and off.
  /// When on, the background is replaced by the virtual world.
  /// When off, the background is replaced by an AR camera passthrough.
  /// </summary>
  public void ToggleVirtualWorld() {
    // Toggle the master gameobject on/off
    VirtualWorld.active = !VirtualWorld.active;

    // Apple ARKit (iOS)
    if (TeleportalPlatformInfo.IsMobileApple) {
      // Only render the camera background when the virtual world is NOT showing
      CurrentCamera.GetComponent<UnityARVideo>().enabled = !VirtualWorld.active;

      // Only render the skybox when the virtual world IS showing
      CurrentCamera.GetComponent<Camera>().clearFlags = VirtualWorld.active ? CameraClearFlags.Skybox : CameraClearFlags.Depth;
    }
    // Google ARCore (Android)
    else if (TeleportalPlatformInfo.IsMobileGoogle) {
      // Only render the camera background when the virtual world is NOT showing
      CurrentCamera.GetComponent<ARCoreBackgroundRenderer>().enabled = !VirtualWorld.active;
    }
  }

  public void CreateRealmItem(string realmId, XRWorldItem item) {
    // Get item transform
    Transform t = item.gameObject.transform;
    float posX = t.position.x;
    float posY = t.position.y;
    float posZ = t.position.z;
    float heading = t.eulerAngles.y;
    float pitch = t.eulerAngles.x;

    // Send to Teleportal
    TeleportalNet.Shared.Send(TeleportalCmd.REALM_ITEM_ADD, realmId, item.Id, posX.ToString(), posY.ToString(), posZ.ToString(), heading.ToString(), pitch.ToString());
  }

  /// <summary>
  /// Adds an XR Item to the world scene.
  /// </summary>
  /// <param name="id">The unique id of the XR Item.</param>
  /// <param name="lat">The latitude location of the XR Item.</param>
  /// <param name="lon">The longitude location of the XR Item.</param>
  /// <param name="heading">The euler heading (in degrees) of the XR Item, relative to the origin normal.</param>
  /// <param name="pitch">The euler pitch rotation (in degrees) of the XR Item, relative to the origin normal.</param>
  public void AddItem(string type, string id, double lat, double y, double lon, double heading, double pitch) {
    // Get prefab to use
    string path = "P_" + type; // "P" means "prefab" here
    GameObject prefab = Resources.Load(path) as GameObject;

    // Create new XR Item & set its location + rotation
    GameObject itemGO = Instantiate(prefab);
    XRItem item = itemGO.GetComponent<XRItem>();
    item.SetId(id);
    item.SetLocation(lat, y, lon);
    item.SetRotation(heading, pitch);

    // Reposition
    item.Reposition();

    // Add to dictionaries
    Items[id] = item;

    // TP //
    if (EditorToggleButton.Shared.waitingForDuplication) {
      EditorToggleButton.Shared.OnDuplication(id, item);
      EditorToggleButton.Shared.waitingForDuplication = false; // reset
    }
  }

  /// <summary>
  /// Asks Teleportal to move an XR Item.
  /// </summary>
  /// <param name="id">The ID of the Item to move.</param>
  /// <param name="posX">The new X coordinate (local) for the Item.</param>
  /// <param name="posY">The new Y coordinate (local) for the Item.</param>
  /// <param name="posZ">The new Z coordinate (local) for the Item.</param>
  /// <param name="newHeading">The new heading for the Item.</param>
  /// <param name="newPitch">The new pitch for the Item.</param>
  public void MoveItem(string id, double posX, double posY, double posZ, double newHeading, double newPitch) {
    // Send new position/rotation to server
    TeleportalNet.Shared.Send(TeleportalCmd.ITEM_MOVE, id, posX.ToString(), posY.ToString(), posZ.ToString(), newHeading.ToString(), newPitch.ToString());
  }

  /// <summary>
  /// Asks Teleportal to delete an XR Item.
  /// </summary>
  /// <param name="id">The ID of the Item to delete.</param>
  public void DeleteItem(string id) {
    // Send deletion request to server
    TeleportalNet.Shared.Send(TeleportalCmd.ITEM_DELETE, id);
  }

  /// <summary>
  /// Parents the given XR Item's transform to the current User,
  /// allowing the User to "carry" the XR Item somewhere.
  /// </summary>
  /// <param name="item">The XR Item to hold.</param>
  public void HoldItem(XRItem item) {
    // Parent the XR Item to the current player
    Transform t = item.gameObject.transform;
    t.parent = TeleportalPlayer.Current.transform;
  }

  /// <summary>
  /// Releases the given XR Item from the "grip" of the current User,
  /// and sends the new position to Teleportal.
  /// </summary>
  /// <param name="item">The XR Item to release.</param>
  public void ReleaseItem(XRItem item) {
    // Clear the parent transform
    Transform t = item.gameObject.transform;
    t.parent = null;

    // Ask Teleportal to move the item
    MoveItem(item.Id, t.position.x, t.position.y, t.position.z, t.eulerAngles.y, t.eulerAngles.x);
  }

  /// <summary>
  /// Deletes the specified XR Item from the dictionary,
  /// and from the local world scene. (client only)
  /// For impacting the world on the server (and other clients),
  /// see `TeleportalAr.DeleteItem(string id)` above.
  /// </summary>
  /// <param name="item">The ID of the XR Item to remove.</param>
  public void DeleteItemVisual(string id){
    // Remove the item from the Items list
    if (Items.Keys.Contains(id)) {
      Items[id].RemoveFromGame();
      Items.Remove(id);
    }
    else {
      print("Attempted to remove XR Item at ID '" + id + "' but it does not exist.");
    }
  }

  /// <summary>
  /// Repositions the item, relative to the synchronized coordinate system.
  /// </summary>
  /// <param name="id">The ID of the item to reposition.</param>
  /// <param name="position">The new local position of the Item's transform.</param>
  /// <param name="euler">The new local rotation (euler angles) of the item's Transform.</param>
  public void RepositionItem(string id, Vector3 position, Vector3 euler) {
    // Find the item, if it exists
    if (Items.Keys.Contains(id)) {
      // Tell the XR Item to update its position and rotation
      Items[id].RepositionCallback(position, euler);
    }
  }

  public void SetItemState(string id, string property, string value) {
    // Find the item, if it exists
    if (Items.Keys.Contains(id)) {
      // Tell the XR Item to update its state
      Items[id].SetStateCallback(property, value);
    }
  }

  /// <summary>
  /// Called by Teleportal.
  /// Sets the avatar for another Player in the viewport (client-only).
  /// </summary>
  /// <param name="username">The username of the Player to change avatar.</param>
  /// <param name="newType">The AvatarType for changing the Player.</param>
  public void SetPlayerAvatar(string username, AvatarType newType) {
    if (PlayerItems.ContainsKey(username)) {
      PlayerItems[username].SetAvatar(newType);
    }
  }

  ///// USER LOCATION HANDLERS /////

  /// <summary>
  /// Called when a user is located to another position in the real/game world.
  /// </summary>
  /// <param name="username">The username of the player that relocated.</param>
  /// <param name="latitude">The latitude of the player's new position.</param>
  /// <param name="posY">The Y coordinate of the player's new position.</param>
  /// <param name="longitude">The longitude of the player's new position.</param>
  public void UserLocated(string username, string latitude, string posY, string longitude, string heading) {
      // Check to make sure located player is not self
      if (username != TeleportalAuth.Shared.Username)
      {
          // Discretely inform player of other player location
          TeleportalUi.Shared.Toast(username + " located at: (" + latitude + ", " + posY + ", " + longitude);

          // Update player location in TeleportalAr
          TeleportalAr.Shared.UpdatePlayerItem(username, double.Parse(latitude), double.Parse(posY), double.Parse(longitude), float.Parse(heading));
      }
  }

  /// <summary>
  /// Updates another player's location in the AR world.
  /// </summary>
  /// <param name="username">The username of the player to update.</param>
  /// <param name="latitude">The new latitude location of the player.</param>
  /// <param name="posY">The Y coordinate of the player's new position.</param>
  /// <param name="longitude">The new longitude location of the player.</param>
  public void UpdatePlayerItem(string username, double latitude, double posY, double longitude, float heading) {
    // Check if username already exists; create/add it if new
    if (!PlayerItems.ContainsKey(username)){
      GameObject playerGO = Instantiate(TeleportalAr.Shared._PlayerItem);
      XRPlayerItem playerXRItemComponent = playerGO.GetComponent<XRPlayerItem>();
      playerXRItemComponent.ChangeTitle(username); // set floating title text
      Items.Add(username, playerXRItemComponent);
      PlayerItems.Add(username, playerXRItemComponent);
    }

    // Temporary variable for XRPlayerItem reference
    XRPlayerItem item = new XRPlayerItem();

    // If this XRItem still exists...
    if (PlayerItems.TryGetValue(username, out item)){
      // Update username / item ID
      item.SetId(username);

      // Update position
      item.SetLocation(latitude, posY, longitude);

      // Update rotation
      item.SetRotation(heading, 0);

      // Reposition player item to relative
      item.Reposition();
    }
    else {
      print("Cannot update player: " + username + " does not exist");
    }
  }

  /// <summary>
  /// Removes the specified player from the dictionary, and from the AR world.
  /// </summary>
  /// <param name="username">The username of the player to remove.</param>
  public void RemovePlayerItem(string username){
    if (PlayerItems.ContainsKey(username)){
      PlayerItems[username].RemoveFromGame();
      Items.Remove(username);
      PlayerItems.Remove(username);
    }
    else {
      print("Attempted to remove player " + username + " but it does not exist.");
    }
  }

  /// <summary>
  /// Removes all timed game-related items from the dictionary, and from the AR world.
  /// </summary>
  public void RemoveGameItems() {
    foreach (string key in Items.Keys.ToList()) {
      DeleteItemVisual(key);
    }
    foreach (string key in PlayerItems.Keys.ToList()) {
      RemovePlayerItem(key);
    }
  }

  /// <summary>
  /// Repositions all items in the world, relative to the GPS location.
  /// </summary>
  public void RepositionItems() {
    foreach (XRItem item in Items.Values) {
      item.Reposition();
    }
  }

  ///// CALCULATIONS /////

  /// <summary>
  /// Calculates distances from the specified location to every item in the game,
  /// saving the distances in each XR Item object's "distance" property.
  /// </summary>
  /// <param name="lat">Latitude from which to measure.</param>
  /// <param name="lon">Longitude from which to measure.</param>
  public void CalculateItemDistances(double lat, double lon) {
    // Iterate through all XR Items in the game
    Items.Values.ToList().ForEach(delegate(XRItem item) {
      // Set distance
      double camX = TeleportalPlayer.Current.transform.position.x;
      double camZ = TeleportalPlayer.Current.transform.position.z;
      double itemX = item.gameObject.transform.position.x;
      double itemZ = item.gameObject.transform.position.z;
      item.Distance = Math.Sqrt(Math.Pow(camX - itemX, 2.0) + Math.Pow(camZ - itemZ, 2.0));
    });
  }

}
