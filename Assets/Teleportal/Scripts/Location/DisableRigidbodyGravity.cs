// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Disables gravity on the parent GameObject's rigidbody.
/// </summary>
public class DisableRigidbodyGravity : MonoBehaviour {

	// Object references
	private Rigidbody rb;

	/// <summary>
	/// Called before Start().
	/// </summary>
	void Awake() {
		// Link object references
		rb = transform.parent.gameObject.GetComponent<Rigidbody>();
	}

	/// <summary>
	/// Standard MonoBehaviour Start() function.
	/// </summary>
	void Start() {
		rb.useGravity = false;
		rb.isKinematic = true;
	}

}
