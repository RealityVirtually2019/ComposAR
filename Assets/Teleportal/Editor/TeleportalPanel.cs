// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class TeleportalPanel : EditorWindow {
  
  /// <summary>
  /// Status variable
  /// </summary>
  private bool Initialized = false;

  private string itemId = "";

  private GameObject main;
  private GameObject hand;
  private Texture2D tex;

  /// <summary>
  /// GUI Editor window
  /// </summary>
  [MenuItem("Window/XR/Teleportal")]
  public static void ShowWindow() {
    GetWindow<TeleportalPanel>("Teleportal");
  }

   /// <summary>
  /// Called when GUI is updated
  /// </summary>
	void OnGUI () {
    // Add title label
		GUILayout.Label("\\ Teleportal SDK /", EditorStyles.boldLabel);

    // Warn dev about active scene, if necessary
    if (this.WarnActiveScene()) {
      return;
    }
    
    // Prompt user to initialize
    if (!this.Initialized) {
      this.AddInitializeButton();
    }
    
    // Main logic GUI
    else {
      this.AddHideButton();
      this.AddTextFields();
      this.AddItemWizard();
    }
	}

  private bool WarnActiveScene() {
    // Check active scene name (should not be Teleportal library scene)
    if (SceneManager.GetActiveScene().name == "Teleportal") {
      GUILayout.Label("Switch to another scene to begin using Teleportal.");
      return true; // not OK to continue (yet)
    }
    
    // All OK
    return false;
  }

  private void AddInitializeButton() {
    if (!this.Initialized && GUILayout.Button("Initialize")) {
      this.Initialize();
    }
  }

  private void AddHideButton() {
    if (GUILayout.Button("Hide")) {
      this.Initialized = false;
    }
  }

  private void AddTextFields() {
    // Assign in real-time
    TeleportalProject.Shared.ApiKey = EditorGUILayout.TextField("API Key", TeleportalProject.Shared.ApiKey);
    TeleportalProject.Shared.Id = EditorGUILayout.TextField("Project ID", TeleportalProject.Shared.Id);
  }

  private void AddItemWizard() {
    // Create layout
    GUILayout.Label("XR Item Wizard", EditorStyles.boldLabel);
    itemId = EditorGUILayout.TextField("Item ID (string)", itemId);
    main = EditorGUILayout.ObjectField(main, typeof(GameObject), true) as GameObject;
    hand = EditorGUILayout.ObjectField(hand, typeof(GameObject), true) as GameObject;
    tex = EditorGUILayout.ObjectField(tex, typeof(Texture2D), true) as Texture2D;
    
    if (GUILayout.Button("Create XR Item +")) {
      // Check whether every object is filled in the panel
      if (main != null && hand != null && tex != null) {
        // Hand
        hand.name = "H_" + itemId;
        this.SavePrefab(hand);

        // Main
        main.name = "P_" + itemId;
        main.layer = LayerMask.NameToLayer("XR");
        XRItem xri = main.GetComponent<XRItem>();
        if (xri == null) {
          main.AddComponent(typeof(XRItem));
          xri = main.GetComponent<XRItem>();
        }
        xri.Type = XRItem.XRItemType.Inventory;
        xri.Id = itemId;
        this.SavePrefab(main);

        // Texture
        tex.name = "T_" + itemId;
        this.SaveTexture(tex);
      }
    }
  }

  private void Initialize() {
    this.SpawnTeleportalPrefab();
    this.AddTeleportalScene();
    this.Initialized = true;
  }

  private void SpawnTeleportalPrefab() {
    // Instantiate "Teleportal Project" prefab
    // if it doesn't already exist
    if (GameObject.Find("Teleportal Project") == null) {
      // Disable existing camera
      GameObject cam = GameObject.Find("Main Camera");
      if (cam != null) {
        cam.SetActive(false);
      }

      // Add Teleportal Project prefab
      GameObject prefab = Resources.Load("Teleportal Project", typeof(GameObject)) as GameObject;
      PrefabUtility.InstantiatePrefab(prefab);
    }
  }

  private void AddTeleportalScene() {
    this.AddSceneToBuildSettings("Assets/Teleportal/Scenes/Teleportal.unity");
  }

  private void SavePrefab(GameObject obj) {
    // Create folders if they don't already exist
    if (!AssetDatabase.IsValidFolder("Assets/Prefabs")) {
      AssetDatabase.CreateFolder("Assets", "Prefabs");
    }
    if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Resources")) {
      AssetDatabase.CreateFolder("Assets/Prefabs", "Resources");
    }

    string path = "Assets/Prefabs/Resources/" + obj.name + ".prefab";
    UnityEngine.Object prefab = PrefabUtility.CreatePrefab(path, obj);
    PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
  }

  private void SaveTexture(Texture2D tex) {
    File.WriteAllBytes("Assets/Prefabs/Resources/" + tex.name + ".png", (byte[]) tex.EncodeToPNG());
  }

  /// <summary>
  /// This function code by gilley033 on Unity Answers forum:
  /// http://answers.unity.com/answers/1428453/view.html
  /// </summary>
  /// <param name="pathOfSceneToAdd"></param>
  void AddSceneToBuildSettings(string pathOfSceneToAdd) {
    // Loop through and see if the scene already exist in the build settings.
    int indexOfSceneIfExist = -1;

    for (int i = 0; i < EditorBuildSettings.scenes.Length; i++) {
      if (EditorBuildSettings.scenes[i].path == pathOfSceneToAdd) {
        indexOfSceneIfExist = i;
        break;
      }
    }

    EditorBuildSettingsScene[] newScenes;

    if (indexOfSceneIfExist == -1) {
      newScenes = new EditorBuildSettingsScene[EditorBuildSettings.scenes.Length + 1];

      // Seems inefficent to add scene to build settings after creating each scene
      // (rather than doing it all at once, after they are all created).
      // However, it's necessary to avoid memory issues.
      int i = 0;
      for (; i < EditorBuildSettings.scenes.Length; i++) {
        newScenes[i] = EditorBuildSettings.scenes[i];
      }

      newScenes[i] = new EditorBuildSettingsScene(pathOfSceneToAdd, true);
    }
    else {
      newScenes = new EditorBuildSettingsScene[EditorBuildSettings.scenes.Length];

      int i = 0, j = 0;
      for (; i < EditorBuildSettings.scenes.Length; i++) {
        // Skip over the scene that is a duplicate.
        // This will effectively delete it from the build settings.
        if (i != indexOfSceneIfExist) {
          newScenes[j++] = EditorBuildSettings.scenes[i];
        }
      }
      newScenes[j] = new EditorBuildSettingsScene(pathOfSceneToAdd, true);
    }

    EditorBuildSettings.scenes = newScenes;
  }

}
