// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes the Teleportal logo rotate.
/// Attach this script to the GameObject
/// that contains the Canvas for the logo.
/// </summary>
public class LogoSpin : MonoBehaviour {

	// Seconds per full 360-degree rotation
	public float secPerRot = 20f;

	// The RectTransform component of this gameobject
	private RectTransform rectTransform;

	// The rotation vector to be applied on each frame
	private Vector3 rotationVector;

	void Start () {
		// Link gameobject component
		rectTransform = GetComponent<RectTransform>();

		// Calculate rotation vector.
		// Negative for clockwise Z rotation.
		// Time.deltaTime for "per frame" rotation.
		rotationVector = new Vector3(0, 0, -360 / secPerRot * Time.deltaTime);
	}
	
	void Update () {
		// Apply the rotation vector on each frame
		rectTransform.Rotate(rotationVector);
	}
}
