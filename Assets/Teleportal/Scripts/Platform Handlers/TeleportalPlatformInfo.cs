// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shared static module for providing platform-specific runtime information.
/// </summary>
public static class TeleportalPlatformInfo {

	// TODO remove these temp variables and actually test those features at runtime
	static bool TEMP;

	/* Desktop */

	public static RuntimePlatform CurrentPlatform {
		get { return Application.platform; }
	}

	public static bool IsDesktop {
		get {
			return CurrentPlatform == RuntimePlatform.WindowsPlayer
						|| CurrentPlatform == RuntimePlatform.WindowsEditor
						|| CurrentPlatform == RuntimePlatform.OSXPlayer
						|| CurrentPlatform == RuntimePlatform.OSXEditor
						|| CurrentPlatform == RuntimePlatform.LinuxPlayer
						|| CurrentPlatform == RuntimePlatform.LinuxEditor;
		}
	}

	public static bool HasWASD {
		get { return IsDesktop; }
	}

	/* Desktop VR (Oculus Rift / HTC Vive) */

	public static bool IsVirtualDesktop {
		get {
			return CurrentXRDevice == XRDeviceModel.OculusRift
						|| CurrentXRDevice == XRDeviceModel.HtcVive;
		}
	}

	public static bool HasJoystick {
		get { return TEMP; }
	}

	public static bool HasLighthouse {
		get { return TEMP; }
	}

	/* Mobile VR (Gear VR / Daydream / Oculus Go) */

	public static bool IsVirtualCardboard {
		get { return CurrentXRDevice == XRDeviceModel.GoogleCardboard; }
	}

	public static bool IsVirtualMobile {
		get { return CurrentXRDevice == XRDeviceModel.SamsungGear
					|| CurrentXRDevice == XRDeviceModel.Daydream; }
	}

	/* Mobile (iOS / Android) */

	public static bool IsMobileApple {
		get { return CurrentPlatform == RuntimePlatform.IPhonePlayer; }
	}

	public static bool IsMobileGoogle {
		get { return CurrentPlatform == RuntimePlatform.Android; }
	}

	public static bool IsMobile {
		get { return IsMobileApple || IsMobileGoogle; }
	}

	public static bool HasTouchscreen {
		get { return TEMP; }
	}

	/* Mobile AR (ARKit / ARCore) */

	public static bool HasSLAM {
		get { return TEMP; }
	}

	/* Immersive (general cases for AR / VR) */

	public enum XRDeviceModel {
		None, Unknown,
		/* VR */ OculusRift, HtcVive, SamsungGear, Daydream, GoogleCardboard,
		/* AR */ AppleArkit, GoogleArCore
	}

	public static XRDeviceModel CurrentXRDevice {
		get {
			string model = UnityEngine.XR.XRDevice.model;

			// return XRDeviceModel.GoogleArCore; // TEMP hardcode

			if (IsMobileApple) {
				return XRDeviceModel.AppleArkit; // TODO actually check AR engine status
			}
			else if (IsMobileGoogle) {
				return XRDeviceModel.GoogleArCore; // TODO actually check AR engine status
			}

			// If VR not connected
			if (model == null) {
				return XRDeviceModel.None;
			}

			// Desktop VR
			if (model.IndexOf("Rift") >= 0) {
				return XRDeviceModel.OculusRift;
			}
			else if (model.IndexOf("Vive") >= 0) {
				return XRDeviceModel.HtcVive;
			}

			// Mobile VR
			else if (model.IndexOf("Gear") >= 0) {
				return XRDeviceModel.SamsungGear;
			}
			else if (model.IndexOf("Daydream") >= 0) {
				return XRDeviceModel.Daydream;
			}
			else if (model.IndexOf("Cardboard") >= 0) {
				return XRDeviceModel.GoogleCardboard;
			}

			// Unknown / other
			else {
				return XRDeviceModel.Unknown;
			}
		}
	}

	// Is this a virtual reality headset?
	public static bool IsVirtual {
		get { return IsVirtualCardboard
					|| IsVirtualMobile
					|| IsVirtualDesktop;
		}
	}

	// Is this a mobile augmented reality camera device?
	public static bool IsAugmented {
		get { return CurrentXRDevice == XRDeviceModel.AppleArkit
					|| CurrentXRDevice == XRDeviceModel.GoogleArCore; }
	}

	// Is this an immersive (either AR or VR) device?
	public static bool IsImmersive {
		get { return IsVirtual || IsAugmented; }
	}

}
