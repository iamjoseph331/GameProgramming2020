using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turn : MonoBehaviour
{
    Vector3 lastPosition;
    void Start()
    {
        lastPosition = transform.parent.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 deltaPosition = transform.parent.position - lastPosition;
        lastPosition = transform.parent.position;
        Vector3 deltaRotate = deltaPosition - transform.forward;
        transform.forward += Mathf.SmoothStep(0, 1, Time.deltaTime) * deltaRotate;
    }
}
