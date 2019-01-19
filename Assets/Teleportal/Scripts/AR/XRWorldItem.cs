// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;

/// <summary>
/// A 3D world object that is shown and controlled in the XR world.
/// </summary>
public class XRWorldItem: XRItem {

  [HideInInspector]
  /// <summary>
  /// TODO doc
  /// </summary>
  public static int NextId = 0;

  public void Awake() {
      // TP // TeleportalActions.Shared.OnLocationLock += Start;
  }

  public override void Start() {
    // Specific to World objects
    this.Type = XRItemType.World;

    // Generate next ID
    this.Id = XRWorldItem.NextId.ToString();
    XRWorldItem.NextId += 1;

    // Add to List
    // TP // TeleportalAr.Shared.CreateRealmItem("0", this); // 0 is the realm id. TEMP
  }

}
