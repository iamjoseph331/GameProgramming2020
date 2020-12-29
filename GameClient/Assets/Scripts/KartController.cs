using System.Collections;
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
    float MAXSPEED = 400f;
    float rotate, currentRotate;
    int driftDirection;
    float driftPower;
    public int driftMode = 0;
    bool first, second, third;
    Color c;

    [Header("Bools")]
    private bool boosting;
    public bool drifting;
    public bool JumpInput = false, JumpEnd = false, AccelInput = false;

    [Header("Parameters")]

    public float startingAcceleration;
    public float acceleration = 80f;
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

    float HorizontalMovement()
    {
        if(HorizontalInput == 0f)
        {
            return Input.GetAxis("Horizontal");
        }
        return HorizontalInput;
    }

    private IEnumerator Countdown()
    {
        float duration = 2f;

        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        acceleration = startingAcceleration;
        boosting = false;
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
        //Follow Collider
        transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);
        transform.parent.position += transform.localPosition;
        sphere.transform.position -= transform.localPosition;
        transform.position -= transform.localPosition;

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
#pragma warning disable CS0618 // 類型或成員已經過時
                p.startColor = Color.clear;
#pragma warning restore CS0618 // 類型或成員已經過時
                p.Play();
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

        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f); speed = 0f; //slowly return to default
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f; //slowly return to default

        //Animations    
        if (transform.GetComponent<AudioSource>() != null)
            transform.GetComponent<AudioSource>().volume = currentSpeed / MAXSPEED;

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
        /*if(!drifting)
            sphere.AddForce(-kartModel.transform.right * currentSpeed, ForceMode.Acceleration);
        else
            sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);

        //Gravity
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);*/

        //Steering
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);
         
        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + (transform.up*.1f), Vector3.down, out hitOn, 1.1f,layerMask);
        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 2.0f, layerMask);

        //Normal Rotation
        kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);
    }

    public void Boost()
    {
        if(boosting)
        {
            return;
        }
        drifting = false;
        boosting = true;

        if (driftMode > 0)
        {
            kartModel.Find("Tube001").GetComponentInChildren<ParticleSystem>().Play();
            kartModel.Find("Tube002").GetComponentInChildren<ParticleSystem>().Play();
        }

        driftPower = 0;
        driftMode = 0;
        first = false; second = false; third = false;

        acceleration *= (1.3f + 0.1f * driftPower);
        StartCoroutine(Countdown());

        foreach (ParticleSystem p in primaryParticles)
        {
#pragma warning disable CS0618 // 類型或成員已經過時
            p.startColor = Color.clear;
#pragma warning restore CS0618 // 類型或成員已經過時
            p.Stop();
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

        if (driftPower > 30 && driftPower < 60 -1 && !first)
        {
            first = true;
            c = turboColors[0];
            driftMode = 1;

            PlayFlashParticle(c);
        }

        if (driftPower > 45 && driftPower < 90 - 1 && !second)
        {
            second = true;
            c = turboColors[1];
            driftMode = 2;

            PlayFlashParticle(c);
        }

        if (driftPower > 90 && !third)
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

    public void Speed(float x)
    {
        currentSpeed = x;
    }

    
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position + transform.up, transform.position - (transform.up * 2));
    //}
}
