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

	public Canvas UICanvas;

	// Initializer function
	void Start() {
		// Move to be a child under the immersive camera
		WaitThenSetParent();
	}

	void WaitThenSetParent() {
		StartCoroutine(WaitThenSetParentI());
	}

	IEnumerator WaitThenSetParentI() {
		// Wait 1 second
		yield return new WaitForSeconds(1);

		// Move to be a child under the new parent
		// TP //
		UICanvas.worldCamera = TeleportalAr.Shared.CurrentCamera;
		Transform newParent = UICanvas.worldCamera.transform;
		transform.parent.SetParent(newParent, false);

		// Finish
		yield return null;
	}

}
