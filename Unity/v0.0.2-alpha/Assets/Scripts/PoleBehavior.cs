using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleBehavior : MonoBehaviour
{
    private bool is_target;

    public void SetTarget(bool swtch) {
        is_target = swtch;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (is_target)
        {
            print(collision.transform.name);
            GameObject.Find("GameLogic").GetComponent<GameLogic>().PointByPole(collision.transform.parent);
        }
    }

    void Update()
    {
        
    }
}
