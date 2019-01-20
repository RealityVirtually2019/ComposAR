using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComposarCamera : MonoBehaviour {

	private RenderTexture rentex;

	void Start () {
		this.rentex = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
		this.rentex.Create();
		this.gameObject.transform.Find("camera").GetComponent<Camera>().targetTexture = this.rentex;
	}

	public RenderTexture GetRenderTexture() {
		return this.rentex;
	}

	public Texture2D GrabImage() {
		// Convert to Texture2D
		Texture2D tex = new Texture2D(512, 512, TextureFormat.ARGB32, false);
		RenderTexture.active = this.rentex;
		tex.ReadPixels(new Rect(0, 0, this.rentex.width, this.rentex.height), 0, 0);
		tex.Apply();
		return tex;
	}

	public void SaveImage() {
		Texture2D tex = this.GrabImage();
		
		// TODO save to iOS/Android camera rolls
	}
	
}
