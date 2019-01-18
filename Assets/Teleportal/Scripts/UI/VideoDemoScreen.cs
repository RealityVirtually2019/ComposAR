// WiTag Unity Client
// Code by C. Dillon Martin Hall and Thomas Suarez
// Copyright 2018 WiTag Inc

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Interactive video demo window object.
/// </summary>
public class VideoDemoScreen : MonoBehaviour {

  // Constants
  public double FPS = 60.0; // fps of onboarding video
  private List<double> HelperTimes = new List<double>(); // frame number points at which to update
  private List<bool> HelperTaps = new List<bool>(); // whether we should stop and wait for user input at each point
  private List<string> HelperTexts = new List<string>(); // texts to update at each point
  private List<string> HelperImages = new List<string>(); // images to update at each point

  // Variables
  private bool VideoStarted = false;
  private bool VideoDone = false;
  private bool Waiting = false;
  private int TimerCount = 0;
  private double CurrentTime = 0.0;
  private double TargetTime = 0.0;
  private bool WasCalledFromGame = false;

  // GUI objects
  private Color fadedIn;
  private Color fadedOut;
  public Text TitleView;
  public Text MessageView;
  public Text HelperTextView;
  public RawImage HelperImageView;
  public VideoPlayer VideoView;
  public VideoClip OnboardingVideoClip;

  ///// LIFECYCLE /////

  void Start() {
    // Define colors for UI
    fadedIn = new Color(1f, 1f, 1f, 1f); // opaque white
    fadedOut = new Color(1f, 1f, 1f, 0f); // transparent white

    // Create helpers, timed with video
    CreateHelpers();
  }

  void Update() {
    // Capture single taps on-screen
    if (Input.touchCount > 0) {
      if (Input.GetTouch(0).phase == TouchPhase.Began) {
        Tap();
      }
    }

    // editor debug: mouse button - left click
    if (Input.GetMouseButtonUp(0)) {
      Tap();
    }

    // If video is playing, add current time
    if (VideoView.isPlaying) {
      CurrentTime += Time.deltaTime;

      if (CurrentTime >= TargetTime) {
        // Wait for player interaction (with modified helper UI)
        WaitForTap(HelperTaps[TimerCount], HelperTexts[TimerCount], HelperImages[TimerCount]);
      }
    }
  }

  public void Play() {
    // Skip straight to video if was called from game (not first time setup)
    if (WasCalledFromGame) {
      VideoUI();
      return;
    }

    // Start with welcome UI
    WelcomeUI();
  }

  ///// VIEWS /////

  public void CalledFromGame() {
    // Not first-time setup, set flag
    WasCalledFromGame = true;
  }

  private void WelcomeUI() {
    // Set title/message
    TitleView.text = "Interactive Demo";
    MessageView.text = "In the next 60 seconds, you'll learn the basics of WiTag.\nLet's go!";
  }

  private void VideoUI() {
    // Start views fully faded out
    TitleView.color = fadedIn;
    MessageView.color = fadedIn;
    HelperTextView.color = fadedOut;
    HelperImageView.color = fadedOut;
    VideoView.targetCameraAlpha = 0f;

    // TODO fade in views
    TitleView.color = fadedOut;
    MessageView.color = fadedOut;
    HelperTextView.color = fadedIn;
    HelperImageView.color = fadedIn;
    VideoView.gameObject.SetActive(true);

    // Start video
    StartVideo();
  }

  private void AwesomeUI() {
    // Choose different UI if was called from game (not first-time setup)
    if (WasCalledFromGame) {
      BackToGameUI();
      return;
    }

    // TODO fade out views
    TitleView.color = fadedIn;
    MessageView.color = fadedIn;
    HelperTextView.color = fadedOut;
    HelperImageView.color = fadedOut;
    VideoView.gameObject.SetActive(false);

    // Set title/message
    TitleView.text = "Awesome!";
    MessageView.text = "You're now ready to play WiTag anywhere!\nRegister your player to finish training.";
  }

  private void BackToGameUI() {
    // TODO fade out views
    TitleView.color = fadedIn;
    MessageView.color = fadedIn;
    HelperTextView.color = fadedOut;
    HelperImageView.color = fadedOut;
    VideoView.gameObject.SetActive(false);

    // Set title/message
    TitleView.text = "Awesome!";
    MessageView.text = "You're now ready to play WiTag anywhere!\nTap below to return to the game.";
  }

  ///// INPUT /////

  public void Skip() {
    if (!VideoDone) {
      // TODO report analytical event
    }

    SkipNow();
  }

  public void Ready() {
    if (!VideoDone) {
      VideoUI();
    } else {
      SkipNow();
    }
  }

  private void WaitForTap(bool required, string newHelperText, string newImageName) {
    // Modify UI views
    HelperTextView.text = newHelperText;

    // Update timer count (for next timer)
    TimerCount++;

    if (required) {
      Waiting = true;

      // Pause video
      VideoView.Pause();
    } else {
      // Start next video timer (for helper)
      NextTimer();
    }
  }

  public void Tap() {
    // If we were already waiting for the player's response...
    if (Waiting) {
      Waiting = false; // not waiting anymore
      VideoView.Play(); // resume video
      NextTimer(); // start next video timer (for helper)
    }

    // If the user tapped before the video started or after the video ended
    else if ((VideoStarted && VideoDone) || (!VideoStarted && !VideoDone)) {
      Ready();
    }
  }

  ///// TIMING /////

  private void NextTimer() {
    // Finish if there are no more helpers
    if (TimerCount >= HelperTimes.Count) {
      VideoDone = true; // video is now done
      AwesomeUI(); // give UI back to player
      TeleportalUi.Shared.PlayMusic(TeleportalUi.MusicType.Lobby); // replace normal music
    }

    // Set target for timer
    TargetTime = HelperTimes[TimerCount] / FPS;
  }

  ///// VIDEO HANDLING /////

  private void StartVideo() {
    VideoStarted = true;

    // Play onboarding video
    VideoView.clip = OnboardingVideoClip;
    VideoView.Play();

    // Stop background audio
    TeleportalUi.Shared.PlayMusic(TeleportalUi.MusicType.None);
  }

  private void SkipNow() {
    // Stop video
    VideoView.Stop();

    // Resume music
    TeleportalUi.Shared.PlayMusic(TeleportalUi.MusicType.Lobby);

    // Choose different UI if was called from game (not first-time setup)
    if (WasCalledFromGame) {
      BackToGameUI();
      return;
    }

    // Show player name field
    // TODO TeleportalUi.Shared.OnboardPlayerName();
  }

  ///// VIDEO HELPERS /////

  private void CreateHelpers() {
    // START
    HelperTimes.Add(0);
    HelperTaps.Add(false);
    HelperTexts.Add("");
    HelperImages.Add("");

    HelperTimes.Add(215);
    HelperTaps.Add(true);
    HelperTexts.Add("Aim at the enemy.\nTap anywhere to fire.");
    HelperImages.Add("tap");

    HelperTimes.Add(325);
    HelperTaps.Add(true);
    HelperTexts.Add("Another enemy fired at you!\nTap anywhere to fire back.");
    HelperImages.Add("tap");

    HelperTimes.Add(397);
    HelperTaps.Add(true);
    HelperTexts.Add("Your energy depletes over time.\nFind more energy nearby.");
    HelperImages.Add("energy");

    HelperTimes.Add(550);
    HelperTaps.Add(false);
    HelperTexts.Add("");
    HelperImages.Add("none");

    HelperTimes.Add(778);
    HelperTaps.Add(false);
    HelperTexts.Add("Great job! Now move to\nfind more players.");
    HelperImages.Add("move");

    HelperTimes.Add(1022);
    HelperTaps.Add(false);
    HelperTexts.Add("Hide where possible.");
    HelperImages.Add("hide");

    HelperTimes.Add(1284);
    HelperTaps.Add(true);
    HelperTexts.Add("The reticle shows\nwhen you can fire.");
    HelperImages.Add("reticle");

    HelperTimes.Add(1555);
    HelperTaps.Add(false);
    HelperTexts.Add("");
    HelperImages.Add("none");

    // UNCOMMENT THESE FOR LOGO ENDING:

    /*HelperTimes.Add(1990);
    HelperTaps.Add(false);
    HelperTexts.Add("Congratulations!\nYou are ready");
    HelperImages.Add("fiesta");*/

    // END
    HelperTimes.Add(1990 /*2290*/);
    HelperTaps.Add(false);
    HelperTexts.Add("");
    HelperImages.Add("");
  }

}
