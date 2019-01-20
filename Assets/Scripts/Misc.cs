/// animaid @ MIT Reality Virtually Hacakthon 2019 ///
/// Thomas Suarez, Matt Kelsey, Ryan Reede, Sam Roquitte, Nick Grana ///

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Misc : MonoBehaviour {

	void Start() {
		TeleportalProject.Shared.OnTeleportalLoaded += OnTeleportalLoaded;
	}

	void OnTeleportalLoaded() {
		// TeleportalInventory.Shared.BarUI.active = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
