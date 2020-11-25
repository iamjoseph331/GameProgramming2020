﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KartController : MonoBehaviour
{

    public Transform kartModel;
    public Transform kartNormal;
    public Rigidbody sphere;

    public List<ParticleSystem> primaryParticles = new List<ParticleSystem>();
    public List<ParticleSystem> secondaryParticles = new List<ParticleSystem>();

    float speed, currentSpeed;
    float rotate, currentRotate;
    int driftDirection;
    float driftPower;
    int driftMode = 0;
    bool first, second, third;
    Color c;

    [Header("Bools")]
    public bool drifting;

    [Header("Parameters")]

    public float acceleration = 30f;
    public float steering = 80f;
    public float gravity = 10f;
    public LayerMask layerMask;

    [Header("Model Parts")]

    public Transform frontWheels;
    public Transform backWheels;
    public Transform steeringWheel;

    [Header("Particles")]
    public Transform wheelParticles;
    public Transform flashParticles;
    public Color[] turboColors;

    public float HorizontalInput = 0f;
    public bool JumpInput = false, JumpEnd = false, AccelInput = false;

    float HorizontalMovement()
    {
        if(HorizontalInput == 0f)
        {
            return Input.GetAxis("Horizontal");
        }
        return HorizontalInput;
    }

    bool JumpMovement()
    {
        if(!JumpInput)
        {
            return Input.GetButtonDown("Jump");
        }
        return JumpInput;
    }

    bool JumpEnded()
    { 
        if(!JumpEnd)
        {
            return Input.GetButtonUp("Jump");
        }
        return JumpEnd;
    }

    bool AccelMovement()
    {
        if(!AccelInput)
        {
            return Input.GetButton("Fire1");
        }
        return AccelInput;
    }

    void Start()
    {
        
        for (int i = 0; i < wheelParticles.GetChild(0).childCount; i++)
        {
            primaryParticles.Add(wheelParticles.GetChild(0).GetChild(i).GetComponent<ParticleSystem>());
        }

        for (int i = 0; i < wheelParticles.GetChild(1).childCount; i++)
        {
            primaryParticles.Add(wheelParticles.GetChild(1).GetChild(i).GetComponent<ParticleSystem>());
        }

        foreach(ParticleSystem p in flashParticles.GetComponentsInChildren<ParticleSystem>())
        {
            secondaryParticles.Add(p);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float time = Time.timeScale == 1 ? .9f : 1;
            Time.timeScale = time;
        }

        //Follow Collider
        transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);

        //Accelerate
        if (AccelMovement())
            speed = acceleration;

        //Steer
        if (HorizontalMovement() != 0)
        {
            int dir = HorizontalMovement() > 0 ? 1 : -1;
            float amount = Mathf.Abs((HorizontalMovement()));
            Steer(dir, amount);
        }

        //Drift
        if (JumpMovement() && !drifting && HorizontalMovement() != 0)
        {
            drifting = true;
            driftDirection = HorizontalMovement() > 0 ? 1 : -1;

            foreach (ParticleSystem p in primaryParticles)
            {
             //   p.startColor = Color.clear;
            //    p.Play();
            }
        }

        if (drifting)
        {
            float control = (driftDirection == 1) ? (1f + HorizontalMovement()) : (1f - HorizontalMovement());
            float powerControl = (driftDirection == 1) ? (HorizontalMovement()  + 1f)/ 2f : (1f - HorizontalMovement()) / 2f;
            Steer(driftDirection, control);
            driftPower += powerControl;

            ColorDrift();
        }

        if (JumpEnded() && drifting)
        {
            Boost();
        }

        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f); speed = 0.3f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;

        //Animations    

        //a) Kart
        if (!drifting)
        {
            kartModel.localEulerAngles = Vector3.Lerp(kartModel.localEulerAngles, new Vector3(0, 90 + (HorizontalMovement() * 15), kartModel.localEulerAngles.z), .2f);
        }
        else
        {
            float control = (driftDirection == 1) ? (HorizontalMovement() + 1f) : (1f - HorizontalMovement());
            kartModel.parent.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(kartModel.parent.localEulerAngles.y,(control * 15) * driftDirection, .2f), 0);
        }

        //b) Wheels
        frontWheels.localEulerAngles = new Vector3(0, (HorizontalMovement() * 15), frontWheels.localEulerAngles.z);
        frontWheels.localEulerAngles += new Vector3(0, 0, sphere.velocity.magnitude/2);
        backWheels.localEulerAngles += new Vector3(0, 0, sphere.velocity.magnitude/2);

        //c) Steering Wheel
        steeringWheel.localEulerAngles = new Vector3(-25, 90, ((HorizontalMovement() * 45)));

    }

    private void FixedUpdate()
    {
        //Forward Acceleration
        if(!drifting)
            sphere.AddForce(-kartModel.transform.right * currentSpeed, ForceMode.Acceleration);
        else
            sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);

        //Gravity
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        //Steering
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);

        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + (transform.up*.1f), Vector3.down, out hitOn, 1.1f,layerMask);
        Physics.Raycast(transform.position + (transform.up * .1f)   , Vector3.down, out hitNear, 2.0f, layerMask);

        //Normal Rotation
        kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);
    }

    public void Boost()
    {
        drifting = false;

        if (driftMode > 0)
        {
            kartModel.Find("Tube001").GetComponentInChildren<ParticleSystem>().Play();
            kartModel.Find("Tube002").GetComponentInChildren<ParticleSystem>().Play();
        }

        driftPower = 0;
        driftMode = 0;
        first = false; second = false; third = false;

        foreach (ParticleSystem p in primaryParticles)
        {
        //    p.startColor = Color.clear;
        //    p.Stop();
        }


    }

    public void Steer(int direction, float amount)
    {
        rotate = (steering * direction) * amount;
    }

    public void ColorDrift()
    {
        if(!first)
            c = Color.clear;

        if (driftPower > 50 && driftPower < 100-1 && !first)
        {
            first = true;
            c = turboColors[0];
            driftMode = 1;

            PlayFlashParticle(c);
        }

        if (driftPower > 100 && driftPower < 150- 1 && !second)
        {
            second = true;
            c = turboColors[1];
            driftMode = 2;

            PlayFlashParticle(c);
        }

        if (driftPower > 150 && !third)
        {
            third = true;
            c = turboColors[2];
            driftMode = 3;

            PlayFlashParticle(c);
        }

        foreach (ParticleSystem p in primaryParticles)
        {
            var pmain = p.main;
            pmain.startColor = c;
        }

        foreach(ParticleSystem p in secondaryParticles)
        {
            var pmain = p.main;
            pmain.startColor = c;
        }
    }

    void PlayFlashParticle(Color c)
    {
        

        foreach (ParticleSystem p in secondaryParticles)
        {
            var pmain = p.main;
            pmain.startColor = c;
            p.Play();
        }
    }

    private void Speed(float x)
    {
        currentSpeed = x;
    }

    
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position + transform.up, transform.position - (transform.up * 2));
    //}
}