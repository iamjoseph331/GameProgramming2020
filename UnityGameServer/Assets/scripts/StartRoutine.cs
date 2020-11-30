using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartRoutine : MonoBehaviour
{

    public TMP_Text txt;

    IEnumerator Countdown()
    {
        txt.text = "3";
        float duration = 3f;
        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            txt.text = (3 - (int)(normalizedTime * 3f)).ToString();
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public void CountdownFromThree()
    {
        StartCoroutine(Countdown());
    }
}
