using UnityEngine;
using System;
using System.Collections.Generic;

public class CarController : MonoBehaviour
{
    public enum ControlMode
    {
        Keyboard,
        Buttons
    };

    public enum Axel
    {
        Front,
        Rear
    }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        //public GameObject wheelEffectObj;
        //public ParticleSystem smokeParticle;
        public Axel axel;
    }

    public ControlMode control;

    public float maxAcceleration = 30.0f;
    public float brakeAcceleration = 50.0f;
    public float frictionBrake = 5.0f; // New variable for idle engine/friction braking
    public float maxSpeed = 30.0f; // Added a top speed cap

    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;

    public float downForce = 50.0f;

    public Vector3 _centerOfMass;

    public List<Wheel> wheels;

    float moveInput;
    float steerInput;

    private Rigidbody carRb;


    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass;

    }

    void Update()
    {
        GetInputs();
        AnimateWheels();
        //WheelEffects();
    }

    void FixedUpdate()
    {
        Move();
        Steer();
        Brake();
        ApplyDownForce();
    }

    public void MoveInput(float input)
    {
        moveInput = input;
    }

    public void SteerInput(float input)
    {
        steerInput = input;
    }

    void GetInputs()
    {
        if (control == ControlMode.Keyboard)
        {
            float verticalInput = Input.GetAxisRaw("Vertical");
            float horizontalInput = Input.GetAxisRaw("Horizontal");

            steerInput = horizontalInput;
            moveInput = verticalInput;

            if (Mathf.Approximately(verticalInput, 0f) && !Mathf.Approximately(horizontalInput, 0f))
            {
                moveInput = Mathf.Abs(horizontalInput);
            }
        }
    }

    void Move()
    {
        float forwardSpeed = Vector3.Dot(transform.forward, carRb.linearVelocity);
        
        foreach (var wheel in wheels)
        {
            // Cut engine power if we're going too fast in the direction we want to accelerate
            if (forwardSpeed > maxSpeed && moveInput > 0)
            {
                wheel.wheelCollider.motorTorque = 0;
            }
            else if (forwardSpeed < -maxSpeed && moveInput < 0)
            {
                wheel.wheelCollider.motorTorque = 0;
            }
            else
            {
                wheel.wheelCollider.motorTorque = moveInput * 600 * maxAcceleration;
            }
        }
    }

    void Steer()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 0.6f);
            }
        }
    }

    void Brake()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // Full braking force when pressing Space
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 600 * brakeAcceleration;
                
                // Stop the motor torque from fighting the brakes
                wheel.wheelCollider.motorTorque = 0;
            }
        }
        else if (moveInput == 0)
        {
            // Lighter friction brake when no input keys are pressed
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 600 * frictionBrake;
                
                // Ensure motor torque is off while coasting
                wheel.wheelCollider.motorTorque = 0;
            }
        }
        else
        {
            // No braking when actively driving
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
            }
        }
    }

    void ApplyDownForce()
    {
        carRb.AddForce(-transform.up * downForce * carRb.linearVelocity.magnitude);
    }

    void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot * Quaternion.Euler(0, -90, 0);
        }
    }

    //void WheelEffects()
    //{
    //    foreach (var wheel in wheels)
    //    {
    //        //var dirtParticleMainSettings = wheel.smokeParticle.main;

    //        if (Input.GetKey(KeyCode.Space) && wheel.axel == Axel.Rear && wheel.wheelCollider.isGrounded == true && carRb.linearVelocity.magnitude >= 10.0f)
    //        {
    //            wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = true;
    //            wheel.smokeParticle.Emit(1);
    //        }
    //        else
    //        {
    //            wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = false;
    //        }
    //    }
    //}
}