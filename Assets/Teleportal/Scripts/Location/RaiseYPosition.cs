// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Raises the Y position of the parent Transform by the specified amount.
/// </summary>
public class RaiseYPosition : MonoBehaviour {

	// User-defined value to add to Y position (in meters)
	public float AddY = 1.5f;

	// Transform
	private Transform t;
	private Vector3 origPos;
	private Vector3 newPos;

	/// <summary>
	/// Called before Start().
	/// </summary>
	void Awake() {
		// Get current position
		t = transform.parent;
		origPos = t.position;

		// Calculate raised (new) position
		newPos = origPos;
		newPos.y += AddY;
	}

	/// <summary>
	/// Standard MonoBehaviour Start function.
	/// </summary>
	void Start() {
		// Execute now
		OnEnable();
	}

	/// <summary>
	/// Makes modifications to the parent Transform's position.
	/// </summary>
	void OnEnable() {
		// Set to raised (new) position
		t.position = newPos;
	}

	/// <summary>
	/// Reverses modifications made in OnEnable().
	/// </summary>
	void OnDisable() {
		// Set back to original position
		t.position = origPos;
	}

}
