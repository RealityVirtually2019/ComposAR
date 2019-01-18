// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Shared module for purchasing/licensin of first and third-party modules.
/// </summary>
public class TeleportalModules : MonoBehaviour {

  /* Shared Object References */

  public static TeleportalModules Shared;

  /* Shared object reference */

  /// <summary>
  /// Called before Start().
  /// </summary>
  void Awake() {
    // Set static self reference
    TeleportalModules.Shared = this;
  }

}
