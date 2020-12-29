using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public float gravity = 0f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
    public Transform kartcon;

    private bool start = false;
    private Vector3 positions;
    private Quaternion rotations;
    private float yVelocity = 0;

    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;

        start = false;
    }

    public void FixedUpdate()
    {
        transform.position = positions;
        transform.GetChild(2).rotation = rotations;
        Move();
    }

    private void Move()
    {
        ServerSend.PlayerPosition(this, false);
        ServerSend.PlayerRotation(this);
    }

    public void MoveToStart()
    {
        ServerSend.PlayerPosition(this, true);
        ServerSend.PlayerRotation(this);
        start = true;
    }

    public void SetInput(Vector3 position, Quaternion _rotation)
    {
        if(start)
        {
            positions = position;
            rotations = _rotation;
        }
    }
}