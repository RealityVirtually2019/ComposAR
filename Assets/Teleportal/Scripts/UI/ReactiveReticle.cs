// Teleportal SDK
// Code by Thomas Suarez and C. Dillon Martin Hall
// Copyright 2018 WiTag Inc

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// Floating, fixed-position reticle that dynamically changes, in response to external stimuli.
/// </summary>
public class ReactiveReticle: MonoBehaviour {

  // Enumerated types
  public enum ReticleType {
    Ready, NotReady, Heal, FireHit,
    FireMiss, PickupInRange, PickupOutOfRange
  };

  // Properties
  ReticleType Type;

  // Referenced gameobjects
  public Texture Ready;
  public Texture NotReady;
  public Texture Heal;
  public Texture FireHit;
  public Texture FireMiss;
  public Texture PickupInRange;
  public Texture PickupOutOfRange;

  /// <summary>
  /// Activates (shows) the Reactive Reticle.
  /// </summary>
  public void Show() {
    gameObject.SetActive(true);
  }

  /// <summary>
  /// De-activates (hides) the Reactive Reticle.
  /// </summary>
  public void Hide() {
    gameObject.SetActive(false);
  }
  
  public void SetType(ReticleType type) {
    // Set global variable
    Type = type;

    // Get image texture
    Texture texture = GetImage(type);

    // Change image in GUI
    SetImage(texture);
  }

  /* Gets the image texture for a reticle enumerated type */
  private Texture GetImage(ReticleType type) {
    // Init
    string name;
    Texture texture;

    // Decide
    switch (type) {
      case ReticleType.Ready:
        name = "ready";
        texture = Ready;
        break;
      case ReticleType.NotReady:
        name = "notready";
        texture = NotReady;
        break;
      case ReticleType.Heal:
        name = "heal";
        texture = Heal;
        break;
      case ReticleType.FireHit:
        name = "fire_hit";
        texture = FireHit;
        break;
      case ReticleType.FireMiss:
        name = "fire_miss";
        texture = FireMiss;
        break;
      case ReticleType.PickupInRange:
        name = "pickup_in";
        texture = PickupInRange;
        break;
      case ReticleType.PickupOutOfRange:
        name = "pickup_out";
        texture = PickupOutOfRange;
        break;
      default: // default: not ready
        name = "notready";
        texture = NotReady;
        break;
    }

    // Return
    return texture;
  }

  /* Changes the visible image representation of this reticle in the GUI */
  public void SetImage(Texture texture) {
    GetComponent<RawImage>().texture = texture;
  }

}
