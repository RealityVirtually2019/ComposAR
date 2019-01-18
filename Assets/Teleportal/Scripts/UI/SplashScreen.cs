// Teleportal SDK
// Code by Thomas Suarez and Amir Yacaman
// Copyright 2018 WiTag Inc

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// Timed, branded "splash" screen for the game.
/// </summary>
public class SplashScreen : MonoBehaviour {

  // GUI objects
  public RawImage Background;
  public RawImage Logo;

  ///// LIFECYCLE /////

  public void FadeOutAfterSeconds(int seconds) {
    StartCoroutine(FadeOutAfterSecondsI(seconds));
  }

  public IEnumerator FadeOutAfterSecondsI(int seconds) {
    // Wait for seconds
    yield return new WaitForSeconds(seconds);

    // Fade out
    float fadeTime = 1.0f;
    StartCoroutine(FadeRawImage(Background, 1.0f, 0.0f, fadeTime));
    yield return FadeRawImage(Logo, 1.0f, 0.0f, fadeTime);

    // Destroy this gameobject
    Destroy(gameObject);
  }

  IEnumerator FadeRawImage(RawImage rawImage, float alphaFrom, float alphaTo, float time) {
    for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time) {
      Color newColor = rawImage.color;
      newColor.a = Mathf.Lerp(alphaFrom, alphaTo, t);
      rawImage.color = newColor;
      yield return new WaitForEndOfFrame();
    }
    yield return null;
  }

}
