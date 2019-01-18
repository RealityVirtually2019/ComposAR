// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using UnityEngine;
using System;
using UnityEngine.Events;
using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// Shared object module for controlling authentication.
/// </summary>
public class TeleportalAuth : MonoBehaviour {

  /* Shared Object References */

  public static TeleportalAuth Shared;

  /* Runtime login variables */

  [HideInInspector]
  /// <summary>
  /// Whether the current Teleportal user
  /// has been successfully authenticated / logged in.
  /// </summary>
  public bool Authenticated = false;

  [HideInInspector]
  /// <summary>
  /// Username of logged in player.
  /// </summary>
  public string Username;

  [HideInInspector]
  /// <summary>
  /// The provider (Google, Phone, etc) of authentication used during this session.
  /// </summary>
  public AuthProvider LoginProvider;

  [HideInInspector]
  /// <summary>
  /// The login ID for authentication.
  /// For Simple Auth, this is a user-generated PIN.
  /// For Google Auth, this is an email address.
  /// </summary>
  public string LoginId = "-";

  [HideInInspector]
  /// <summary>
  /// The login token for authentication.
  /// For Simple Auth, this remains as "-".
  /// For Google Auth, this is a Google authentication token.
  /// </summary>
  public string LoginToken = "-";

  /// <summary>
  /// Teleportal Internal Use only.
  /// Whether the dummy account should be utilized
  /// for development. No login required.
  /// </summary>
  public bool UseDummyAccount = false;

  /// <summary>
  /// Called before Start().
  /// </summary>
  void Awake() {
    // Set static self reference
    TeleportalAuth.Shared = this;
  }

  /// <summary>
  /// Standard MonoBehaviour Start() function.
  /// </summary>
  void Start() {
    // EraseSavedLogin();
  }

  /// <summary>
  /// Authentication was successful.
  /// </summary>
  public void Success() {
    // Set authenticated
    Authenticated = true;

    // Save login (correct credentials)
    SaveLogin();

    // Hide Splash screen
    TeleportalUi.Shared.HideSplash();

    // Start GPS updates
    TeleportalGps.Shared.HandleGPS();

    // If switching servers due to a game_join_move or app_in_review command...
    if (TeleportalNet.Shared.SwitchingServers) {
      TeleportalNet.Shared.SwitchingServers = false; // reset flag
    }
  }

  /// <summary>
  /// Authentication failed.
  /// </summary>
  public void Fail() {
    // Try registration
    TeleportalAuth.Shared.PromptRegister();
  }

  /// <summary>
  /// Attempts to use a saved username and auth token, if it has previously been saved.
  /// </summary>
  public void AttemptSavedLogin() {
    // Sign into dummy account, if specified by developer (debugging ONLY)
    if (UseDummyAccount) {
      Username = "dev-" + Regex.Replace(SystemInfo.deviceModel, "[^A-Za-z0-9]", "").ToLower();
      LoginProvider = AuthProvider.Simple;
      LoginId = "help@witag.co";
      Login();
      return;
    }

    // If saved login exists, attempt it
    if (LoginSaved()) {
      if (LoginProvider == AuthProvider.Simple) {
        // Attempt to login server-side with existing credentials
        Login();
      }
      else if (LoginProvider == AuthProvider.Google) {
        // Attempt to use an existing Google login session
        Login();
      }
      else if (LoginProvider == AuthProvider.Digits) {
        // Attempt to login server-side with existing phone credentials
        Login();
      }
      else {
        // Let player login manually
        PromptLogin();
      }
    }

    else {
      PromptRegister();
    }
  }

  /// <summary>
  /// Saves the current authentication variables to persistent storage.
  /// </summary>
  public void SaveLogin() {
    PlayerPrefs.SetString("username", Username);
    PlayerPrefs.SetString("loginProvider", ((int) LoginProvider).ToString());
    PlayerPrefs.SetString("loginId", LoginId);
    PlayerPrefs.SetString("loginToken", LoginToken);
    PlayerPrefs.Save();
  }

  /// <summary>
  /// Erases previously saved authentication variables from persistent storage.
  /// </summary>
  public void EraseSavedLogin() {
    PlayerPrefs.DeleteKey("username");
    PlayerPrefs.DeleteKey("loginProvider");
    PlayerPrefs.DeleteKey("loginId");
    PlayerPrefs.DeleteKey("loginToken");
  }

  /// <summary>
  /// Returns whether authentication variables were previously saved to persistent storage.
  /// </summary>
  /// <returns>Whether authentication variables were previously saved to persistent storage.</returns>
  public bool LoginSaved() {
    // Set runtime variables to stored data (if it exists)
    if (PlayerPrefs.HasKey("username")) {
      Username = PlayerPrefs.GetString("username");
      LoginProvider = (AuthProvider) PlayerPrefs.GetInt("loginProvider");
      LoginId = PlayerPrefs.GetString("loginId");
      LoginToken = PlayerPrefs.GetString("loginToken");
      Debug.Log("SAVED LOGIN: " + Username + "/" + LoginProvider + "/" + LoginId + "/" + LoginToken);
      return true;
    }
    Debug.Log("SAVED LOGIN: N/A");
    return false;
  }

  /// <summary>
  /// Sends login results to the WiTag server.
  /// </summary>
  public void Login() {
    // Send to server
    TeleportalNet.Shared.Send(TeleportalCmd.AUTH_LOGIN, Username, ((int) LoginProvider).ToString(), LoginId, LoginToken);
  }

  /// <summary>
  /// Logs out of Google and Phone auth sessions
  /// </summary>
  public void Logout() {
    // TODO Logout of Google and Digits sessions

    // Erase saved credentials
    EraseSavedLogin();

    // Show exit alert (permanent)
    TeleportalUi.Shared.ShowAlert(Alert.TypeError, "Successfully Logged Out!", "Please restart Teleportal to login again.", -1);
  }

  /// <summary>
  /// Sends register results to the WiTag server.
  /// </summary>
  public void Register() {
    // Send to server
    TeleportalNet.Shared.Send(TeleportalCmd.AUTH_REGISTER, Username, ((int) LoginProvider).ToString(), LoginId);
  }

  /// <summary>
  /// Prompts the player to login with one of the authentication providers.
  /// </summary>
  public void PromptLogin() {
    // UI prompt
    if (LoginProvider == AuthProvider.Simple) {
      PromptSimpleUsername();
    } else {
      PromptGoogleUsername();
    }
  }

  /// <summary>
  /// Prompts the player to register their username.
  /// </summary>
  public void PromptRegister() {
    // UI prompt
    PromptLogin();
  }

  /// <summary>
  /// Prompts the user to enter a username
  /// </summary>
  private void PromptGoogleUsername() {
    // Set login provider
    LoginProvider = AuthProvider.Google;

    // Prompt player
    TeleportalUi.Shared.ShowTextField(false, "Username?", delegate { TeleportalAuth.Shared.PromptGoogleUsername_Callback(); });
  }

  /// <summary>
  /// Google Authentication callback.
  /// </summary>  
  private void PromptGoogleUsername_Callback() {
    // Set global auth username to the user-inputted text
    Username = TeleportalUi.Shared.MainTextField.text.ToLower();
    TeleportalUi.Shared.HideTextField(true);

    // Show auth
    TeleportalUi.Shared.ShowAuth();
  }

  /// <summary>
  /// Prompts the user to enter a username for a temporary account.
  /// </summary>
  private void PromptSimpleUsername() {
    // Set login provider
    LoginProvider = AuthProvider.Simple;

    // Prompt player
    TeleportalUi.Shared.ShowTextField(false, "Teleportal ID...", delegate { TeleportalAuth.Shared.PromptSimpleUsername_Callback(); });
  }

  /// <summary>
  /// Prompts the user to enter a PIN for their temporary account.
  /// </summary>
  private void PromptSimplePin() {
    // Prompt player
    TeleportalUi.Shared.ShowTextField(true, "PIN for " + Username, delegate { TeleportalAuth.Shared.PromptSimplePin_Callback(); });
  }

  /// <summary>
  /// Simple Authentication callback.
  /// </summary>
  private void PromptSimpleUsername_Callback() {
    // Set global auth username to the user-inputted text
    Username = TeleportalUi.Shared.MainTextField.text.ToLower();

    TeleportalUi.Shared.HideTextField(true);
    PromptSimplePin();
  }

  /// <summary>
  /// Simple Authentication callback.
  /// </summary>
  private void PromptSimplePin_Callback() {
    // Set global auth id to the user-inputted text
    LoginId = TeleportalUi.Shared.MainTextField.text.ToLower();

    // Log in
    Login();
  }

}
