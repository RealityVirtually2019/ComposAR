// WiTag Unity Client
// Code by C. Dillon Martin Hall and Thomas Suarez
// Copyright 2018 WiTag Inc

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// Live countdown window object.
/// </summary>
public class CountdownScreen : MonoBehaviour {

  // GUI objects
  public Text Number;
  public Text Warning;

  // String objects
  string[] warnings = new string[] {
    "Always play in a safe area\nand watch for hazards!",
    "Don't WiTag and drive!",
    "Watch out for objects and\ncivilians while playing.",
    "WiTag is an outdoor game.\nPlay outside for better accuracy!"
  };

  ///// LIFECYCLE /////

  void Start() {
    // Scale number & warning for this screen
    ScaleText();

    // Show a random warning message
    ShowWarning();
  }

  private void ScaleText() {
    Number.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, Screen.height);
    Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, Screen.height / 2);
  }

  private void ShowWarning() {
    // Choose a random warning
    int index = new System.Random().Next(0, warnings.Length);
    String warning = warnings[index];

    // Set the warning text
    SetWarning(warning);
  }

  public void FadeOutAfterSeconds(int seconds) {
    StartCoroutine(FadeOutAfterSecondsI(seconds));
  }

  private IEnumerator FadeOutAfterSecondsI(int seconds) {
    // For each second until the countdown is finished...
    int s = seconds;
    while (s > 0) {
      // Animate the number text with the new value
      StartCoroutine(AnimateNumber(s.ToString()));

      // Wait 1 second before continuing
      yield return new WaitForSeconds(1);

      // Decrement value by 1
      s--;
    }

    // Animate start message
    StartCoroutine(AnimateNumber("START!"));
    yield return new WaitForSeconds(1);

    // Fade out
    yield return FadeText(Warning, 1.0f, 0.0f, 0.5f);

    // Destroy this gameobject
    Destroy(gameObject);
  }

  private IEnumerator FadeText(Text text, float alphaFrom, float alphaTo, float time) {
    for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time) {
      Color newColor = text.color;
      newColor.a = Mathf.SmoothStep(alphaFrom, alphaTo, t);
      text.color = newColor;
      yield return new WaitForEndOfFrame();
    }
  }

  private IEnumerator ScaleText(Text text, float scaleFrom, float scaleTo, float time) {
    for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time) {
      float newScale = Mathf.SmoothStep(scaleFrom, scaleTo, t);
      text.gameObject.transform.localScale = new Vector3(newScale, newScale, newScale);
      yield return new WaitForEndOfFrame();
    }
  }

  /* Animates countdown text (scale/opacity) */
  private IEnumerator AnimateNumber(String text) {
    StartCoroutine(ScaleText(Number, 2.5f, 0.5f, 1.0f)); // scale down (with animation)
    StartCoroutine(FadeText(Number, 0.0f, 0.75f, 0.2f)); // fade in
    Number.text = text; // set new text
    yield return new WaitForSeconds(0.6f); // delay before fadeout (stay on screen)
    StartCoroutine(FadeText(Number, 0.75f, 0.0f, 0.2f)); // fade out
  }

  ///// PROPERTY SETTERS /////

  public void SetNumber(string number) {
    // Get GUI text component of title gameobject, then set new title string to it.
    Number.text = number;
  }

  public void SetWarning(string warning) {
    // Get GUI text component of message gameobject, then set new message string to it.
    Warning.text = warning;
  }

}
