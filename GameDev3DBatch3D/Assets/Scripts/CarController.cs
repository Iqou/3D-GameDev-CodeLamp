using UnityEngine;
using System;
using System.Collections.Generic;

public class CarController : MonoBehaviour
{
    public enum Axel
    {
        Front,
        Rear
    }

    [Serializable]
    public class Wheel
    {
        public Transform wheelModel;
        public WheelCollider wheelCollider;
        public GameObject wheelEffectObj;
        public Axel axel;
    }

    [Header("References")]
    public List<Wheel> wheels;

    Rigidbody rb;

    [Header("Engine")]
    public float acceleration = 35f;
    public float reverseAcceleration = 18f;
    public float maxSpeed = 32f;
    public float engineBrake = 8f;

    [Header("Steering")]
    public float maxSteerAngle = 35f;
    public float minSteerAngle = 10f;
    public float steerSpeed = 180f;

    [Header("Brakes")]
    public float brakeTorque = 5000f;
    public float brakeStrength = 40f;

    [Header("Grip")]
    [Range(0.85f,1f)]
    public float lateralGrip = 0.93f;

    [Range(0.95f,1f)]
    public float driftGrip = 0.985f;

    public float normalSidewaysStiffness = 2.4f;
    public float driftSidewaysStiffness = 0.8f;

    [Header("Physics")]
    public float downforce = 250f;
    public Vector3 centerOfMass;

    float throttle;
    float steering;
    bool handBrake;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.centerOfMass = centerOfMass;
        rb.maxAngularVelocity = 4f;

    }

    void Update()
    {
        throttle = Input.GetAxisRaw("Vertical");
        steering = Input.GetAxisRaw("Horizontal");
        handBrake = Input.GetKey(KeyCode.Space);

        AnimateWheels();
        WheelEffects();

        foreach (var wheel in wheels)
{
    // Debug.Log(wheel.wheelCollider.rpm);
}
    }

    void FixedUpdate()
    {
        Drive();
        Steer();
        Brake();
        Stabilize();
        ApplyDownforce();
        HandleGrip();
    }

    void Drive()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);

        float forwardSpeed = localVelocity.z;

        if (throttle > 0)
        {
            if (forwardSpeed < maxSpeed)
            {
                rb.AddForce(transform.forward * acceleration, ForceMode.Acceleration);
            }
        }
        else if (throttle < 0)
        {
            if (forwardSpeed > -maxSpeed * 0.5f)
            {
                rb.AddForce(transform.forward * reverseAcceleration * throttle, ForceMode.Acceleration);
            }
        }
        else
{

    localVelocity.z = Mathf.Lerp(
        localVelocity.z,
        0f,
        engineBrake * Time.fixedDeltaTime);

    rb.linearVelocity = transform.TransformDirection(localVelocity);
}

        // Clamp forward speed only
        localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        localVelocity.z = Mathf.Clamp(localVelocity.z, -maxSpeed * 0.5f, maxSpeed);

        rb.linearVelocity = transform.TransformDirection(localVelocity);

        foreach (var wheel in wheels)
{
    if (wheel.axel == Axel.Rear)
    {
        wheel.wheelCollider.motorTorque = throttle * 5f;
    }
}
    }

    void Steer()
    {
        float speedPercent = rb.linearVelocity.magnitude / maxSpeed;

        float steerAngle = Mathf.Lerp(
            maxSteerAngle,
            minSteerAngle,
            speedPercent);

        foreach (var wheel in wheels)
        {
            if (wheel.axel != Axel.Front)
                continue;

            float target = steering * steerAngle;

            wheel.wheelCollider.steerAngle =
                Mathf.MoveTowards(
                    wheel.wheelCollider.steerAngle,
                    target,
                    steerSpeed * Time.fixedDeltaTime);
        }
    }

    void Brake()
{

    // Apply brake torque to rear wheels if handbrake is pressed, otherwise set brake torque to 0
    foreach (var wheel in wheels)
    {
        if (wheel.axel == Axel.Rear)
            wheel.wheelCollider.brakeTorque = handBrake ? brakeTorque : 0;
        else
            wheel.wheelCollider.brakeTorque = 0;
    }


    // Apply handbrake effect on the car's velocity (This would cause problem with the space bar drift as it would slow down the car too much)
    if (handBrake)
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);

        localVelocity.z = Mathf.MoveTowards(
            localVelocity.z,
            0f,
            brakeStrength * Time.fixedDeltaTime);

        rb.linearVelocity = transform.TransformDirection(localVelocity);
    }
}
    
    // Stabilize the car's lateral movement by reducing sideways velocity (Creates Drifting)
    void Stabilize()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);

        if (handBrake)
            localVelocity.x *= driftGrip;
        else
            localVelocity.x *= lateralGrip;

        rb.linearVelocity = transform.TransformDirection(localVelocity);
    }

    // Apply downforce to the car based on its speed so it doesn't turnover easily at high speeds and to increase grip
    void ApplyDownforce()
    {
        rb.AddForce(
            -transform.up *
            downforce *
            rb.linearVelocity.magnitude,
            ForceMode.Force);
    }

    // Adjust the sideways friction of the wheels based on whether the handbrake is pressed or not
    void HandleGrip()
    {
        float stiffness = handBrake
            ? driftSidewaysStiffness
            : normalSidewaysStiffness;

        foreach (var wheel in wheels)
        {
            WheelFrictionCurve side = wheel.wheelCollider.sidewaysFriction;
            side.stiffness = stiffness;
            wheel.wheelCollider.sidewaysFriction = side;
        }
    }

    // Animate the wheel models to match the position and rotation of the wheel colliders
    void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.GetWorldPose(
                out Vector3 pos,
                out Quaternion rot);

            wheel.wheelModel.position = pos;
            wheel.wheelModel.rotation = rot * Quaternion.Euler(0, -90, 0);
        }
    }

    void WheelEffects()
    {
        foreach (var wheel in wheels)
        {
            //var dirtParticleMainSettings = wheel.smokeParticle.main;

            if (Input.GetKey(KeyCode.Space) && wheel.axel == Axel.Rear && wheel.wheelCollider.isGrounded == true && rb.linearVelocity.magnitude >= 10.0f)
            {
                wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = true;
                // wheel.smokeParticle.Emit(1);
            }
            else
            {
                wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = false;
            }
        }
    }
}