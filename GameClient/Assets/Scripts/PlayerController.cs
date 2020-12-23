using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void FixedUpdate()
    {
        SendInputToServer();
    }

    /// <summary>Sends player input to the server.</summary>
    private void SendInputToServer()
    {
        bool[] _inputs = new bool[]
        {
            Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow),
            Input.GetKey(KeyCode.Space),
            Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow),
            Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)
        };

        ClientSend.PlayerMovement(_inputs);
    }
}
