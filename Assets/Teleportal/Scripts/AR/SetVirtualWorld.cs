// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the Virtual CG World background,
/// and its transition to/from AR Camera Passthrough.
/// </summary>
public class SetVirtualWorld : MonoBehaviour {

	/// <summary>
	/// Whether the viewport should default
	/// to AR mode (true; camera passthrough)
	/// or VR mode (false; environment background).
	/// The user can switch between these modes
	/// while the app is running
	/// (see the UI section of the Teleportal Docs for more info).
	/// </summary>
	public bool DefaultToAR = false;

  void Start() {
    TeleportalProject.Shared.OnTeleportalLoaded += OnTeleportalLoaded;
  }

  void OnTeleportalLoaded() {
    TeleportalAr.Shared.VirtualWorld = gameObject;

		// Switch from CG -> AR mode
		// (if specified)
		if (DefaultToAR) {
			TeleportalAr.Shared.ToggleVirtualWorld();
		}
  }

}
