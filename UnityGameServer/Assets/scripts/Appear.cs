using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appear : MonoBehaviour
{
    // Update is called once per frame
    public GameObject follwer;
    public GameObject Ai;

    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(5);
        follwer.SetActive(true);
    }

    void Update()
    {
        if(GameObject.FindGameObjectsWithTag("Player").Length != 0)
        {
            Ai.SetActive(true);
            StartCoroutine(ExampleCoroutine());
        }
    }
}
