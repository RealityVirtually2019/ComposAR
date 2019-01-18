// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachARToCurrentCamera : MonoBehaviour {

	void Start() {
		// TeleportalActions.Shared.OnWorldLoaded += SetCamera;
		SetCamera();
	}
	
	private void SetCamera() {
		UnityARCameraManager manager = GetComponent<UnityARCameraManager>();
		manager.m_camera = TeleportalAr.Shared.CurrentCamera;
	}
	
}
