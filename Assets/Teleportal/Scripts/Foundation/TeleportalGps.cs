// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Shared module for controlling geolocation.
/// </summary>
public class TeleportalGps : MonoBehaviour {

  /* Shared Object References */

  public static TeleportalGps Shared;

  /* Editor Variables */

  public float UpdateFrequency = 0.25f;

  /* Runtime properties (latitude/longitude) */

  /// <summary>
  /// Player's origin latitude.
  /// </summary>
  public double Latitude = 0.0;

  [HideInInspector]
  /// <summary>
  /// Player's origin Y position.
  /// </summary>
  public double PosY = 0.0;

  /// <summary>
  /// Player's origin longitude.
  /// </summary>
  public double Longitude = 0.0;

  /* Runtime properties (other) */

  [HideInInspector]
  /// <summary>
  /// The current Player's heading rotation.
  /// </summary>
  public float Heading = 0.0f;

  [HideInInspector]
  /// <summary>
  /// The current Player's pitch rotation.
  /// </summary>
  public float Pitch = 0.0f;

  [HideInInspector]
  /// <summary>
  /// Whether the GPS or origin location has been
  /// locked already for this player.
  /// </summary>
  public bool LocationLocked = false;

  [HideInInspector]
  /// <summary>
  /// The number of location updates that have
  /// occurred since connecting to Teleportal.
  /// </summary>
  public int UpdateCount = 0;

  [HideInInspector]
  /// <summary>
  /// The location update token for shortened UDP location communication.
  /// </summary>
  public string UpdateToken = "-";

  [HideInInspector]
  /// <summary>
  /// The temporary headings from this Player to the triangulation points.
  /// </summary>
  public float[] PointHeadings = new float[3];

  /// <summary>
  /// Called before Start().
  /// </summary>
  void Awake() {
    // Set static self reference
    TeleportalGps.Shared = this;
  }

  /// <summary>
  /// Simulates a real-world origin location
  /// on devices without actual GPS/AGPS systems.
  /// </summary>
  /// <returns></returns>
  private IEnumerator HandleGPS_Desktop() {
    // Simulate real-world origin location
    ReportOriginLoc(Latitude, PosY, Longitude); // set this in the gameobject Inspector

    yield return null;
  }

  /// <summary>
  /// Waits for the location to lock, then continues
  /// to send this player's location while connected
  /// to Teleportal.
  /// </summary>
  /// <returns></returns>
  private IEnumerator HandleGPS_PlayerRepeat() {
    // Wait for the location to lock first
    while (!LocationLocked) {
      yield return new WaitForSeconds(0.5f);
    }

    // Global signal
    if (TeleportalActions.Shared.OnLocationLock != null) {
      TeleportalActions.Shared.OnLocationLock();
    }

    while (true) {
      // Continue sending location, based on player's world position
      Transform t = TeleportalAr.Shared.CurrentCamera.transform;

      // Create a vector containing the current local, unmodified position.
      // This is indirectly retrieved from ARKit/ARCore (via Unity camera transform).
      Vector3 scenePosition = new Vector3( (float) t.position.x, 0f, (float) t.position.z);

      // Get the current heading angle of this player
      Heading = t.eulerAngles.y;
      Pitch = t.eulerAngles.x;

      // Report location to server
      ReportDeltaLoc(scenePosition);

      // Global signal
      if (TeleportalActions.Shared.OnLocationUpdate != null) {
        TeleportalActions.Shared.OnLocationUpdate();
      }

      // Wait another x seconds before updating
      yield return new WaitForSeconds(UpdateFrequency);
    }

    yield return null;
  }

  /// <summary>
  /// Mobile devices only.
  /// Obtains permissions for, starts, and begins updating
  /// the location from the GPS/AGPS system.
  /// </summary>
  /// <returns>Asynchronous IEnumerator.</returns>
  private IEnumerator HandleGPS_Mobile() {
    // If location services is disabled by user (device or privacy settings)
    if (!Input.location.isEnabledByUser) {
      TeleportalUi.Shared.ShowError(TeleportalUi.ErrorType.GpsBad);
      yield break;
    }

    // Start service before querying location
    float desiredAccuracyInMeters = 1f; // 1m desired accuracy
    float updateDistanceInMeters = 1f; // 1m update distance
    Input.location.Start(); // start tracking location
    Input.compass.enabled = true; // enable compass

    // If location service connection failed
    if (Input.location.status == LocationServiceStatus.Failed) {
      TeleportalUi.Shared.ShowError(TeleportalUi.ErrorType.GpsBad);
      yield break;
    }

    // Wait until location locks
    while (Input.location.lastData.latitude == 0 || Input.location.lastData.longitude == 0) {
      yield return new WaitForSeconds(0.5f);
    }

    // By now, access is granted & GPS is ready.

    // Get latest location values
    LocationLocked = true;
    Latitude = Input.location.lastData.latitude;
    PosY = 0.0;
    Longitude = Input.location.lastData.longitude;
    Heading = Input.compass.trueHeading;

    // Set real-world origin location
    ReportOriginLoc(Latitude, PosY, Longitude);

    // If heading is negative, spin 'round the clock (360 degrees)
    if (Heading < 0) {
      Heading += 360;
    }
  }

  /// <summary>
  /// Handler loop for handling GPS.
  /// </summary>
  /// <returns>Standard IEnumerator</returns>
  public void HandleGPS() {
    // Handle GPS
    if (TeleportalPlatformInfo.IsDesktop) {
      StartCoroutine(HandleGPS_Desktop());
    }
    else if (TeleportalPlatformInfo.IsMobile) {
      StartCoroutine(HandleGPS_Mobile());
    }

    // Repeat
    StartCoroutine(HandleGPS_PlayerRepeat());
  }

  /// <summary>
  /// Sends this player's full origin location and heading to the WiTag server.
  /// </summary>
  /// <param name="latI">The full latitude of the initial (origin) location.</param>
  /// <param name="lonI">The full longitude of the initial (origin) location.</param>
  public void ReportOriginLoc(double latI, double posY, double lonI) {
    // Send to server
    TeleportalNet.Shared.Send(TeleportalCmd.LOCATION_ORIGIN_USER, latI.ToString(), posY.ToString(), lonI.ToString());
  }

  /// <summary>
  /// Sends this player's current delta (minified) location and heading to Teleportal.
  /// </summary>
  public void ReportDeltaLoc(Vector3 scenePosition) {
    // Skip if a location update token has not yet been assigned
    if (UpdateToken == "-") { return; }

    // Create location string
    string locStr = TeleportalCmd.UDP_LOCATION + " "
                    + UpdateToken + " "
                    + scenePosition.x.ToString() + " "
                    + scenePosition.y.ToString() + " "
                    + scenePosition.z.ToString() + " "
                    + ((int)Heading).ToString() + " "
                    + ((int)Pitch).ToString();

    // Send to server via UDP
    TeleportalNet.Shared.SendUDP(locStr);

    // Increment variable
    UpdateCount++;
  }

  /// <summary>
  /// Called by Teleportal.
  /// Prepares this local scene with information about the player's origin location.
  /// </summary>
  /// <param name="latI">The latitude of the player's location.</param>
  /// <param name="posY">The Y coordinate of the player's location.</param>
  /// <param name="lonI">The longitude of the player's location.</param>
  public void LocationOrigin(string latI, string posY, string lonI) {
      // Location is now locked
      LocationLocked = true;

      // Reset player's delta location.
      // This only works on desktop; moving devices are handled in LocationSync()
      TeleportalPlayer.Current.transform.position = new Vector3(0, 0, 0);

      // Show lightspeed effect (teleporting)
      TeleportalUi.Shared.LightspeedToEndor(true);

      // Reposition remaining XR Items, relative to the new GPS position
      TeleportalAr.Shared.RepositionItems();

      // Global signal
      if (TeleportalActions.Shared.OnLocationOrigin != null) {
        TeleportalActions.Shared.OnLocationOrigin();
      }
  }

  /// <summary>
  /// Called by Teleportal.
  /// Handles repositioning of items after a user synchronizes their location.
  /// </summary>
  /// <param name="lat">New origin latitude for this user.</param>
  /// <param name="posY">New origin Y coordinate for this user.</param>
  /// <param name="lon">New origin longitude for this user.</param>
  public void SyncUserLocation(string lat, string posY, string lon) {
      // Reposition remaining XR Items, relative to the new GPS position
      TeleportalAr.Shared.RepositionItems();
  }

  /// <summary>
  /// Called by Teleportal.
  /// Delays the calling of SyncUserLocation().
  /// </summary>
  /// <param name="lat">See SyncUserLocation()</param>
  /// <param name="posY">See SyncUserLocation()</param>
  /// <param name="lon">See SyncUserLocation()</param>
  public void SyncUserLocationDelayed(string lat, string posY, string lon) {
      StartCoroutine( SyncUserLocationDelayedC(lat, posY, lon) );
  }

  /// <summary>
  /// Called by Teleportal.
  /// Delays the calling of SyncUserLocation().
  /// </summary>
  /// <param name="lat">See SyncUserLocation()</param>
  /// <param name="posY">See SyncUserLocation()</param>
  /// <param name="lon">See SyncUserLocation()</param>
  /// <returns>Async IEnumerator</returns>
  private IEnumerator SyncUserLocationDelayedC(string lat, string posY, string lon) {
      yield return new WaitForSeconds(TeleportalGps.Shared.UpdateFrequency * 1.1f);
      SyncUserLocation(lat, posY, lon);
  }

  /// <summary>
  /// Prompts the user to synchronize location with
  /// another user's device by putting the devices
  /// together, facing forward.
  /// </summary>
  /// <param name="username">The name of the user with whom to synchronize locations.</param>
  public void HighFivePrompt(string username) {
      // Only honor this if we're on a mobile, moving device (augmented)
      if (!TeleportalPlatformInfo.IsAugmented) {
        return;
      }

      // Setup
      string prompt = "Sync with " + username;
      string desc = "Put phones together\nfacing forward.\nTap when ready.";
      string buttonTitle = "Ready";
      Action func = delegate {
        // Hide the existing prompt from screen
        TeleportalUi.Shared.HideAlert();

        // Complete transaction with server
        HighFiveNow(username);
      };

      // Prompt "go to your friend" to sync locations
      TeleportalUi.Shared.WaitForAck(prompt, desc, buttonTitle, func);
  }

  /// <summary>
  /// Assigns the current heading to the given point.
  /// </summary>
  /// <param name="pointId">The ID of the point for assignment.</param>
  public void TriangulateStep(int pointId) {
    PointHeadings[pointId - 1] = Heading;
  }

  /// <summary>
  /// Requests a triangulation from Teleportal
  /// using the previously-targeted points.
  /// </summary>
  public void ProcessTriangulation() {
    TeleportalNet.Shared.Send(TeleportalCmd.LOCATION_POINT_HEADINGS, PointHeadings[0].ToString(), PointHeadings[1].ToString(), PointHeadings[2].ToString());
  }

  /// <summary>
  /// Prompts the user to synchronize their location by
  /// aiming at 3 points around their space.
  /// </summary>
  /// <param name="pointId">The current point ID (1, 2, or 3) at which to aim.</param>
  public void PointPrompt(int pointId) {

      TeleportalUi.Shared.LockReticle();
      TeleportalUi.Shared.SetReticle(ReactiveReticle.ReticleType.Heal);

      // Only honor this if we're on a mobile, moving device (augmented)
      if (!TeleportalPlatformInfo.IsAugmented) {
        // return;
      }

      // Setup
      string prompt = "Aim at Point " + pointId.ToString();
      string desc = "Tap when reticle is centered.";
      string buttonTitle = "Ready";
      Action func = delegate {
        // Hide the existing prompt from screen
        TeleportalUi.Shared.HideAlert();

        PointHeadings[pointId - 1] = Heading;

        if (pointId == 3) {
          // Complete transaction with server
          TeleportalNet.Shared.Send(TeleportalCmd.LOCATION_POINT_HEADINGS, PointHeadings[0].ToString(), PointHeadings[1].ToString(), PointHeadings[2].ToString());
          TeleportalUi.Shared.UnlockReticle();
        }
        else {
          PointPromptDelayed(pointId + 1, 0.5f);
        }
      };

      // Prompt ...
      TeleportalUi.Shared.WaitForAck(prompt, desc, buttonTitle, func);
  }

  /// <summary>
  /// Delays the calling of PointPrompt().
  /// </summary>
  /// <param name="pointId">See PointPromt()</param>
  /// <param name="seconds">Number of seconds to delay before calling PointPrompt()</param>
  private void PointPromptDelayed(int pointId, float seconds) {
    StartCoroutine(PointPromptDelayedC(pointId, seconds));
  }

  /// <summary>
  /// Delays the calling of PointPrompt().
  /// </summary>
  /// <param name="pointId">See PointPrompt()</param>
  /// <param name="seconds">Number of seconds to delay before calling PointPrompt()</param>
  /// <returns></returns>
  private IEnumerator PointPromptDelayedC(int pointId, float seconds) {
    yield return new WaitForSeconds(seconds);
    PointPrompt(pointId);
  }

  /// <summary>
  /// Called when two devices are already
  /// positioned next to each other and this
  /// client is ready to synchronize its location.
  /// </summary>
  /// <param name="username">The name of the user with whom to synchronize locations.</param>
  public void HighFiveNow(string username) {
    // Send to server
    TeleportalNet.Shared.Send(TeleportalCmd.META_HIGHFIVE_SENT, username, Heading.ToString());
  }

}
