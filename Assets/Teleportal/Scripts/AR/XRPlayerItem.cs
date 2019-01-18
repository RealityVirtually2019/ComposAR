// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;

/// <summary>
/// A 3D player object that is shown and controlled in the XR world.
/// </summary>
public class XRPlayerItem: XRItem {

  /* Referenced GameObjects */

  /// <summary>
  /// The GUI mesh for the title text.
  /// </summary>
  public TextMesh TitleMesh;

  [HideInInspector]
  /// <summary>
  /// Only applies to XR Items with a Type value of Player.
  /// The GameObject that is currently being used as this Player's avatar.
  /// </summary>
  public GameObject CurrentAvatar;

  /// <summary>
  /// All avatars to which this Player can switch.
  /// </summary>
  public GameObject[] Avatars;

  /// Seconds over which to animate player movement in scene.
  /// For AR player location smoothing.
  /// The most recent value of the networked XR Item state's last property.
  /// </summary>
  private float AnimationTime = 0.5f;

  /// <summary>
  /// The currently-running IEnumerator for animating this XRItem's position and rotation.
  /// </summary>
  private IEnumerator AnimationE;

  private void CustomItemTypeLogic() {
    // Specific to Players
    this.Type = XRItemType.Player;
  }

  /// <summary>
  /// Changes the visible title of this Item.
  /// </summary>
  /// <param name="newTitle">New title for this Item.</param>
  public void ChangeTitle(string newTitle) {
    // Set object variable to new value
    Title = newTitle;

    // Set text component to the new title
    TitleMesh.text = Title;
  }

  
  /// <summary>
  /// Only used for Items with a Type value of Player.
  /// Sets a new avatar for this Player Item. 
  /// </summary>
  /// <param name="type">The new AvatarType that this Player Item should manifest.</param>
  public void SetAvatar(AvatarType type) {
    // Look up the avatar gameobject's array index
    int index = (int) type;

    // If there is already an avatar enabled, disable it
    if (CurrentAvatar != null) {
      CurrentAvatar.SetActive(false);
    }

    // Select the new avatar, based on array index
    CurrentAvatar = Avatars[index];

    // Enable it
    CurrentAvatar.SetActive(true);
  }

}
