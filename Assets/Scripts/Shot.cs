/// ComposAR @ MIT Reality Virtually Hacakthon 2019 ///
/// Thomas Suarez, Matt Kelsey, Ryan Reede, Sam Roquitte, Nick Grana ///

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot {

	public GameObject DefaultCameraPrefab; // prefab reference (set in inspector)

	private string name;
	private string id; // XR Item ID of this Shot
	private Camera camera;

	public Shot() : this("0") {}

	public Shot(string name) : this(name, null, null) {}

	public Shot(string name, string id, Camera camera) {
		// Set variables
		this.name = name;
		this.id = id;
		this.camera = camera;

		// Create camera if does not exist
		if (this.camera == null) {
			this.camera = Instantiate(DefaultCameraPrefab).GetComponent<Camera>();
		}

		// Add XRItem if does not exist
		if (this.id == null) {
			this.id = gameObject.AddComponent(typeof(XRWorldItem)) as XRItem;
		}
	}

	public string GetName() {
		return this.name;
	}

	public string GetId() {
		return this.id;
	}

	public Camera GetCamera() {
		return this.camera;
	}

	public Transform GetTransform() {
		return this.camera.gameObject.transform;
	}

	public Vector3 GetPosition() {
		return this.GetTransform().position;
	}

	public Vector3 GetRotation() {
		return this.GetTransform().eulerAngles;
	}

	public Vector3 GetScale() {
		return this.GetTransform().localScale;
	}

	public string GetSerializedTransform() {
		// Get from helper class
		return ComposarHelper.SerializeTransform(this.GetTransform());
	}

}
