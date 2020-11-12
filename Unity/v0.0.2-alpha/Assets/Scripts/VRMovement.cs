using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using UnityEngine.Events;
using Valve.VR;

public class VRMovement : MonoBehaviour
{

    [Tooltip("This is where motions of the controllers are set")]
    public SteamVR_Action_Single m_squeeze;
    public SteamVR_Action_Boolean m_right;
    public SteamVR_Action_Boolean m_left;
    public SteamVR_Action_Boolean m_side;
    public SteamVR_Input_Sources handType;

    [Tooltip("This is where parameters of the controllers are set")]
    public float max_velocity;
    public float accel;
    public float balance;

    private Transform body;
    private Vector3 Pos_Hand2Body;
    private Vector3 m_NewForce;
    void Start()
    {
        body = transform.parent.Find("Camera").Find("Body");
    }
    /*
    private void Add_Accel() {
        if (m_trigger.GetAxis(handType) > 0.03f)
        {
            //print("Trigger is down");
            m_NewForce = accel * m_trigger.GetAxis(handType) * transform.forward * -1f;
            transform.parent.GetComponent<Rigidbody>().AddForce(m_NewForce, ForceMode.Acceleration);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player" && collision.transform != transform.parent)
            GameObject.Find("GameLogic").GetComponent<GameLogic>().PointByTag(transform.parent);
    }*/

    void Update()
    {
        //Pos_Hand2Body = transform.position - body.position;
        //Add_Accel();
    }

    private void SendInputToServer()
    {
        bool sim_up = m_squeeze.GetAxis(handType) > 0.3f;
        bool sim_jump = m_side.GetState(handType);
        bool sim_left = m_left.GetState(handType);
        bool sim_right = m_right.GetState(handType);
        bool[] _inputs = new bool[] {
            sim_up,
            sim_jump,
            sim_left,
            sim_right
        };

        ClientSend.PlayerMovement(_inputs);
    }

    private void FixedUpdate()
    {
        SendInputToServer();
    }

}
