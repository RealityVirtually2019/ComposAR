// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPlayer : MonoBehaviour {

	/// <summary>
	/// Calls TeleportalGps.HighFivePrompt().
	/// More easily accessible from scenes outside
	/// of the Teleportal library scene.
	/// </summary>
	/// <param name="username">The name of the user with whom to synchronize locations.</param>
	public void SyncWith(string username) {
		TeleportalGps.Shared.HighFivePrompt(username);
	}

}
