// WiTag Unity Client
// Copyright 2018 WiTag Inc

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Shared object for controlling the local player's inventory.
/// </summary>
public class TeleportalInventory : MonoBehaviour {

  /* Shared Object References */
  public static TeleportalInventory Shared;

  /* Runtime properties */

  /// <summary>
  /// Whether the Inventory "treasure chest" is currently showing.
  /// </summary>
  private bool Showing = false;

  /* Objects */

  /// <summary>
  /// A "treasure chest" model of items.
  /// </summary>
  public GameObject Chest;

  /// <summary>
  /// 
  /// </summary>
  public GameObject Hand;

  /// <summary>
  /// The Inventory Bar UI canvas.
  /// Attached in the User GameObject
  /// of Teleportal.scene.
  /// </summary>
  public GameObject BarUI;

  /// <summary>
  /// A dictionary of all Inventory Items
  /// in this Player's inventory
  /// (both in the Inventory Bar and hidden).
  /// </summary>
  public Dictionary<string, InventoryItem> Inventory;

  /// <summary>
  /// A dictionary of all Inventory Items
  /// in this Player's inventory bar.
  /// </summary>
  public Dictionary<string, InventoryItem> BXRItems;

  /// <summary>
  /// An array of all Inventory Items in this Player's inventory.
  /// This array is re-generated every time the Player's
  /// Inventory Bar is updated.
  /// </summary>  
  // // TP // private -> public
  public InventoryItem[] InventoryArray;

  /// <summary>
  /// The current Inventory Item being held by the Inventory.
  /// </summary>
  // // TP // private -> protected 
  public InventoryItem CurrentItem;

  ///// LIFECYCLE /////

  void Awake() {
    // Set static self reference
    TeleportalInventory.Shared = this;
  }

  /// <summary>
  /// MonoBehaviour load.
  /// </summary>
  void Start() {
    // Initialize objects
    Inventory = new Dictionary<string, InventoryItem>();
    BXRItems = new Dictionary<string, InventoryItem>();
    InventoryArray = new InventoryItem[0];
  }

  void Update() {
    // FIXME "Inventory" is not mapped in the input settings,
    // so referencing it below causes the script to stop running!
    // Commented for now, as the chest isn't used anyway.
    //DetectSummonChest();
    
    DetectItemBarChange();
  }

  ///// INPUT /////

  /// <summary>
  /// Detects when the Chest is summoned by
  /// the user's key press.
  /// </summary>
  private void DetectSummonChest() {
    // If the user summons the inventory chest
    if (Input.GetButtonDown("Inventory")) {
      // Toggle UI showing boolean
      Showing = !Showing;

      // Determine the correct animation clip name
      string clipName;
      if (Showing) { clipName = "ChestOpen"; }
      else { clipName = "ChestClose"; }

      // Play the correct animation clip
      Chest.GetComponent<Animator>().Play(clipName);
    }
  }

  /// <summary>
  /// Detects when an Item is summoned to change
  /// by the user's key press (numerical keys 1-5)
  /// </summary>
  private void DetectItemBarChange() {
    if (Input.GetKeyDown("1")) {
      SetItem(0);
    }
    else if (Input.GetKeyDown("2")) {
      SetItem(1);
    }
    else if (Input.GetKeyDown("3")) {
      SetItem(2);
    }
    else if (Input.GetKeyDown("4")) {
      SetItem(3);
    }
    else if (Input.GetKeyDown("5")) {
      SetItem(4);
    }
  }

  ///// INVENTORY /////

  /// <summary>
  /// Asks Teleportal to use the specified Inventory Item.
  /// </summary>
  /// <param name="item">The Inventory Item to use.</param>
  public void Use(InventoryItem item) {
    // Get values
    int id = item.id;
    int heading = (int) TeleportalAr.Shared.CurrentCamera.transform.eulerAngles.y;
    int pitch = (int) TeleportalAr.Shared.CurrentCamera.transform.eulerAngles.x;
    
    // Tell server
    TeleportalNet.Shared.Send(TeleportalCmd.ITEM_USE, id.ToString(), heading.ToString(), pitch.ToString());

    // Add 1 use to the item (client-side)
    item.uses += 1;

    // Check if the item has been depleted of uses (client-side)
    if (item.uses >= item.maxUses && item.maxUses != -1) {
      // Remove it from the dictionary
      Remove(item);
    }

    // Update the "remaining" subtexts
    UpdateBarUI();
  }

  /// <summary>
  /// Uses the currently-selected Inventory Item,
  /// if there is one available and selected.
  /// </summary>
  public void UseCurrent() {
    // Use the current item, if there is one
    if (CurrentItem != null) {
      Use(CurrentItem);
    }
  }

  /// <summary>
  /// Clears the entire Inventory, client-side only.
  /// </summary>
  public void Clear() {
    // Clear entire inventory
    Inventory.Clear();

    // Update UI
    UpdateView();
  }

  /// <summary>
  /// Called by Teleportal.
  /// Adds an Inventory Item, client-side only.
  /// </summary>
  /// <param name="item">The Inventory Item to add.</param>
  public void Add(InventoryItem item) {
    // Add item to client-side inventory, at its stringified ID
    Inventory[item.id.ToString()] = item;

    // Update UI
    UpdateView();
  }

  public void RequestAdd(String itemName, String itemType) {
    // Request Teleportal inventory item add
    TeleportalNet.Shared.Send(TeleportalCmd.META_INVENTORY_ADD, itemName, itemType);
  }

  /// <summary>
  /// Called by Teleportal.
  /// Removes an Inventory item, client-side only.
  /// </summary>
  /// <param name="item">The Inventory Item to remove.</param>
  public void Remove(InventoryItem item) {
    // Remove item from the client-side inventory, at its stringified ID
    Inventory.Remove(item.id.ToString());

    // Update UI
    UpdateView();
  }

  /// <summary>
  /// Updates all Inventory visualizers:
  /// The "treasure chest", Inventory Bar UI, and Hand model.
  /// </summary>
  private void UpdateView() {
    // Update all inventory visualizers
    SetItem(-1); // nullify
    UpdateChest();
    UpdateBarUI();
    UpdateHand();
    AutoSelectItem();
  }

  /// <summary>
  /// Updates the Inventory Items in the "Treasure Chest".
  /// Currently unimplemented.
  /// </summary>
  private void UpdateChest() {
    // TODO update chest
  }

  /// <summary>
  /// Updates the Hand model to the currently-selected
  /// Inventory Item from the Bar UI.
  /// </summary>
  private void UpdateHand() {
    int i = 0;
    int max = BarUI.transform.childCount;
    Transform p = Hand.transform;

    // First, destroy each existing hand item
    foreach (Transform child in p) {
      Destroy(child.gameObject);
    }

    foreach (InventoryItem item in Inventory.Values.ToArray()) {

      // There is only a certain amount of space on the Inventory UI Bar,
      // so we can't go past the maximum number of bar items.
      if (i >= max) {
        break;
      }

      // Add the appropriate prefab object
      string path = "H_" + item.type; // "H" means "hand" here -- the object the user is "holding"
      Debug.Log(path);
      GameObject prefab = Resources.Load(path) as GameObject;
      if (prefab == null) { continue; } // skip this item type, if the current client does not include it
      GameObject g = Instantiate(prefab, Vector3.zero, Quaternion.identity, p);
      g.transform.localPosition = prefab.transform.position;
      g.transform.localRotation = prefab.transform.rotation;

      // Disable each hand item, until the user selects one.
      g.active = false;
      
      // Increment the current number of filled inventory UI bar items
      i++;
    }
  }

  /// <summary>
  /// Updates the Bar UI
  /// </summary>
  private void UpdateBarUI() {
    int i = 0;
    int max = BarUI.transform.childCount;

    // First, reset each existing bar item
    string blankTexPath = "Item_Blank";
    Texture2D blankTex = Resources.Load(blankTexPath) as Texture2D;

    foreach (Transform child in BarUI.transform) {
      RawImage image = child.GetComponent<RawImage>();
      Text text = child.GetChild(0).GetComponent<Text>();
      image.texture = blankTex;
      text.text = "";
    }

    // Clear current inventory item <-> bar item associations
    BXRItems.Clear();

    // Convert to array
    InventoryArray = Inventory.Values.ToArray();

    foreach (InventoryItem item in InventoryArray) {

      // There is only a certain amount of space on the Inventory UI Bar,
      // so we can't go past the maximum number of bar items.
      if (i >= max) {
        break;
      }

      // Get the child gameobject (bar item) of the UI bar,
      // at the currently-selected index.
      Transform bXRItem = BarUI.transform.GetChild(i);
      RawImage image = bXRItem.gameObject.GetComponent<RawImage>();
      Transform textItem = bXRItem.GetChild(0);
      Text text = textItem.gameObject.GetComponent<Text>();

      // Set the new image texture
      string path = "T_" + item.type; // "T" means "thumbnail" image here
      Texture2D tex = Resources.Load(path) as Texture2D;
      if (tex == null) { continue; } // skip this item type, if the current client does not include it
      image.texture = tex;

      // Set the new text
      int remaining = item.maxUses - item.uses;
      text.text = remaining.ToString();

      // Set the text to infinity,
      // if there are unlimited uses
      if (item.maxUses == -1) {
        text.text = "∞";
      }

      // Assign the inventory item to the bar item
      // at the current position: i
      BXRItems[i.ToString()] = item;
      
      // Increment the current number of filled inventory UI bar items
      i++;
    }
  }

  /// <summary>
  /// Sets the new InventoryItem for the "Treasure Chest", Hand, and Bar UI.
  /// </summary>
  /// <param name="id"></param>
  public void SetItem(int id) {
    // Cancel if the requested ID does not exist
    if (id >= InventoryArray.Length) {
      return;
    }

    // -1 means no item (aka null)
    if (id == -1) {
      CurrentItem = null;
      return;
    }

    // Set the current item, at the correct _sequential_ item ID in the bar UI
    // (not necessarily inventory ID)
    CurrentItem = InventoryArray[id];

    // Visualize each UI type
    SetChestItem(id);
    SetHandItem(id);
    SetBXRItem(id);
  }


  /// <summary>
  /// Selects an Inventory Item in the Chest.
  /// Currently unimplemented.
  /// </summary>
  /// <param name="id">The ID of the Inventory Item to select.</param>
  private void SetChestItem(int id) {
    // TODO set chest item
  }

  /// <summary>
  /// Selects an Inventory Item for the Hand model.
  /// </summary>
  /// <param name="id">The ID of the Inventory Item to select.</param>
  private void SetHandItem(int id) {
    // Disable all hand items
    for (int i = 0; i < InventoryArray.Length; i++) {
      // If there are no more children in the Hand,
      // stop here.
      if (Hand.transform.childCount <= i) {
        break;
      }

      // Get the Hand item and disable its GameObject
      GameObject handItem = Hand.transform.GetChild(i).gameObject;
      handItem.active = false;
    }

    // Enable the requested bar item
    GameObject newHandItem = Hand.transform.GetChild(id).gameObject;
    newHandItem.active = true;
  }

  /// <summary>
  /// Selects an Inventory Item for the Bar UI.
  /// </summary>
  /// <param name="id">The ID of the Inventory Item to select.</param>
  private void SetBXRItem(int id) {
    // Reset scale for all bar items
    for (int i = 0; i < InventoryArray.Length; i++) {
      Transform bXRItem = BarUI.transform.GetChild(i);
      bXRItem.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    // Scale up the requested bar item
    Transform newBXRItem = BarUI.transform.GetChild(id);
    newBXRItem.localScale = new Vector3(1.25f, 1.25f, 1.25f);
  }

  /// <summary>
  /// Automatically selects the first Inventory Item
  /// if there is no currently selected one already.
  /// </summary>
  private void AutoSelectItem() {
    // If there is at least one item,
    // and there is no currently selected item,
    // select the first item.
    if (Inventory.Count >= 1 && CurrentItem == null) {
      SetItem(0);
    }
  }

}
