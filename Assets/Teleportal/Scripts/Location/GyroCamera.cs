// Teleportal SDK
// Copyright 2018 WiTag Inc
// This class is from a third-party via forums.

using UnityEngine;
using System.Collections;

/// <summary>
/// Gyroscopically-controlled camera view.
/// </summary>
public class GyroCamera : MonoBehaviour
{
	/// <summary>
	/// First reported Y angle from the gyroscope.
	/// </summary>
	private float initialYAngle = 0f;

	/// <summary>
	/// The applied Y angle from the gyroscope.
	/// </summary>
	private float appliedGyroYAngle = 0f;

	/// <summary>
	/// The calibration Y angle from the gyroscope.
	/// </summary>
	private float calibrationYAngle = 0f;

	/// <summary>
	/// MonoBehaviour load.
	/// </summary>
	void Start()
	{
		Input.gyro.enabled = true;
		Application.targetFrameRate = 60;
		initialYAngle = transform.eulerAngles.y;
	}

	/// <summary>
	/// Updates on each frame.
	/// </summary>
	void Update()
	{
		// If running on a mobile device, apply gyro updates
		#if !UNITY_EDITOR
			ApplyGyroRotation();
			ApplyCalibration();
		#endif
	}

	/// <summary>
	/// Offsets the y angle, in case it was not 0 at edit time.
	/// </summary>
	public void CalibrateYAngle()
	{
		calibrationYAngle = appliedGyroYAngle - initialYAngle;
	}

	/// <summary>
	/// Applies the gyroscope rotation to the camera view.
	/// </summary>
	void ApplyGyroRotation()
	{
		transform.rotation = Input.gyro.attitude;
		transform.Rotate( 0f, 0f, 180f, Space.Self ); // Swap "handedness" of quaternion from gyro.
		transform.Rotate( 90f, 180f, 0f, Space.World ); // Rotate to make sense as a camera pointing out the back of your device.
		appliedGyroYAngle = transform.eulerAngles.y; // Save the angle around y axis for use in calibration.
	}

	/// <summary>
	/// Rotates y angle back however much it deviated when calibrationYAngle was saved.
	/// </summary>
	void ApplyCalibration()
	{
		transform.Rotate( 0f, -calibrationYAngle, 0f, Space.World );
	}
}
