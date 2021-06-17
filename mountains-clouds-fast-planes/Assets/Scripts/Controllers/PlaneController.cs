using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaneController : MonoBehaviour
{
    public float forwardAcceleration;
    public float pitchAcceleration;
    public float yawAcceleration;
    public float rollAcceleration;

    public float minPropellerSpeed;
    public float maxPropellerSpeed;
    
    public float leaveTime;
    public float throttle;

    public int maxHitPoints = 100;
    private int _currentHitPoints;

    private bool isInputEnabled;

    public List<GameObject> departingObjectsBeforeLeaving;

    public int hitPoints
    {
        get => _currentHitPoints;

        set
        {
            _currentHitPoints = value;
            if (_currentHitPoints <= 0)
            {
                TriggerGameOver();
            }
        }
    }

    public Rigidbody rb;
    private Transform _propTransform;

    private PauseResume _pauseResume;

    private void Start()
    {
        isInputEnabled = true;
        Application.targetFrameRate = 60;
        
        // fetch the Rigidbody component
        rb = GetComponent<Rigidbody>();
        
        // fetch the transform of the propeller
        _propTransform = transform.Find("AirplaneModel").Find("fuselage02").Find("prop_tip01");
        
        // set the initial throttle
        throttle = 0.5f;
        
        _currentHitPoints = maxHitPoints;
        
        _pauseResume = GameObject.Find("PauseGame").GetComponent<PauseResume>();
    }

    // Update is called once per frame
    void Update()
    {
        // propeller animation
        var propellerSpeed = (minPropellerSpeed + (maxPropellerSpeed - minPropellerSpeed) * throttle) * Time.deltaTime;
        _propTransform.Rotate(0f, 0f, 2.3f*propellerSpeed);
    }

    private void FixedUpdate()
    {
        if (_pauseResume.IsRunning)
        {
            var input = isInputEnabled ? PlayerInput.getInput() : PlayerInput.nullInput;
            ApplyInput(input);
        }
    }

    private void ApplyInput(PlayerInput.Input input)
    {
        var ts = transform;
        var delta = Time.deltaTime;

        var pitch = ts.right * (input.pitch * pitchAcceleration);
        var roll = ts.forward * (input.roll * rollAcceleration);
        var yaw = ts.up * (input.yaw * yawAcceleration);

        // apply the rotation
        rb.AddTorque(pitch, ForceMode.Acceleration);
        rb.AddTorque(roll, ForceMode.Acceleration);
        rb.AddTorque(yaw, ForceMode.Acceleration);
        
        // update the throttle with input
        throttle = Mathf.Clamp(throttle + (input.acceleration * delta), 0f, 1f);
        
        // calculate the acceleration
        var force = ts.forward * (forwardAcceleration * throttle);
        rb.AddForce(force, ForceMode.Acceleration);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        var message = $"{collision.impulse.magnitude}";
        message = collision.contacts.Aggregate(message, (current, contact) => current + $", {contact.thisCollider}");
        print(message);

        hitPoints -= (int)(collision.impulse.magnitude * 0.01);
    }

    private void TriggerGameOver()
    {
        throttle = 0f;
        foreach(GameObject obj in departingObjectsBeforeLeaving) {
            var departing = obj.GetComponent<IDeparting>();
            if (departing != null)
                departing.Depart();
        }
        Invoke("LeaveGame", leaveTime);
    }

    void LeaveGame() {
        SceneManager.LoadScene("Menu");
    }
}
