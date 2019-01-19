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
using System.Text.RegularExpressions;

/// <summary>
/// Shared module for controlling networking and client/server communication.
/// </summary>
public class TeleportalNet : MonoBehaviour {

	/* Shared Object References */

  public static TeleportalNet Shared;

	// Networking chooser
	/// <summary>
	/// Different WiTag servers to which the client can connect.
	/// </summary>
	public enum Mode { Production, Review, Alpha, Dev, Local };

	/// <summary>
	/// The current server connection type.
	/// </summary>
	public Mode mode = Mode.Production;

	[HideInInspector]
	/// <summary>
	/// The current WebSocket connection hostname.
	/// </summary>
	public string HostWs = "";

	[HideInInspector]
	/// <summary>
	/// The current UDP connection hostname.
	/// </summary>
	public string HostUdp = "";

	[HideInInspector]
	/// <summary>
	/// The current WebSocket connection port.
	/// </summary>
	public string PortWs = "";

	[HideInInspector]
	/// <summary>
	/// The current UDP connection port.
	/// </summary>
	public string PortUdp = "";

	/// <summary>
	/// Hostname for the production server.
	/// </summary>
	public string ProductionHost = "s.teleportal.app";

	/// <summary>
	/// WebSocket port on the production server.
	/// </summary>
	public string ProductionPortWs = "443";

	/// <summary>
	/// UDP port on the production server.
	/// </summary>
	public string ProductionPortUdp = "8081";

	// App Review server

	/// <summary>
	/// Hostname for the review server.
	/// </summary>
	public string ReviewHost = "sr.teleportal.app";

	/// <summary>
	/// WebSocket port for the review server.
	/// </summary>
	public string ReviewPortWs = "443";

	/// <summary>
	/// UDP port for the review server.
	/// </summary>
	public string ReviewPortUdp = "8081";

	// Alpha testing server

	/// <summary>
	/// Hostname for the alpha testing server.
	/// </summary>
	public string AlphaHost = "sa.teleportal.app";

	/// <summary>
	/// WebSocket port for the alpha testing server.
	/// </summary>
	public string AlphaPortWs = "443";

	/// <summary>
	/// UDP port for the alpha testing server.
	/// </summary>
	public string AlphaPortUdp = "8081";

	// Development server

	/// <summary>
	/// Hostname for the development server.
	/// </summary>
	public string DevHost = "sd.teleportal.app";

	/// <summary>
	/// WebSocket port for the development server.
	/// </summary>
	public string DevPortWs = "443";

	/// <summary>
	/// UDP port for the development server.
	/// </summary>
	public string DevPortUdp = "8081";

	// Local server

	/// <summary>
	/// Hostname for a local machine server.
	/// </summary>
	public string LocalHost = "localhost";

	/// <summary>
	/// WebSocket port for a local machine server.
	/// </summary>
	public string LocalPortWs = "8081";

	/// <summary>
	/// UDP port for a local machine server.
	/// </summary>
	public string LocalPortUdp = "8081";

	/* Networking objects */

	/// <summary>
	/// The current WebSocket connection object.
	/// </summary>
	private WebSocket W;

	/// <summary>
	/// The current UDP client object.
	/// </summary>
	private UdpClient UDP;

	/// <summary>
	/// The IP endpoint for the UDP client.
	/// </summary>
	private IPEndPoint UdpEndpoint;

	private Dictionary<string, System.Action<List<string>>> CommandRegistry = new Dictionary<string, System.Action<List<string>>>();

	/* Networking status */

	/// <summary>
	/// Whether the client is currently connected to a server.
	/// </summary>
	public bool IsConnected = false;

	/// <summary>
	/// Whether the client is currently in between server connections (switching servers).
	/// </summary>
	public bool SwitchingServers = false;

	public string SocketUuid = "";

  void Awake() {
    // Set static self reference
    TeleportalNet.Shared = this;
  }

/// <summary>
/// MonoBehaviour load.
/// </summary>
void Start() {
	// Configure networking based on enum choice
	ConfigureConnection();

	// Start connecting WebSocket client
	StartCoroutine(ConnectWS());

	// Wait for network to be connected
	StartCoroutine(WaitForNetwork());
}

    void OnApplicationQuit() {
        W.Close();
    }

	public void ConnectModule(string moduleId) {
		Send(TeleportalCmd.MODULE_CONNECT, moduleId);
	}

  /// <summary>
  /// Waits for network to be connected. Then calls NetworkConnected().
  /// </summary>
  /// <returns>Standard IEnumerator</returns>
  IEnumerator WaitForNetwork() {
    // Wait for network connection...
    yield return new WaitUntil(() => IsConnected == true);
    
    // Global signal
    if (TeleportalActions.Shared.OnNetworkConnected != null) {
      TeleportalActions.Shared.OnNetworkConnected();
    }
  }

	/// <summary>
	/// Configures the server connection variables
	/// based on the mode selected in the "Mode" global variable.
	/// </summary>
	public void ConfigureConnection() {
		switch(mode) {
			case Mode.Production:
				HostWs = ProductionHost;
				PortWs = ProductionPortWs;
				PortUdp = ProductionPortUdp;
				break;
			case Mode.Review:
				HostWs = ReviewHost;
				PortWs = ReviewPortWs;
				PortUdp = ReviewPortUdp;
				break;
			case Mode.Alpha:
				HostWs = AlphaHost;
				PortWs = AlphaPortWs;
				PortUdp = AlphaPortUdp;
				break;
			case Mode.Dev:
				HostWs = DevHost;
				PortWs = DevPortWs;
				PortUdp = DevPortUdp;
				break;
			case Mode.Local:
				HostWs = LocalHost;
				PortWs = LocalPortWs;
				PortUdp = LocalPortUdp;
				break;
		}
	}

	/// <summary>
	/// Starts ConnectWS() and ConnectUDP()
	/// </summary>
	/// <returns>Standard IEnumerator</returns>
	public IEnumerator Connect() {
		yield return StartCoroutine(ConnectWS());
		ConnectUDP();
	}

	/// <summary>
	/// Connects to the server via WebSocket.
	/// </summary>
	/// <returns>Standard IEnumerator</returns>
	public IEnumerator ConnectWS() {
		// Open WebSocket to Teleportal Server
		W = new WebSocket(new Uri("wss://" + HostWs + ":" + PortWs));
		yield return StartCoroutine(W.Connect());
		ConnectedWS();
	}

	/// <summary>
	/// Initializes a UDP client object for .
	/// </summary>
	public void ConnectUDP() {
		UdpEndpoint = new IPEndPoint(IPAddress.Parse(HostUdp), int.Parse(PortUdp));
		UDP = new UdpClient();
	}

	/// <summary>
	/// Called by ConnectWS() when the WebSocket client has connected to the server.
	/// </summary>
	void ConnectedWS() {
		// Set connected flag globally
		IsConnected = true;

		// Register network listener coroutine
		StartCoroutine("Receive");

		// Send build identifier to server
		Send(TeleportalCmd.API_KEY, TeleportalProject.Shared.ApiKey);

		// Start sending heartbeat (no initial delay, send every 30 seconds)
		InvokeRepeating("Heartbeat", 0.0f, 30.0f);
	}

	/// <summary>
	/// Sends one "heartbeat" message to the WiTag server.
	/// </summary>
	void Heartbeat() {
		Send(TeleportalCmd.HEARTBEAT);
	}

	/// <summary>
	/// Sends a message to the WiTag server via WebSocket.
	/// </summary>
	/// <param name="msg">The message to send.</param>
	void Send(string msg) {
		// Check connection first
		if (IsConnected) {
			// Send to server via WebSockets
			W.SendString(msg);
		}
		// TP // Debug.Log("SEND: " + msg);
	}

	/// <summary>
	/// Sends a command and arguments as a message to the WiTag server via WebSocket.
	/// </summary>
	/// <param name="cmd">The command string to send.</param>
	/// <param name="args">Any number of consecutive arguments to send.</param>
	public void Send(string cmd, params string[] args) {
		// Construct message from command string and arguments array
		string msg = cmd;
		foreach (string arg in args) {
			// Encode all spaces (if there are any)
			String argModified = Regex.Replace(arg, @"\s", "*");

			// Add to message buffer string
			msg += " " + argModified;
		}

		// Send to server via WebSocket
		Send(msg);
	}

	/// <summary>
	/// Sends a message to the WiTag server via UDP.
	/// </summary>
	/// <param name="msg">The message to send.</param>
	public void SendUDP(string msg) {
		// Convert UTF8 string to byte array
		byte[] data = Encoding.UTF8.GetBytes(msg);

		// Send to server via UDP
		UDP.Send(data, data.Length, UdpEndpoint);
	}

	/// <summary>
	/// Process loop for receiving messages from the WiTag server.
	/// </summary>
	/// <returns>Standard IEnumerator</returns>
	IEnumerator Receive() {
		// Check connection first
		while (IsConnected) {
			// Receive message from server (if there is one)
			string msg = W.RecvString();
			if (msg != null)
			{
				// Parse message
				try {
					Parse(msg);
				} catch (Exception e) {
					Debug.Log("NET parse error: " + e);
				}
				// TP // Debug.Log("RECV: " + msg);
			}

			// Handle error
			if (W.error != null) {
				TeleportalUi.Shared.HideSplash(); // hide splash screen (if visible)
				Debug.LogError("ERROR: " + W.error); // log to Unity
				TeleportalUi.Shared.ShowError(TeleportalUi.ErrorType.Disconnect); // show permanent error screen
				Close(); // close websocket connection
				IsConnected = false; // break loop and stop
			}

			yield return 0;
		}
	}

	/// <summary>
	/// Prints a byte array to the console.
	/// </summary>
	/// <param name="bytes">The byte array to print.</param>
	public void PrintByteArray(byte[] bytes) {
		var sb = new StringBuilder("BYTES { ");
		foreach (var b in bytes) {
		sb.Append(b + ", ");
		}
		sb.Append("}");
	}

	/// <summary>
	/// Parses an incoming message from the server.
	/// </summary>
	/// <param name="msg">The message to parse.</param>
	void Parse(string msg) {
		// Remove newline character (keep everything before it)
		msg = msg.Split('\n')[0];

		// Split input into arguments list, separated by whitespace
		List<string> args = new List<string>(msg.Split(' '));

		// Extract command from arguments list
		string cmd = args[0];

		// Create a string containing the rest of the arguments, separated by whitespace
		string argsStr = "";
		for (int i = 1; i < args.Count; i++) {
			argsStr += args[i] + " ";
		}

		PrintByteArray(Encoding.ASCII.GetBytes(msg));

		// Send to the correct Module
		ParseByModule(cmd, args, argsStr);
	}

	void ParseByModule(string cmd, List<string> args, string argsStr) {
		// Create a temporary output variable, for the try function
		System.Action<List<string>> func;

		// Try to get the function to call
		if (CommandRegistry.TryGetValue(cmd, out func)) {
			// Call the function with args
			func(args);
		}
	}

	public void RegisterCommand(string id, string cmd, System.Action<List<string>> func) {
		// If Teleportal internal
		if (id.Equals("TP")) {
			// Add the specified function with the command only
			CommandRegistry.Add(cmd, func);
		}
		// If third-party module
		else {
			// Add the specified function with the module ID & command
			CommandRegistry.Add(id + "-" + cmd, func);
		}
	}

	/*public void RegisterModuleCommands(string[] cmds, System.Action<List<string>>[] funcs) {
		// Add the specified functions with the commands
		for (int i = 0; i < cmds.length; i++) {
			RegisterCommand(cmds[i], funcs[i]);
		}
	}*/

	/// <summary>
	/// Closes the connection to Teleportal.
	/// </summary>
	void Close() {
		// Set connected flag globally
		IsConnected = false;

		// Error to player
		TeleportalUi.Shared.ShowError(TeleportalUi.ErrorType.Disconnect);

		// Close WebSocket
		W.Close();
	}

}
