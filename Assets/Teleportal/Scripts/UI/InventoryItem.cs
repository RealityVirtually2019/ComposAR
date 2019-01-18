// WiTag Unity Client
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using System;
using UnityEngine;

/// <summary>
/// A game item that the player can retain across sessions.
/// </summary>
[Serializable]
public class InventoryItem {

  public int id;
  public string name;
  public string type;
  public int uses;
  public int maxUses;

  public InventoryItem(int _id, string _name, string _type, int _uses, int _maxUses) {
    // Set values
    id = _id;
    name = _name;
    type = _type;
    uses = _uses;
    maxUses = _maxUses;
  }

  public void Use() {
    // Visualize the item use
    TeleportalInventory.Shared.Use(this);
  }
  
}