// Teleportal SDK
// Code by Thomas Suarez and C. Dillon Martin Hall
// Copyright 2018 WiTag Inc

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates a live webcam texture on a Canvas.
/// </summary>
public class CameraDeviceInput : MonoBehaviour {
	
	/// <summary>
	/// The camera texture displayed on the GUI.
	/// </summary>
	WebCamTexture webcamTexture;

	/// <summary>
	/// Whether the camera device has been acquired.
	/// </summary>
	bool isSet = false;

	/// <summary>
	/// MonoBehaviour load.
	/// </summary>
	void Start () {
		// Show camera input on this canvas rawimage texture
		webcamTexture = new WebCamTexture();
		RawImage rawImage = GetComponent<RawImage>();
		rawImage.texture = webcamTexture;
		webcamTexture.Play();
	}

	/// <summary>
	/// Updates on each frame.
	/// </summary>
	void Update() {
		// Set preview aspect ratio (if not set already)
		if (!isSet) {
			// Wait for camera lock
			if (webcamTexture.width > 100) {
				// Set aspect ratio
				GetComponent<AspectRatioFitter>().aspectRatio = (float)( (float)webcamTexture.width / (float)webcamTexture.height );

				// Reverse texture gameobject's vertical mirror if necessary (mainly an iOS issue)
				float scaleX = transform.localScale.x;
				float scaleY = transform.localScale.y * (float)(webcamTexture.videoVerticallyMirrored ? -1.0 : 1.0);
				float scaleZ = transform.localScale.z;
				transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

				// Prevent more executions of this function
				isSet = true;
			}
		}
	}
}
