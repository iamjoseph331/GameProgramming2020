using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    
    void Update()
    {
        transform.position = transform.GetChild(0).position;
        transform.GetChild(0).localPosition = Vector3.zero;
    }
}
