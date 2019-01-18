// Teleportal SDK
// Copyright 2018 WiTag Inc

using System;
using UnityEngine;
using UnityEngine.UI;

public class Modules : MonoBehaviour {

  /* Shared Object References */
  public static Modules Shared;

  void Awake() {
    // Set static self reference
    Modules.Shared = this;
  }

}