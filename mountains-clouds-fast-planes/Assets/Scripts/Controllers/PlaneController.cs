using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    public float forwardAcceleration;
    public float pitchAcceleration;
    public float yawAcceleration;
    public float rollAcceleration;
    
    public float throttle;

    private Rigidbody _rb;

    private void Start()
    {
        // fetch the Rigidbody component
        _rb = GetComponent<Rigidbody>();
        
        // set the initial throttle
        throttle = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        var input = PlayerInput.getInput();
        var direction = transform.forward;
        var delta = Time.deltaTime;

        var pitch = transform.right * (input.pitch * pitchAcceleration);
        var roll = transform.forward * (input.roll * rollAcceleration);
        var yaw = transform.up * (input.yaw * yawAcceleration);

        // apply the rotation
        _rb.AddTorque(pitch, ForceMode.Acceleration);
        _rb.AddTorque(roll, ForceMode.Acceleration);
        _rb.AddTorque(yaw, ForceMode.Acceleration);
        
        // update the throttle with input
        throttle = Mathf.Clamp(throttle + (input.acceleration * delta), 0f, 1f);
        
        // calculate the acceleration
        var force = direction * (forwardAcceleration * throttle);
        _rb.AddForce(force, ForceMode.Acceleration);
    }
}
