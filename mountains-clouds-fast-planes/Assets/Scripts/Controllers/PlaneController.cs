using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    public float forwardAcceleration = 50f;
    public float pitchAcceleration = 45f;
    public float yawAcceleration = 15f;
    public float rollAcceleration = 75f;
    public float maxVelocity = 150f;

    public float dragMultiplier = 0.7f;

    public float gravityAcceleration = 15f;

    public Vector3 velocity = new Vector3(0f, 0f, 0f);
    public float throttle;

    private void Start()
    {
        throttle = 0.5f;
        velocity = new Vector3(0, 0, 10);
    }

    // Update is called once per frame
    void Update()
    {
        var input = PlayerInput.getInput();
        var direction = transform.forward;
        var delta = Time.deltaTime;

        // apply the rotation
        transform.Rotate(
            relativeTo: Space.Self,
            xAngle: input.pitch * pitchAcceleration * delta,
            yAngle: input.yaw * yawAcceleration * delta,
            zAngle: input.roll * rollAcceleration * delta
        );

        throttle = Mathf.Clamp(throttle + (input.acceleration * delta), 0f, 1f);
        
        // calculate the velocity, as applying acceleration to current velocity,
        // in the direction of the nose
        velocity += direction * (forwardAcceleration * throttle * delta);
        
        // apply gravity
        velocity += Vector3.down * (gravityAcceleration * delta);
        
        // we multiply the velocity vector by a constant to simulate drag
        var drag = -velocity * dragMultiplier;
        velocity += drag * delta;
        
        velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
        
        transform.position += velocity * delta;
    }
}
