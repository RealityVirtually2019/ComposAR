// Teleportal SDK
// Code by Thomas Suarez
// Copyright 2018 WiTag Inc

using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Helper functions for animating movement.
/// </summary>
public class MovementHelper {

  /// <summary>
  /// Animates movement and euler rotation from one transform to a target position.
  /// </summary>
  /// <param name="transform">The Unity transform object to modify.</param>
  /// <param name="targetPos">The target vector3 position to which to animate.</param>
  /// <param name="targetEuler">The target vector3 euler rotation to which to animate.</param>
  /// <param name="animationTime">Numnber of seconds over which to animate.</param>
  /// <returns>Standard IEnumerator.</returns>
  public static IEnumerator AnimateMoveRotateTo(Transform transform, Vector3 targetPos, Vector3 targetEuler, float animationTime) {
    // Get source position and rotation
    Vector3 sourcePos = transform.localPosition;
    Quaternion sourceRot = transform.localRotation;

    // Calculate target rotation from euler angle
    Quaternion targetRot = Quaternion.Euler(0, targetEuler.y, 0);
    float diffRot = Mathf.Abs(targetRot.eulerAngles.y - sourceRot.eulerAngles.y);

    // Animate the position & rotation changes
    float startTime = Time.time;
    while (Time.time < startTime + animationTime) {
      // Current delta time (between 0 and 1)
      float deltaTime = (Time.time - startTime) / animationTime;

      // Spherically interpolate position
      transform.localPosition = Vector3.Slerp(sourcePos, targetPos, deltaTime);

      // Rotate towards target quaternion (shortest path) in calculated number of steps
      float steps = deltaTime * diffRot;
      transform.localRotation = Quaternion.RotateTowards(sourceRot, targetRot, steps);

      // Continue
      yield return null;
    }
  }
  
}
