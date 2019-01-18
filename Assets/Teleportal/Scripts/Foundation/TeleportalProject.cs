// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class TeleportalProject : MonoBehaviour {

	/* Shared Object References */
	public static TeleportalProject Shared;

	public string ApiKey = "";
	public string Id = "";
  public string[] DefaultInventoryItems = new string[] {};

  public UnityAction OnTeleportalLoaded;

  void OnGUI() {
    // If in Edit mode
    if (!Application.isPlaying) {
      // Set static self reference
      TeleportalProject.Shared = this;
    }
  }

	void Start() {
    // If in Edit mode
    if (!Application.isPlaying) {
      return;
    }

		// Set static self reference
		TeleportalProject.Shared = this;
    
    StartCoroutine(WaitForTeleportalLoad());

		// Load Teleportal scene
		SceneManager.LoadScene("Teleportal", LoadSceneMode.Additive);
	}

  private IEnumerator WaitForTeleportalLoad() {
    // Wait until not null
    while (TeleportalActions.Shared == null) {
      yield return new WaitForSeconds(0.1f);
    }

    // Callback action
    OnTeleportalLoaded();

    // Attach core actions
    TeleportalActions.Shared.OnLocationLock += OnLocationLock;
  }

  private void OnLocationLock() {
    foreach (string itemId in DefaultInventoryItems) {
      TeleportalInventory.Shared.RequestAdd("Item", itemId);
    }
  }

	public void Send(params string[] msg) {
		// Relay to TeleportalNet
		TeleportalNet.Shared.Send(TeleportalProject.Shared.Id, msg);
	}

	public void RegisterCommand(string cmd, System.Action<List<string>> func) {
		TeleportalNet.Shared.RegisterCommand(this.Id, cmd, func);
	}

}
