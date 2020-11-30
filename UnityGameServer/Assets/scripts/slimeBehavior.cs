using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slimeBehavior : MonoBehaviour
{
    public float speed = 0.8f;
    bool start = false;
    
    public void StartSlime()
    {
        start = true;   
    }

    private void FixedUpdate()
    {
        if(start)
            transform.Translate(Vector3.up * Time.deltaTime * speed);
    }
}
