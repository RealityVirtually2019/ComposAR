// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour for limiting the activation of the attached GameObject, to specific platform(s).
/// </summary>
public class TeleportalPlatformTarget : MonoBehaviour {

  public enum TeleportalPlatformType {
		All,
		Desktop,
		Virtual,
		Augmented,
		DesktopAndVirtual,
		VirtualAndAugmented,
    DesktopAndArkit,
		AppleArkitOnly,
		GoogleArcoreOnly,
		None,
        EditorOnly
	};

	public TeleportalPlatformType TargetPlatform;

	// Initializer function
	void Awake() {
		// Temporary variable for storing active/inactive state
		bool active = false;

		// Activate if on the given platform
		switch (TargetPlatform) {
			case TeleportalPlatformType.All:
				active = true;
				break;
			case TeleportalPlatformType.Desktop:
				active = TeleportalPlatformInfo.IsDesktop && !TeleportalPlatformInfo.IsVirtualDesktop;
				break;
			case TeleportalPlatformType.Virtual:
				active = TeleportalPlatformInfo.IsVirtual;
				break;
			case TeleportalPlatformType.Augmented:
				active = TeleportalPlatformInfo.IsAugmented;
				break;
			case TeleportalPlatformType.DesktopAndVirtual:
				active = TeleportalPlatformInfo.IsDesktop || TeleportalPlatformInfo.IsVirtual;
				break;
			case TeleportalPlatformType.VirtualAndAugmented:
				active = TeleportalPlatformInfo.IsVirtual || TeleportalPlatformInfo.IsAugmented;
				break;
			case TeleportalPlatformType.DesktopAndArkit:
				active = (TeleportalPlatformInfo.IsDesktop || TeleportalPlatformInfo.CurrentXRDevice == TeleportalPlatformInfo.XRDeviceModel.AppleArkit) && !TeleportalPlatformInfo.IsVirtualDesktop;
				break;
			case TeleportalPlatformType.AppleArkitOnly:
				active = TeleportalPlatformInfo.CurrentXRDevice == TeleportalPlatformInfo.XRDeviceModel.AppleArkit;
				break;
			case TeleportalPlatformType.GoogleArcoreOnly:
				active = TeleportalPlatformInfo.CurrentXRDevice == TeleportalPlatformInfo.XRDeviceModel.GoogleArCore;
				break;
			case TeleportalPlatformType.None:
				active = false;
				break;
			default:
				active = false;
				break;
		}

        // Handle editor-only state
        if (TargetPlatform == TeleportalPlatformType.EditorOnly) {
            #if UNITY_EDITOR
                active = true;
            #endif
        }

		// Apply state
		gameObject.SetActive(active);
	}

}
