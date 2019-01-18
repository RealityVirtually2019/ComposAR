// Teleportal SDK
// Copyright 2018 WiTag Inc

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Deprecated.
/// </summary>
public class SceneLoader : MonoBehaviour {
	
	public static int CurrentIndex = 0;

	public string[] sceneNames;
	public int index = 0;
	public bool asyncMode;

	void Awake() {
		if (!asyncMode) {
			foreach (string sceneName in sceneNames) {
				SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
			}
		}
	}

	void Start() {
		if (asyncMode) {
			StartCoroutine(LoadSceneAsync());
		}
	}

	private IEnumerator LoadSceneAsync() {
		while (SceneLoader.CurrentIndex != this.index) {
			yield return new WaitForSeconds(0.1f);
		}

		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNames[0], LoadSceneMode.Additive);

		// Wait until the asynchronous scene fully loads
		while (!asyncLoad.isDone)
		{
			SceneLoader.CurrentIndex += 1;
			yield return null;
		}
    }
}
