// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scales the attached Canvas based on display resolution.
/// </summary>
public class CanvasScaler : MonoBehaviour {

	/// <summary>
	/// Distance from camera / fulcrum.
	/// </summary>
	public float distance = 1.0f;

	public bool ShouldScale = true;

	/// <summary>
	/// MonoBehaviour load. Scales this UI canvas to the screen size.
	/// </summary>
	void Start () {
		// Scale to display resolution
		RectTransform rt = GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2(Screen.width, Screen.height);

		// Calculate value
		float val = 2.0f * distance * Mathf.Tan(Mathf.Deg2Rad * (TeleportalAr.Shared.CurrentCamera.fieldOfView * 0.5f));
		float scale = val / rt.sizeDelta.y;

		// Reposition this UI canvas
		transform.position += new Vector3(0, 0, distance);

		// Scale this UI canvas
		if (ShouldScale) {
			transform.localScale = new Vector3(scale, scale, scale);
		}
	}

}
