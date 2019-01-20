/// animaid @ MIT Reality Virtually Hacakthon 2019 ///
/// Thomas Suarez, Matt Kelsey, Ryan Reede, Sam Roquitte, Nick Grana ///

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot {

	public GameObject DefaultCameraPrefab; // prefab reference (set in inspector)

	private string name;
	private XRItem XRI;
	private Camera camera;

	public Shot() : this("0") {}

	public Shot(string name) : this(name, null, null) {}

	public Shot(string name, XRItem item, Camera camera) {
		// Set variables
		this.name = name;
		this.XRI = item;
		this.camera = camera;

		// Create camera if does not exist
		if (this.camera == null) {
			this.camera = GameObject.Instantiate(DefaultCameraPrefab).GetComponent<Camera>();
		}

		// Add XRItem if does not exist
		if (this.XRI == null) {
			this.XRI = this.camera.gameObject.AddComponent(typeof(XRWorldItem)) as XRItem;
		}
	}

	public string GetName() {
		return this.name;
	}

	public void SetName(string name) {
		this.name = name;
	}

	public XRItem GetXRItem() {
		return this.XRI;
	}

	public void SetXRItem(XRItem item) {
		this.XRI = item;
	}

	public string GetId() {
		return this.XRI.Id;
	}

	public Camera GetCamera() {
		return this.camera;
	}

	public void SetCamera(Camera camera) {
		this.camera = camera;
	}

	public Transform GetTransform() {
		return this.camera.gameObject.transform;
	}

	public string GetSerializedTransform() {
		// Get from helper class
		return ComposarHelper.SerializeTransform(this.GetTransform());
	}

}
