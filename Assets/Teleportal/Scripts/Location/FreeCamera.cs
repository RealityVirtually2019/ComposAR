﻿// Modified code by Ryan Reede

using UnityEngine;

/// <summary>
/// 
/// </summary>
public class FreeCamera : MonoBehaviour {
	public bool holdRightMouseCapture = false;
	                                                                      // average human walking speed ~= 1.4 m/s, running speed ~= 3.6 m/s
	[SerializeField] private float m_MoveSpeed = 2f;                      // How fast the rig will move to keep up with the target's position.
	[SerializeField] private float m_SprintSpeed = 5f;
    [Range(0f, 10f)] [SerializeField] private float m_TurnSpeed = 2.0f;   // How fast the rig will rotate from user input.
	[SerializeField] private float m_TiltMax = 75f;                       // The maximum value of the x axis rotation of the pivot.
	[SerializeField] private float m_TiltMin = 45f;                       // The minimum value of the x axis rotation of the pivot.
	[SerializeField] private bool m_LockCursor = false;                   // Whether the cursor should be hidden and locked.
	[SerializeField] private bool m_VerticalAutoReturn = false;           // set wether or not the vertical axis should auto return

	private float m_LookAngle;                    // The rig's y axis rotation.
	private float m_TiltAngle;                    // The pivot's x axis rotation.

	private bool AllowMovement = false; // start by disallowing movement (in case any UI needs to be displayed at runtime)
	public bool IsEditorCamera = false;
	public bool DroneCamMode = false;

	bool	m_inputCaptured;
	float	m_yaw;
	float	m_pitch;

	void CaptureInput() {
		Cursor.lockState = CursorLockMode.Locked;

		Cursor.visible = false;
		m_inputCaptured = true;
	}

	void ReleaseInput() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		m_inputCaptured = false;
	}

	/// <summary>
	/// Called before Start().
	/// </summary>
	void Awake() {
		if (IsEditorCamera || TeleportalPlatformInfo.IsVirtual) {
			AllowMovement = true;
		}
	}

	/// <summary>
	/// Desktop only.
	/// Checks if movement should be allowed yet,
	/// and sets the permission accordingly.
	/// </summary>
	void CheckMovementPermission() {
		// If the main text field object is active,
		// then temporarily disallow WASD movement.
		if (TeleportalUi.Shared.MainTextField.gameObject.active) {
			AllowMovement = false;
		}

		else if (DroneCamMode) {
			AllowMovement = false;
		}

		// Otherwise, allow WASD movement as usual.
		else {
			AllowMovement = true;
		}
	}

	/// <summary>
	/// Callback for when the app gains or loses focus.
	/// Mainly for Desktop.
	/// </summary>
	/// <param name="focus">Whether the app is now focused in view.</param>
	void OnApplicationFocus(bool focus) {
        if(m_inputCaptured && !focus)  
			ReleaseInput();
	}

	/// <summary>
	/// Updates on every rendered frame.
	/// </summary>
	void Update() {
		if(!m_inputCaptured) {
			if(!holdRightMouseCapture && Input.GetMouseButtonDown(0)) 
				CaptureInput();
			else if(holdRightMouseCapture && Input.GetMouseButtonDown(1))
				CaptureInput();
		}

		if(!m_inputCaptured)
			return;

		if(m_inputCaptured) {
			if(!holdRightMouseCapture && Input.GetKeyDown(KeyCode.Escape))
				ReleaseInput();
			else if(holdRightMouseCapture && Input.GetMouseButtonUp(1))
				ReleaseInput();
		}

		// If the mouse clicked inside the game view...
		if (Input.GetMouseButtonDown(0) && !IsEditorCamera) {
			CheckMovementPermission();
		}

		// If drone mode is activated, toggle movement and the drone camera
		if (Input.GetKeyDown(KeyCode.G)) {
			// Variable toggle
			AllowMovement = !AllowMovement;

			// Active toggle
			transform.parent.gameObject.GetComponent<Camera>().enabled = AllowMovement;

			// UI toggle
			TeleportalUi.Shared.SharedCanvas.gameObject.active = !AllowMovement;
		}

		// Stop here if we shouldn't allow movement
		if (!AllowMovement) { return; }

		// Update mouse rotation
        updateRotation();

		// Lateral movement
		var speed = Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? m_SprintSpeed : m_MoveSpeed);
		var forward = speed * Input.GetAxis("Vertical");
		var right = speed * Input.GetAxis("Horizontal");
		var up = 0f; //speed * ((Input.GetKey(KeyCode.E) ? 1f : 0f) - (Input.GetKey(KeyCode.Q) ? 1f : 0f));
		Vector3 forwardVector = transform.parent.forward;

		// Restrict players to lateral vector (disallow vertical)
		if (!DroneCamMode) {
			forwardVector.y = 0f;
		}
		
		// Execute transformation
		transform.parent.position += forwardVector * forward + transform.parent.right * right + Vector3.up * up;
	}

	/// <summary>
	/// Updates the rotation of the camera,
	/// in response to the mouse.
	/// </summary>
	private void updateRotation()
	{
		// Read the user input
		var x = Input.GetAxis("Mouse X");
		var y = Input.GetAxis("Mouse Y");

		// Adjust the look angle by an amount proportional to the turn speed and horizontal input.
		m_LookAngle += x * m_TurnSpeed;

		// on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
		m_TiltAngle -= y * m_TurnSpeed;

		// and make sure the new value is within the tilt range
		m_TiltAngle = Mathf.Clamp(m_TiltAngle, -m_TiltMin, m_TiltMax);
		transform.parent.rotation = Quaternion.Euler(m_TiltAngle, m_LookAngle, 0);
	}

}
