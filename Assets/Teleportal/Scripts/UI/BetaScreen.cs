// Teleportal SDK
// Code by Thomas Suarez and Amir Yacaman
// Copyright 2018 WiTag Inc

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// Beta access window object.
/// </summary>
public class BetaScreen : MonoBehaviour {

  // GUI objects
  public RawImage Background;
  public Text Title;
  public Text Message;

  // TODO Beta Input Field

  ///// LIFECYCLE /////

  void Start() {
    // Scale title & message
    ScaleText();

    // Fade in
    float fadeTime = 1.0f;
    StartCoroutine(FadeRawImage(Background, 0.0f, 1.0f, fadeTime));
    StartCoroutine(FadeText(Title, 0.0f, 1.0f, fadeTime));
    StartCoroutine(FadeText(Message, 0.0f, 1.0f, fadeTime));
  }

  public void ScaleText() {
    Title.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, Screen.height);
    Message.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, Screen.height / 2);
  }

  public void FadeOutAfterSeconds(int seconds) {
    StartCoroutine(FadeOutAfterSecondsI(seconds));
  }

  public IEnumerator FadeOutAfterSecondsI(int seconds) {
    // Wait for seconds
    yield return new WaitForSeconds(seconds);

    // Fade out
    float fadeTime = 1.0f;
    StartCoroutine(FadeRawImage(Background, 1.0f, 0.0f, fadeTime));
    StartCoroutine(FadeText(Title, 1.0f, 0.0f, fadeTime));
    yield return FadeText(Message, 1.0f, 0.0f, fadeTime);

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

  IEnumerator FadeText(Text text, float alphaFrom, float alphaTo, float time) {
    for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time) {
      Color newColor = text.color;
      newColor.a = Mathf.Lerp(alphaFrom, alphaTo, t);
      text.color = newColor;
      yield return new WaitForEndOfFrame();
    }
  }

  ///// PROPERTY SETTERS /////

  public void SetTitle(string title) {
    // Get GUI text component of title gameobject, then set new title string to it.
    Title.text = title;
  }

  public void SetMessage(string message) {
    // Get GUI text component of message gameobject, then set new message string to it.
    Message.text = message;
  }

}
