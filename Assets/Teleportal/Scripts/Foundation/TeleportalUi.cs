// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Shared module for controlling the User Interface (UI).
/// </summary>
public class TeleportalUi : MonoBehaviour {

  /* Shared Object References */

  public static TeleportalUi Shared;

  /* Prefab references */
  public Alert _Alert;
  public BetaScreen _BetaScreen;
  public VideoDemoScreen _VideoDemoScreen;
  public SplashScreen _SplashScreen;
  public CountdownScreen _CountdownScreen;

  // GameObject references
  public Canvas SharedCanvas;
  private BetaScreen BetaScreen;
  private SplashScreen SplashScreen;
  private CountdownScreen CountdownScreen;
  // public EnergyBarController EnergyBarController;
  public ReactiveReticle ReactiveReticle;
  public InputField MainTextField;
  private VideoDemoScreen VideoDemoScreen;
  private Alert alert;

  /* Object references */

  public Action CurrentPrompt;

  /* Enumerated types */

  /// <summary>
  /// Types of errors that can be shown.
  /// </summary>
  public enum ErrorType {
    Old, Login, GpsBad, Disconnect, Unexpected
  };

  /// <summary>
  /// Types of music that can be played.
  /// </summary>
  public enum MusicType {
    Lobby, Battle, None
  };

  /// <summary>
  /// Types of sounds that can be played.
  /// </summary>
  public enum SoundType {
    Select, Laser, Charge, Teleport, Login
  };

  /* Sound Objects */
  public AudioClip SoundSelect;
  public AudioClip SoundLaser;
  public AudioClip SoundCharge;
  public AudioClip SoundTeleport;
  public AudioClip SoundLogin;

  /* Audio Objects */
  public AudioSource AudioSourceLobby;
  public AudioSource AudioSourceBattle;
  public AudioSource AudioSourceSound;

  /* Variables */

  /// <summary>
  /// Whether there is a player directive UI currently enabled.
  /// </summary>
  public bool DirectiveEnabled = false;

  /// <summary>
  /// The threshold (in pixels) for swipe gestures on the screen.
  /// </summary>
  public float TouchMoveThreshold = 100f; // px

  /// <summary>
  /// The current position of the first touch on the screen.
  /// </summary>
  private Vector2 touchPos;

  ///// LIFECYCLE /////

  /// <summary>
  /// Called before Start().
  /// </summary>
  void Awake() {
    // Set static self reference
    TeleportalUi.Shared = this;

    // Find the necessary scene objects to control
    FindObjects();
  }

  /// <summary>
  /// Links Teleportal-specific UI GameObjects.
  /// </summary>
  private void FindObjects() {
    // These objects need to be present in the scene
    // by runtime, with the exact names in quotes
    SharedCanvas = GameObject.Find("Canvas UI").GetComponent<Canvas>();
    // EnergyBarController = GameObject.Find("Energy Bar").GetComponent<EnergyBarController>();
    ReactiveReticle = GameObject.Find("Reactive Reticle").GetComponent<ReactiveReticle>();
    MainTextField = GameObject.Find("Main Text Field").GetComponent<InputField>();
    AudioSourceLobby = GameObject.Find("Audio Source Lobby").GetComponent<AudioSource>();
    AudioSourceBattle = GameObject.Find("Audio Source Battle").GetComponent<AudioSource>();
    AudioSourceSound = GameObject.Find("Audio Source Sound").GetComponent<AudioSource>();
  }

  /// <summary>
  /// Updated on every rendered frame.
  /// </summary>
  void Update() {
    if (Input.GetMouseButtonDown(0)) {
      // If the main text field object is active, focus it.
      if (MainTextField.gameObject.active) {
        MainTextField.ActivateInputField();
      }

      // Otherwise, tap the menu raycaster.
      else {
        XRItemRaycaster.Shared.Tap();
      }
    }

    // Right click
    else if (Input.GetMouseButtonDown(1)) {
      // If the main text field object is active, focus it.
      if (MainTextField.gameObject.active) {
        MainTextField.ActivateInputField();
      }

      // Otherwise, tap the menu raycaster.
      else {
        XRItemRaycaster.Shared.TapAlt();
      }
    }

    // If the user is authenticated already...
    if (TeleportalAuth.Shared.Authenticated) {
        // Only continue if there is no input in progress
        if (!MainTextField.gameObject.active) {
            // Open livemap when user presses the "M" key.
            if (Input.GetKeyDown(KeyCode.M)) {
                ShowLivemap();
            }

            // Prompt to visit Realm when user presses the "R" key.
            else if (Input.GetKeyDown(KeyCode.R)) {
                PromptRealmId();
            }
        }
    }
  }

  /// <summary>
  /// Standard MonoBehaviour Start() function.
  /// </summary>
  void Start() {
    InstantiatePrefabs();
  }

  /// <summary>
  /// Instantiates new Game Objects from their prefabs.
  /// </summary>
  void InstantiatePrefabs() {
    // add here, as necessary.
    
    // Hide Reactive Reticle & Text Field to start with
    ReactiveReticle.Hide();
    HideTextField(true);
  }

  /// <summary>
  /// Parents the specified Game Object to the canvas.
  /// </summary>
  /// <param name="go">The Game Object to parent.</param>
  private void ParentToCanvas(GameObject go) {
    // Parent transform
    go.transform.position = SharedCanvas.transform.position;
    go.transform.rotation = SharedCanvas.transform.rotation;
    go.transform.SetParent(SharedCanvas.transform);
    go.transform.SetAsFirstSibling();

    // Reset the automatically-set properties
    RectTransform rt = go.GetComponent<RectTransform>();
    rt.anchoredPosition3D = new Vector3(0, 0, -10);
    rt.localScale = new Vector3(1, 1, 1);
    rt.anchorMin = new Vector2(0, 0);
    rt.anchorMax = new Vector2(1, 1);
    rt.offsetMin = new Vector2(0, 0);
    rt.offsetMax = new Vector2(1, 1);
    rt.pivot = new Vector2(0.5f, 0.5f);
  }

  ///// ALERTS /////

  /// <summary>
  /// Shows an alert with the specified parameters.
  /// </summary>
  /// <param name="background">The name of the desired background color material.</param>
  /// <param name="title">The desired title of the new alert.</param>
  /// <param name="message">The desired message of the new alert.</param>
  /// <param name="seconds">The desired number of seconds for which to show the alert.</param>
  /// <returns></returns>
  public Alert ShowAlert(string background, string title, string message, int seconds) {
    HideAlert();

    // Instantiate a new Alert from its Prefab
    alert = Instantiate(_Alert);

    // Set alert properties
    alert.SetBackground(background);
    alert.SetTitle(title);
    alert.SetMessage(message);

    // Parent this Alert to the existing Canvas
    ParentToCanvas(alert.gameObject);

    // Fade out after specified number of seconds (if wanted)
    if (seconds > -1) {
      alert.FadeOutAfterSeconds(seconds);
    }

    return alert;
  }

  /// <summary>
  /// Hides the current Alert, if there is one.
  /// </summary>
  public void HideAlert() {
    if (alert != null) {
      if (alert.gameObject != null) {
        Destroy(alert.gameObject);
      }
    }
  }

  /// <summary>
  /// Shows an error with the specified error type.
  /// </summary>
  /// <param name="type">The type of error type to show.</param>
  public void ShowError(ErrorType type) {
    // Create message based on error type
    string message = "";
    switch (type) {
      case ErrorType.Old:
        message = "This version of Teleportal is too old.\nPlease update it.";
        break;
      case ErrorType.Login:
        message = "We couldn't log you in.\nPlease try again soon.";
        break;
      case ErrorType.GpsBad:
        message = "Location Services (GPS) is not working.\nPlease turn it on in Settings.";
        break;
      case ErrorType.Disconnect:
        message = "Lost connection. Please turn off\nWiFi and try again.";
        break;
      case ErrorType.Unexpected:
        message = "Something strange happened.\nPlease reopen Teleportal.";
        break;
    }

    // Show permanent Alert
    ShowAlert(Alert.TypeError, "Oops!", message, -1);
  }

  ///// PROMPTS /////

  /// <summary>
  /// Asks the user a question.
  /// </summary>
  /// <param name="question">The question to ask.</param>
  /// <param name="desc">A more detailed description of the question.</param>
  /// <param name="yesTitle">The title of the "yes" option.</param>
  /// <param name="yesFunc">A callback function for when the "yes" option is tapped.</param>
  /// <param name="noTitle">The title of the "no" option.</param>
  /// <param name="noFunc">A callback function for when the "no" option is tapped.</param>
  public void Ask(string question, string desc, string yesTitle, Func<int> yesFunc, string noTitle, Func<int> noFunc) {
    // TODO question prompts
    yesFunc(); // TODO remove this line
  }

  /// <summary>
  /// Prompts the player and waits for acknowledgement from the player.
  /// </summary>
  /// <param name="prompt">The desired title of the prompt to dsiplay.</param>
  /// <param name="desc">A detailed description of the prompt.</param>
  /// <param name="buttonTitle">The desired title of the acknowledgement button.</param>
  /// <param name="buttonFunc">A callback function for when the acknowledgement button is tapped.</param>
  public void WaitForAck(string prompt, string desc, string buttonTitle, Action buttonFunc) {
    // Show semi-permanent Alert (prompt)
    ShowAlert(Alert.TypeStandard, prompt, desc, -1);

    // Set the global current prompt function (see XRItemRaycaster.cs:Tap() for its invocation)
    CurrentPrompt = buttonFunc;
  }

  ///// MOMENTARY ALERTS /////

  /// <summary>
  /// Creates a momentary message on the screen.
  /// </summary>
  /// <param name="message">The message to display.</param>
  public void Toast(string message) {
    // TODO toast to screen
  }

  /// <summary>
  /// Starts a directive for the player
  /// that can be canceled in EndDirective().
  /// </summary>
  /// <param name="message">The message for the directive.</param>
  public void StartDirective(string message) {
    // Skip duplicates
    if (DirectiveEnabled) { return; }

    // Enable directive
    DirectiveEnabled = true;

    // TODO Set text in UI
    // for now, show a standard alert
    ShowAlert(Alert.TypeStandard, "!!", message, 3);

    // Start pulsing (repeating)
    StartCoroutine(PulseDirective());
  }

  /// <summary>
  /// Pulses (fades in/out) the directive message.
  /// </summary>
  /// <returns></returns>
  IEnumerator PulseDirective() {
    // While the directive is enabled...
    while (DirectiveEnabled) {
      // TODO Fade in over 0.5 second

      // TODO Fade out over 0.5 second (with 0.5 second delay, to account for the above animation)

      // Disable directive (when finished)
      DirectiveEnabled = false;

      // Yield to other processes
      yield return null;
    }
  }

  /// <summary>
  /// Cancels the directive shown to the player.
  /// </summary>
  public void EndDirective() {
    // Disable directive
    DirectiveEnabled = false;

    // TODO Reset text in UI
  }

  ///// REALMS /////
  
  /// <summary>
  /// Prompts the user to enter a Realm ID to which to jump.
  /// </summary>
  public void PromptRealmId() {
    ShowTextField(true, "Realm ID...", delegate { TeleportalUi.Shared.PromptRealmId_Callback(); });
  }

  /// <summary>
  /// Jumps to the Realm given by PromptRealmId().
  /// </summary>
  public void PromptRealmId_Callback() {
    // Jump to realm
    string realmId = TeleportalUi.Shared.MainTextField.text.ToLower();
    TeleportalNet.Shared.Send(TeleportalCmd.REALM_JOIN, realmId);
  }

  ///// ENERGY BAR /////

  /// <summary>
  /// Depletes the GUI energy bar to zero (animated).
  /// </summary>
  public void DepleteEnergyBar() {
    // Animate GUI energy bar
    // EnergyBarController.SetEnergy(0f);
  }

  /// <summary>
  /// Fills the GUI energy bar to maximum (animated).
  /// </summary>
  public void FillEnergyBar() {
    // Animate GUI energy bar
    // EnergyBarController.SetEnergy(100f/*LaserTag.Shared.MaxEnergy*/);
  }

  /// <summary>
  /// Animates the GUI energy bar to a specified value.
  /// </summary>
  /// <param name="c">The value to which to animate the GUI energy bar.</param>
  public void AnimateEnergyBar(float c) {
    // Animate GUI energy bar to the specified value
    // EnergyBarController.SetEnergy(c);
  }

  ///// FULL-SCREEN EFFECTS /////

  public void LightspeedToEndor(bool activate) {
    // TODO new teleporting animation video
    return;

    if (activate) {
      // Tell the shared lightspeed effect view
      // to start playing now
      LightspeedEffect.Shared.Play();
    }
    else {
      // Or stop
      LightspeedEffect.Shared.Stop();
    }
  }

  ///// REACTIVE RETICLE /////

  /// <summary>
  /// Unlocks the Reticle, so the image can be changed again.
  /// </summary>
  public void UnlockReticle() {
    SetReticleLockState(false);
  }

  /// <summary>
  /// Locks the Reticle, so the image cannot be changed, until unlocked.
  /// </summary>
  public void LockReticle() {
    SetReticleLockState(true);
  }

  /// <summary>
  /// Sets the lock state of the Reticle.
  /// Also see UnlockReticle() and LockReticle().
  /// </summary>
  /// <param name="lockState">Whether the Reticle should be locked.</param>
  private void SetReticleLockState(bool lockState) {
    XRItemRaycaster.Shared.ShouldRaycast = !lockState;
  }

  /// <summary>
  /// Sets the reactive reticle type to a new one.
  /// </summary>
  /// <param name="type">The new reactive reticle type.</param>
  public void SetReticle(ReactiveReticle.ReticleType type) {
    // Set new reactive reticle type
    ReactiveReticle.SetType(type);
  }

  /// <summary>
  /// Sets the reactive reticle type to a new one, then sets it back after a delay.
  /// </summary>
  /// <param name="type">The new reactive reticle type.</param>
  /// <param name="delay">Time to wait (in seconds) after setting the new reticle type.
  public void SetReticle(ReactiveReticle.ReticleType type, float delay) {
    // Call delaying coroutine
    StartCoroutine(SetReticleDelayed(type, delay));
  }

  /// <summary>
  /// Sets the reactive reticle type to a new one, then sets it back after a delay.
  /// </summary>
  /// <param name="type">The new reactive reticle type.</param>
  /// <param name="delay">Time to wait (in seconds) after setting the new reticle type.
  private IEnumerator SetReticleDelayed(ReactiveReticle.ReticleType type, float delay) {
    // Set new reactive reticle type
    SetReticle(type);

    // Wait for delay to pass first
    yield return new WaitForSeconds(delay);

    // Set the type back
    SetReticle(ReactiveReticle.ReticleType.NotReady);

    // Finish
    yield return null;
  }

  ///// USER INPUT /////

  /// <summary>
  /// Shows and focuses the main text field, with a prompt for the user.
  /// </summary>
  /// <param name="prompt">The prompt to display.</param>
  public void ShowTextField(bool autoHide, string prompt, Action func) {
    // Activate gameobject
    // TODO: animate fade
    MainTextField.gameObject.active = true;

    // Set new placeholder value
    Text placeholderText = ((Text) MainTextField.placeholder);
    placeholderText.text = prompt;

    // Set submit listener functions
    MainTextField.onEndEdit.RemoveAllListeners();
    MainTextField.onEndEdit.AddListener( delegate {
      // Skip if there is a button held down that should not be...
      if (Input.touchCount > 0 || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape)) {
        return;
      }

      // Skip if there was no input
      if (MainTextField.text == "") {
        return;
      }

      // Execute the passed listener function
      func();

      // Hide the text field (HideTextField()) and reset it (true)
      if (autoHide) {
        TeleportalUi.Shared.HideTextField(true);
      }
    } );

    // Activate and focus the UI cursor inside the text field
    MainTextField.ActivateInputField();

    // Hide Reactive Reticle
    ReactiveReticle.Hide();

    // Global signal
    if (TeleportalActions.Shared.OnWorldHide != null) {
      TeleportalActions.Shared.OnWorldHide();
    }
  }

  /// <summary>
  /// Hides the main text field from the screen.
  /// </summary>
  /// <param name="clear">Whether to clear the current value and placeholder text.</param>
  /// <returns>The current user-inputted textfield value.</returns>
  public string HideTextField(bool clear) {
    // Store the current value of the text field
    string text = MainTextField.text;

    // Deactivate gameobject
    // TODO: animate fade
    MainTextField.gameObject.active = false;

    // Clear both the text and placeholder values (if requested)
    if (clear) {
      MainTextField.text = "";
      Text placeholderText = ((Text) MainTextField.placeholder);
      placeholderText.text = "";
    }

    // Show Reactive Reticle
    TeleportalUi.Shared.ReactiveReticle.Show();

    // Global signal
    if (TeleportalActions.Shared.OnWorldShow != null) {
      TeleportalActions.Shared.OnWorldShow();
    }

    // Return stored text to calling function
    return text;
  }

  ///// SCREENS /////

  /// <summary>
  /// Shows the splash screen.
  /// </summary>
  public void ShowSplash() {
    // Instantiate a new SplashScreen from its Prefab
    SplashScreen = Instantiate(_SplashScreen);

    // Parent this SplashScreen to the existing Canvas
    ParentToCanvas(SplashScreen.gameObject);
  }

  /// <summary>
  /// Hides the splash screen, if it has already been displayed.
  /// </summary>
  public void HideSplash() {
    // If Splash screen gameobject still exists...
    if (SplashScreen != null) {
      // Hide SplashScreen screen
      SplashScreen.FadeOutAfterSeconds(0);
    }
  }

  /// <summary>
  /// Shows the beta screen.
  /// </summary>
  public void ShowBeta() {
    // Instantiate a new BetaScreen from its Prefab
    BetaScreen = Instantiate(_BetaScreen);

    // Set title & message
    BetaScreen.SetTitle("Welcome to WiTag\n" + TeleportalAuth.Shared.Username + "!");
    BetaScreen.SetMessage("You are #" + TeleportalBeta.Shared.BetaWaitAhead.ToString() + " in the WiTag line.\nWe are staging our rollout to ensure quality.\nFor now, you can share WiTag with your friends\nso you all join at once! See you soon! :)");

    // Parent this BetaScreen to the existing Canvas
    ParentToCanvas(BetaScreen.gameObject);
  }

  /// <summary>
  /// Hides the beta screen.
  /// </summary>
  public void HideBeta() {
    // Hide beta screen
    BetaScreen.FadeOutAfterSeconds(0);
  }

  /// <summary>
  /// Plays the video tutorial (during first run).
  /// </summary>
  public void ShowTutorialPre() {
    // Instantiate a new VideoDemoScreen from its Prefab
    VideoDemoScreen = Instantiate(_VideoDemoScreen);

    // Parent this VideoDemoScreen to the existing Canvas
    ParentToCanvas(VideoDemoScreen.gameObject);

    // Play
    VideoDemoScreen.Play();
  }

  /// <summary>
  /// Plays the video tutorial (for an existing player)
  /// </summary>
  public void ShowTutorialPost() {
    // Instantiate a new VideoDemoScreen from its Prefab
    VideoDemoScreen = Instantiate(_VideoDemoScreen);

    // Parent this VideoDemoScreen to the existing Canvas
    ParentToCanvas(VideoDemoScreen.gameObject);

    // Play with game flag
    VideoDemoScreen.CalledFromGame();
    VideoDemoScreen.Play();
  }

  /// <summary>
  /// Shows a countdown screen.
  /// </summary>
  public void ShowCountdown() {
    // Instantiate a new CountdownScreen from its Prefab
    CountdownScreen = Instantiate(_CountdownScreen);

    // Hide countdown screen
    CountdownScreen.FadeOutAfterSeconds(10);

    // Parent this CountdownScreen to the existing Canvas
    ParentToCanvas(CountdownScreen.gameObject);
  }

  public void ShowLivemap() {
    string format = "https://map.teleportal.app/?ip={0}&port={1}&username={2}";
		string url = string.Format(format, TeleportalNet.Shared.HostWs, TeleportalNet.Shared.PortWs, TeleportalAuth.Shared.Username);
		this.OpenURL("Teleportal Map", url);
  }

  public void ShowAuth() {
    string format = "https://static.teleportal.app/auth/?uuid={0}&ip={1}";
    string url = string.Format(format, TeleportalNet.Shared.SocketUuid, TeleportalNet.Shared.HostWs);
    this.OpenURL("Teleportal Login", url);
  }	

  /// <summary>
  /// Opens a URL in the in-app web browser.
  /// </summary>
  /// <param name="title">The title of the webpage, to display in a bar above the page.</param>
  /// <param name="url">The URL of the webpage.</param>
  public void OpenURL(string title, string url) {
    // iOS
    if (TeleportalPlatformInfo.IsMobileApple) {
      SFSafariView.LaunchUrl(url);
    }

  /*
    // Android
    else if (TeleportalPlatformInfo.IsMobileGoogle) {
      BrowserOpener.Shared.OpenURL(title, url); // TODO get redist license
    }
  */

    // Desktop and VR
    else {
      Application.OpenURL(url);
    }
  }

  /// <summary>
  /// Shows the credits screen.
  /// </summary>
  public void ShowCredits() {
    /* TODO show credits

    // Instantiate a new CreditsScreen from its Prefab
    CreditsScreen = Instantiate(_CreditsScreen);

    // Parent this CreditsScreen to the existing Canvas
    ParentToCanvas(CreditsScreen.gameObject); */
  }

  ///// BUTTON INPUT /////

  public void DetermineTouches() {
    // If there is 1 touch on the screen
    if (Input.touchCount == 1) {
      // Get the touch object
      Touch t = Input.touches[0];

      // When the touch begins
      if (t.phase == TouchPhase.Began) {
        // Save its start position.
        // Used for the delta calculation below.
        this.touchPos = t.position;
      }

      // When the touch ends
      else if (t.phase == TouchPhase.Ended) {
        // Calculate its delta position from start.
        Vector2 deltaPos = t.position - this.touchPos;

        // X movement
        if (Math.Abs(deltaPos.y) < TouchMoveThreshold) {
          // Swipe Right
          if (deltaPos.x > TouchMoveThreshold) {
            // Toggle virtual world
            // TP // TeleportalAr.Shared.ToggleVirtualWorld();
          }
          // Swipe Left
          else if (Input.touchCount == 2 && deltaPos.x < -TouchMoveThreshold) {
            // Reposition item (TODO move this to a "tap and hold" gesture)
            XRItemRaycaster.Shared.TapAlt();
          }
        }
        
        // Y movement
        else if (Math.Abs(deltaPos.x) < TouchMoveThreshold){
          // Swipe Up
          if (deltaPos.y > TouchMoveThreshold) {
            // Show livemap
            ShowLivemap();
          }
          // Swipe Down
          else if (deltaPos.y < -TouchMoveThreshold) {
            // If there is a player targeted in the reticle
            if (XRItemRaycaster.Shared.PlayerFocus != null) {
              // Sync with that player
              TeleportalGps.Shared.HighFivePrompt(XRItemRaycaster.Shared.PlayerFocus);
            }
          }
        }
      }
    }

    /* TODO REMOVE old 2-tap code
    // Toggle the virtual world, if the player taps with 2 fingers
    if (Input.touchCount == 2) {
      if (Input.touches[0].phase == TouchPhase.Ended) {
        TeleportalAr.Shared.ToggleVirtualWorld();
      }
    }
    else if (Input.touchCount == 3) {
      if (Input.touches[0].phase == TouchPhase.Ended) {
        ShowLivemap();
      }
    }
    */
  }

  ///// AUDIO /////

  /// <summary>
  /// Plays the specified type of music.
  /// </summary>
  /// <param name="type">The type of music to play.</param>
  public void PlayMusic(MusicType type) {
    switch (type) {
      case MusicType.Lobby:
        AudioSourceLobby.Play();
        AudioSourceBattle.Stop();
        break;
      case MusicType.Battle:
        AudioSourceBattle.Play();
        AudioSourceLobby.Stop();
        break;
      case MusicType.None:
        AudioSourceBattle.Stop();
        AudioSourceLobby.Stop();
        break;
    }
  }

  /// <summary>
  /// Plays the specified type of sound.
  /// </summary>
  /// <param name="type">The type of sound to play.</param>
  public void PlaySound(SoundType type) {
    switch (type) {
      case SoundType.Select:
        AudioSourceSound.clip = SoundSelect;
        break;
      case SoundType.Laser:
        AudioSourceSound.clip = SoundLaser;
        break;
      case SoundType.Charge:
        AudioSourceSound.clip = SoundCharge;
        break;
      case SoundType.Teleport:
        AudioSourceSound.clip = SoundTeleport;
        break;
      case SoundType.Login:
        AudioSourceSound.clip = SoundLogin;
        break;
    }
    AudioSourceSound.Play();
  }

  /// <summary>
  /// Vibrate with the "hit" preset.
  /// </summary>
  public void VibrateHit() {
    #if UNITY_IOS || UNITY_ANDROID
      // Vibrate device
      Handheld.Vibrate();
    #endif
  }

  /// <summary>
  /// Vibrate with the "fire" preset.
  /// </summary>
  public void VibrateFire() {
    #if UNITY_IOS || UNITY_ANDROID
      // Vibrate device
      Handheld.Vibrate();
    #endif
  }

}
