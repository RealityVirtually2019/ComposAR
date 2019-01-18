// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes the parent Transform a child of the immersive camera.
/// </summary>
public class ParentToImmersiveCamera : MonoBehaviour {

	// Initializer function
	void Start() {
		// Move to be a child under the immersive camera
		WaitThenSetParent(TeleportalAr.Shared.CurrentCamera.transform);
	}

	void WaitThenSetParent(Transform newParent) {
		StartCoroutine(WaitThenSetParentI(newParent));
	}

	IEnumerator WaitThenSetParentI(Transform newParent) {
		// Wait 1 second
		yield return new WaitForSeconds(1);

		// Move to be a child under the new parent
		transform.parent.SetParent(newParent, false);

		// Finish
		yield return null;
	}

}
