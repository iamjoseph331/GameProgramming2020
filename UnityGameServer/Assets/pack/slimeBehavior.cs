using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slimeBehavior : MonoBehaviour
{
    public float speed = 0.8f;
    bool start = false, over = false;

    public Transform floor;
    
    public void SpeedUp()
    {
        speed *= 2;
    }

    public void SpeedDown()
    {
        speed /= 2;
    }

    public void Stop()
    {
        speed = 0f;
    }

    public void StartSlime()
    {
        start = true;   
    }

    private void FixedUpdate()
    {
        if(start)
            transform.Translate(Vector3.up * Time.deltaTime * speed);
        if(!over && transform.position.y > floor.position.y)
        {
            over = true;
            transform.GetComponent<AudioSource>().Play();
            floor.GetComponent<AudioSource>().Stop();
        }
    }
}
