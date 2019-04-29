/// animaid @ MIT Reality Virtually Hacakthon 2019 ///
/// Thomas Suarez, Matt Kelsey, Ryan Reede, Sam Roquitte, Nick Grana ///

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Teleportal;

public class Misc : MonoBehaviour {

	void Start() {
		Teleportal.Project.Shared.OnTeleportalLoaded += OnTeleportalLoaded;
	}

	void OnTeleportalLoaded() {
		// Teleportal.Inventory.Shared.BarUI.active = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
