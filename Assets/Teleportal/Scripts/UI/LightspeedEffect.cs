// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LightspeedEffect : MonoBehaviour {

	public static LightspeedEffect Shared;

	private VideoPlayer VideoView;
	private RawImage rawImage;

	void Awake() {
		// Set singleton instance
		LightspeedEffect.Shared = this;
	}

	// Initialization method
	void Start () {
		VideoView = GetComponent<VideoPlayer>();
		VideoView.loopPointReached += EndReached;
		VideoView.Prepare();

		rawImage = GetComponent<RawImage>();
	}

	private void EndReached(VideoPlayer player) {
		Stop();
	}

	public void Play() {
		// Show this object
		rawImage.enabled = true;

		// Start playing animation
		VideoView.Play();
		
		// Play the sound
		TeleportalUi.Shared.PlaySound(TeleportalUi.SoundType.Teleport);
	}

	public void Stop() {
		// Hide this object
		rawImage.enabled = false;
		
		// Stop playing animation
		VideoView.Stop();
		
		// Reset to beginning
		VideoView.frame = 0;
	}
}
