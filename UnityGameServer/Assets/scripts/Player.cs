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

    private bool[] inputs;
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

        inputs = new bool[4];
    }
    private bool flag = false;
    private float deltaHorizontal = 0f;

    public void FixedUpdate()
    {
        Vector2 _inputDirection = Vector2.zero;
        if (inputs == null) return;
        if (inputs[0])
        {
            kartcon.GetComponent<KartController>().AccelInput = true;
        }
        else
        {
            kartcon.GetComponent<KartController>().AccelInput = false;
        }
        if (inputs[1] && !flag)
        {
            flag = true;
            kartcon.GetComponent<KartController>().JumpInput = true;
            kartcon.GetComponent<KartController>().JumpEnd = false;
        }
        else if(inputs[1] == false)
        {
            flag = false;
            kartcon.GetComponent<KartController>().JumpInput = false;
            kartcon.GetComponent<KartController>().JumpEnd = true;
        }
        if (inputs[2])
        {
            deltaHorizontal -= 1 / moveSpeed;
            deltaHorizontal = Mathf.Max(-1f, deltaHorizontal);
        }
        if (inputs[3])
        {
            deltaHorizontal += 1 / moveSpeed;
            deltaHorizontal = Mathf.Min(1f, deltaHorizontal);
        }
        else if(!inputs[2] && !inputs[3])
        {
            deltaHorizontal = 0f;
        }

        kartcon.GetComponent<KartController>().HorizontalInput = deltaHorizontal;
        Move(_inputDirection);

        transform.GetChild(2).rotation = rotations;
    }

    private void Move(Vector2 _inputDirection)
    {
        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        _moveDirection *= moveSpeed;
        
        yVelocity = gravity;
        _moveDirection.y = yVelocity;

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
            inputs = _inputs;
            rotations = _rotation;        
    }
}