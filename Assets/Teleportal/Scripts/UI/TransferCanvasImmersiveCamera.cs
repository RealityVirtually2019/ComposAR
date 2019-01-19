// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Transfers the parent Canvas's world camera setting to the immersive camera.
/// </summary>
public class TransferCanvasImmersiveCamera : MonoBehaviour {

	public Canvas UICanvas;

	// Initializer function
	void Start() {
		// Transfer render camera to the immersive camera
		UICanvas.worldCamera = TeleportalAr.Shared.CurrentCamera;
	}

}
