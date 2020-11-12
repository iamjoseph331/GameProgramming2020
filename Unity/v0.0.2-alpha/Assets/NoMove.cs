using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class NoMove : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        ResetVR();
    }

    void ResetVR()
    {
        transform.position = Vector3.zero;
    }
}
