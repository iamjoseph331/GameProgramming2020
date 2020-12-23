using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam : MonoBehaviour
{
    Vector3 lastpos;
    // Start is called before the first frame update
    void Start()
    {
        lastpos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(lastpos, transform.position, 0.999f);
        lastpos = transform.position;
    }
}
