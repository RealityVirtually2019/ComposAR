// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

 using UnityEngine;
 using System.Collections;

/// <summary>
/// Controls jumping on Desktop.
/// </summary>
public class Jump : MonoBehaviour {

    /// <summary>
    /// The full Vector3 for a jump action.
    /// </summary>
    public Vector3 jump;

    /// <summary>
    /// The force of the jump.
    /// </summary>
    public float jumpForce = 2.0f;

    /// <summary>
    /// The physics rigidbody component of this GameObject.
    /// </summary>
    Rigidbody rb;

    /// <summary>
    /// Standard MonoBehaviour Start function.
    /// </summary>
    void Start() {
        rb = transform.parent.gameObject.GetComponent<Rigidbody>();
        jump = new Vector3(0.0f, 2.0f, 0.0f);
    }

    /// <summary>
    /// Updates on every rendered frame.
    /// </summary>
    void Update(){
        if (Input.GetKeyDown(KeyCode.Space)){
            rb.AddForce(jump * jumpForce, ForceMode.Impulse);
        }
    }
}