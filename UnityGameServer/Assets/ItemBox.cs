using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    /*
     * -1   = unset
     * 0    = accelerate
     */

    public int type = -1;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"Create a box with type {type}");        
    }



    public void OnTriggerEnter(Collider collision) {
        Transform player1GameObject = collision.transform.parent;
        KartController player = player1GameObject.GetComponentInChildren<KartController>();
        player.acceleration *= 10;
        Debug.Log($"Box collided!"); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
