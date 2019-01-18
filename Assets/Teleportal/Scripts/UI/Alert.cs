// Teleportal SDK
// Code by Thomas Suarez and Amir Yacaman
// Copyright 2018 WiTag Inc

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// UI Alert window object.
/// </summary>
public class Alert : MonoBehaviour {

  /* GUI objects */

  /// <summary>
  /// The GUI image element displaying the background color on the alert.
  /// </summary>
  public RawImage Background;

  /// <summary>
  /// The GUI text element displaying the alert's title.
  /// </summary>
  public Text Title;

  /// <summary>
  /// The GUI text element displaying the alert's message.
  /// </summary>
  public Text Message;

  /* Background materials */
  public Material NoEnergy;
  public Material Error;
  public Material LaserBad;
  public Material LaserGood;
  public Material Standard;

  /* Background material types */
  public static string TypeNoEnergy = "NoEnergy";
  public static string TypeError = "Error";
  public static string TypeLaserBad = "LaserBad";
  public static string TypeLaserGood = "LaserGood";
  public static string TypeStandard = "Standard";

  ///// LIFECYCLE /////

  /// <summary>
  /// MonoBehaviour load.
  /// </summary>
  void Start() {
    // Scale title & message
    ScaleText();

    // Fade in
    float fadeTime = 1.0f;
    StartCoroutine(FadeRawImage(Background, 0.0f, 1.0f, fadeTime));
    StartCoroutine(FadeText(Title, 0.0f, 1.0f, fadeTime));
    StartCoroutine(FadeText(Message, 0.0f, 1.0f, fadeTime));
  }

  /// <summary>
  /// Scales the text on the GUI.
  /// </summary>
  public void ScaleText() {
    Title.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, Screen.height);
    Message.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, Screen.height / 2);
  }

  /// <summary>
  /// Coroutine wrapper for FadeOutAfterSecondsI()
  /// </summary>
  /// <param name="seconds">Number of seconds over which to fade.</param>
  public void FadeOutAfterSeconds(int seconds) {
    StartCoroutine(FadeOutAfterSecondsI(seconds));
  }

  /// <summary>
  /// Fades out the Alert GUI.
  /// </summary>
  /// <param name="seconds">Number of seconds over which to fade.</param>
  /// <returns>Standard IEnumerator</returns>
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

  /// <summary>
  /// Fades a raw image on the GUI.
  /// </summary>
  /// <param name="rawImage">The raw image element to fade.</param>
  /// <param name="alphaFrom">The starting opacity (between 0 and 1).</param>
  /// <param name="alphaTo">The ending opacity (between 0 and 1).</param>
  /// <param name="time">Number of seconds over which to fade.</param>
  /// <returns>Standard IEnumerator.</returns>
  IEnumerator FadeRawImage(RawImage rawImage, float alphaFrom, float alphaTo, float time) {
    for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time) {
      Color newColor = rawImage.color;
      newColor.a = Mathf.Lerp(alphaFrom, alphaTo, t);
      rawImage.color = newColor;
      yield return new WaitForEndOfFrame();
    }
    yield return null;
  }

  /// <summary>
  /// Fades GUI text.
  /// </summary>
  /// <param name="text">The text element to fade.</param>
  /// <param name="alphaFrom">The starting opacity (between 0 and 1).</param>
  /// <param name="alphaTo">The ending opacity (between 0 and 1).</param>
  /// <param name="time">Number of seconds over which to fade.</param>
  /// <returns>Standard IEnumerator.</returns>
  IEnumerator FadeText(Text text, float alphaFrom, float alphaTo, float time) {
    for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time) {
      Color newColor = text.color;
      newColor.a = Mathf.Lerp(alphaFrom, alphaTo, t);
      text.color = newColor;
      yield return new WaitForEndOfFrame();
    }
  }

  ///// PROPERTY SETTERS /////

  /// <summary>
  /// Sets the background to specified material type.
  /// </summary>
  /// <param name="type">The new material type.</param>
  public void SetBackground(string type) {
    // Set background to specified material type
    Background.material = typeof(Alert).GetField(type).GetValue(this) as Material;
  }

  /// <summary>
  /// Sets the title for the alert.
  /// </summary>
  /// <param name="title">The new title for the alert.</param>
  public void SetTitle(string title) {
    // Get GUI text component of title gameobject, then set new title string to it.
    Title.text = title;
  }

  /// <summary>
  /// Sets the message for the alert.
  /// </summary>
  /// <param name="message">The new message for the alert.</param>
  public void SetMessage(string message) {
    // Get GUI text component of message gameobject, then set new message string to it.
    Message.text = message;
  }

}
