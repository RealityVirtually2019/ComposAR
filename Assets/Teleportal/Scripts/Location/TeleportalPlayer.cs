// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines a shared player GameObject class.
/// </summary>
public class TeleportalPlayer : MonoBehaviour {

	/* Shared Object References */

	/// <summary>
	/// The current Teleportal Player GameObject.
	/// </summary>
	public static GameObject Current;

	/// <summary>
	/// Called before Start().
	/// </summary>
	void Awake() {
		// Link object references
		TeleportalPlayer.Current = gameObject;
	}

}
