using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public Transform[] startline;
    public Transform[] players;
    private IEnumerator TenSecondes()
    {
        float duration = 5f;
        float normalizedTime = 0f;
        while(normalizedTime < 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        GameObject.Find("Slime").GetComponent<slimeBehavior>().StartSlime();
        for(int i = 0; i < players.Length; ++i)
        {
            if (players[i] == null) break;
            players[i].transform.position = startline[i].position;
            players[i].transform.rotation = startline[i].rotation;

            Transform tmp = players[i].Find("Canvas");
            tmp = tmp.Find("ReadyPanel");
            tmp = tmp.Find("READY");
            tmp.GetComponentInChildren<Button>().onClick.Invoke();
            players[i].GetComponent<Player>().MoveToStart();
        }
    }

    public void CountDown()
    {
        StartCoroutine(TenSecondes());
    }
    private void Start()
    {

    }
}
