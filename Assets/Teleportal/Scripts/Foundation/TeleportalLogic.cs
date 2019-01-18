// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using System;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Shared module for controlling networking and client/server communication.
/// </summary>
public class TeleportalLogic : MonoBehaviour {

    private string TP_ID = "TP";

    void Start() {
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.SOCKET_UUID, SocketUuid);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.API_KEY_OK, ApiKeyOk);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.API_KEY_BAD, ApiKeyBad);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.BETA_AUTHORIZED, BetaAuthorized);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.BETA_UNAUTHORIZED, BetaUnauthorized);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.DIRECT_IP, DirectIp);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.HEARTBEAT, Heartbeat);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.APP_IN_REVIEW, AppInReview);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.AUTH_REGISTER_OK, AuthRegisterOk);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.AUTH_REGISTER_BAD, AuthRegisterBad);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.AUTH_LOGIN_OK, AuthLoginOk);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.AUTH_LOGIN_BAD, AuthLoginBad);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.AUTH_DISCONNECT, AuthDisconnect);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.AUTH_VIA_WEB, AuthViaWeb);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.LOCATION_UPDATE_TOKEN, LocationUpdateToken);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.LOCATION_ORIGIN_USER, LocationOriginUser);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.LOCATION_ORIGIN_AREA, LocationOriginArea);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.LOCATION_SYNC, LocationSync);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.LOCATION_UPDATE, LocationUpdate);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.META_HIGHFIVE, MetaHighFive);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.META_INVENTORY, MetaInventory);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.ITEM_ADD, ItemAdd);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.ITEM_DELETE, ItemDelete);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.ITEM_REPOSITION, ItemReposition);
        TeleportalNet.Shared.RegisterCommand(TP_ID, TeleportalCmd.ITEM_STATE, ItemState);
    }

    public void SocketUuid(List<string> args) {
        // Set globally
        TeleportalNet.Shared.SocketUuid = args[1];
    }

    public void AuthViaWeb(List<string> args) {
        // Get values
        TeleportalAuth.Shared.LoginId = args[1];
        TeleportalAuth.Shared.LoginToken = args[2];

        // Attempt login
        TeleportalAuth.Shared.Login();
    }

    public void ApiKeyOk(List<string> args) {
        // Try to log in automatically (if the user has previously signed in with Google or Digits)
        TeleportalAuth.Shared.AttemptSavedLogin();
    }

    public void ApiKeyBad(List<string> args) {
        // Show error: auth failed
        TeleportalUi.Shared.ShowError(TeleportalUi.ErrorType.Login);
    }

    public void BetaAuthorized(List<string> args) {
        // Tell user
        TeleportalUi.Shared.ShowAlert(Alert.TypeStandard, "Congratulations!", "You're in! Welcome to Teleportal", 4);

        // Hide beta screen
        TeleportalUi.Shared.HideBeta();

        // Now that beta is authorized, attempt login again
        TeleportalAuth.Shared.Login();
    }

    public void BetaUnauthorized(List<string> args) {
        // Get number of players ahead of this player in the waiting list
        TeleportalBeta.Shared.BetaWaitAhead = args[1];

        // Show beta screen
        TeleportalUi.Shared.HideSplash(); // hide splash screen first
        TeleportalUi.Shared.ShowBeta();
    }

    public void DirectIp(List<string> args) {
        // Set UDP host
        TeleportalNet.Shared.HostUdp = args[1];

        // Initialize UDP sending client
        TeleportalNet.Shared.ConnectUDP();
    }

    public void Heartbeat(List<string> args) {
        // do nothing; this is just to keep the connection alive :)
    }

    public void AppInReview(List<string> args) {
        // Connect to app review server
        TeleportalNet.Shared.SwitchingServers = true; // enable switching servers flag
        TeleportalNet.Shared.mode = TeleportalNet.Mode.Review; // switch server mode to app review server
        TeleportalNet.Shared.ConfigureConnection(); // reconfigure connection
        StartCoroutine("Connect"); // connect to new server
    }

    public void AuthRegisterOk(List<string> args) {
        // Attempt login
        TeleportalAuth.Shared.Login();
    }

    public void AuthRegisterBad(List<string> args) {
        TeleportalAuth.Shared.Fail();
    }

    public void AuthLoginOk(List<string> args) {
        TeleportalAuth.Shared.Success();
    }

    public void AuthLoginBad(List<string> args) {
        // Attempt register
        TeleportalAuth.Shared.Register();
    }

    public void AuthDisconnect(List<string> args) {
        // Show error: server disconnected
        TeleportalUi.Shared.ShowError(TeleportalUi.ErrorType.Disconnect);
    }

    public void LocationUpdateToken(List<string> args) {
        TeleportalGps.Shared.UpdateToken = args[1];
    }

    public void LocationOriginUser(List<string> args) {
        // Get values
        string latI = args[1];
        string posY = args[2];
        string lonI = args[3];

        // Send to game
        TeleportalGps.Shared.LocationOrigin(latI, posY, lonI);
    }

    public void LocationOriginArea(List<string> args) {
        // Get values
        double lat = double.Parse(args[1]);
        double posY = double.Parse(args[2]);
        double lon = double.Parse(args[3]);
        
        // Set as this user's origin location also
        TeleportalGps.Shared.ReportOriginLoc(lat, posY, lon);
    }

    public void LocationSync(List<string> args) {
        // Get values
        string latI = args[1];
        string posYI = args[2];
        string lonI = args[3];

        // Send to game
        TeleportalGps.Shared.SyncUserLocationDelayed(latI, posYI, lonI);
    }

    public void LocationTrackingReset(List<string> args) {
        // Send to AR
        TeleportalAr.Shared.ResetTracking();
    }

    public void MetaHighFive(List<string> args) {
        // Get values
        string username = args[1];

        // Send to game
        TeleportalGps.Shared.HighFivePrompt(username);
    }

    public void LocationUpdate(List<string> args) {
        // Get values
        string username = args[1];
        string latitude = args[2];
        string posY = args[3];
        string longitude = args[4];
        string heading = args[5];

        // Send to game
        TeleportalAr.Shared.UserLocated(username, latitude, posY, longitude, heading);
    }

    public void ItemAdd(List<string> args) {
        // Get values
        string type = args[1];
        string id = args[2];
        string name = args[3];
        double lat = double.Parse(args[4]);
        double posY = double.Parse(args[5]);
        double lon = double.Parse(args[6]);
        double heading = double.Parse(args[7]);
        double pitch = double.Parse(args[8]);

        // Send to AR
        TeleportalAr.Shared.AddItem(type, id, lat, posY, lon, heading, pitch);
    }

    public void ItemDelete(List<string> args) {
        // Get values
        string id = args[2];

        // Send to AR
        TeleportalAr.Shared.DeleteItemVisual(id);
    }

    public void ItemReposition(List<string> args) {
        // Get values
        string id = args[1];
        string x = args[2];
        string y = args[3];
        string z = args[4];
        string a = args[5];
        string b = args[6];
        string c = args[7];

        // Convert to position and euler rotation vectors
        Vector3 position = new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
        Vector3 euler = new Vector3(float.Parse(a), float.Parse(b), float.Parse(c));

        // Send to AR
        TeleportalAr.Shared.RepositionItem(id, position, euler);
    }

    public void ItemState(List<string> args) {
        // Get values
        string id = args[1];
        string property = args[2];
        string value = args[3];

        // Send state to that XR Item
        TeleportalAr.Shared.SetItemState(id, property, value);
    }

    ///// INVENTORY /////

    public void MetaInventory(List<string> args) {
		// Create a string containing the rest of the arguments, separated by whitespace
		string argsStr = "";
		for (int i = 1; i < args.Count; i++) {
			argsStr += args[i] + " ";
		}

		// Get values
		string json = argsStr;

		// Parse inventory array
		InventoryItem[] array = JsonHelper.FromJson<InventoryItem>(json);

		// Convert the array to a dictionary of: ID -to- inventory item
		TeleportalInventory.Shared.Clear();
		foreach (InventoryItem item in array) {
			TeleportalInventory.Shared.Add(item);
		}
	}
  
}