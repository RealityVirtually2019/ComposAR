// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Shared module for controlling beta access on a user level.
/// </summary>
public class TeleportalBeta : MonoBehaviour {

  /* Shared Object References */

  public static TeleportalBeta Shared;

  /* Buildtime beta properties */

  public bool BetaMode = true;

  /* Runtime beta properties */

  /// <summary>
  /// The current number of players waiting in line for WiTag beta access.
  /// </summary>
  public string BetaWaitAhead = "0";

  /// <summary>
  /// Called before Start().
  /// </summary>
  void Awake() {
    // Set static self reference
    TeleportalBeta.Shared = this;
  }

}
