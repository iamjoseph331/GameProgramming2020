using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleBehavior : MonoBehaviour
{
    private bool target;

    public void SetTarget(bool _target)
    {
        target = _target;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (target)
        {
            Debug.Log(collision.transform.name);
            GameObject.Find("NetworkManager").GetComponent<GameLogic>().PointByPole(collision.transform);
        }
    }
}
