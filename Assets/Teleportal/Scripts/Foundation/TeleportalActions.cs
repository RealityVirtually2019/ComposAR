// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Shared module for adding Teleportal listeners anywhere.
/// </summary>
public class TeleportalActions : MonoBehaviour {

    /* Shared Object References */

    public static TeleportalActions Shared;

    /* Unity Actions */

    public UnityAction OnNetworkConnected;
    public UnityAction OnLocationLock;
    public UnityAction OnLocationUpdate;
    public UnityAction OnLocationOrigin;
    public UnityAction SignalFindObjects;
    public UnityAction OnWorldShow;
    public UnityAction OnWorldHide;

    void Awake() {
        // Set static self reference
        TeleportalActions.Shared = this;
    }

}
