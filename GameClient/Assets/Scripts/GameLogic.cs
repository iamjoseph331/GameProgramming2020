using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public float mainTimer = 600;

    //the number of current players
    private int playerCount;
    //TBD
    private List<Transform> playerTransforms;
    private List<int> playerScores;
    private List<Transform> poles;
    private int target_pole;
    private bool mainTimerStart = false;

    [Tooltip("Pole settings")]
    public int polecount = 4;
    public GameObject pole;
    public Material pole_material;
    public Material target_pole_material;

    private void Start()
    {
        playerTransforms = new List<Transform>();
        playerScores = new List<int>();
        poles = new List<Transform>();
    }

    IEnumerator WaitSecond(int waitingTime)
    {
        for (int i = waitingTime; i >= 0; --i)
        {
            Debug.Log($"Starting Game in {i}");
            yield return new WaitForSecondsRealtime(1);
        }
        StartGame();
    }

    public void CountDown()
    {
        int waitingTime = 8;
        StartCoroutine(WaitSecond(waitingTime));
    }

    private void StartGame()
    {
        //initialize
        for (int i = 0; i < playerCount; ++i)
        {
            playerScores.Add(0);
        }
        target_pole = 0;
        poles.Add(Instantiate(pole, new Vector3(0f, 0f, 0f), Quaternion.identity).transform);
        poles.Add(Instantiate(pole, new Vector3(0f, 0f, 300f), Quaternion.identity).transform);
        poles.Add(Instantiate(pole, new Vector3(300f, 0f, 300f), Quaternion.identity).transform);
        poles.Add(Instantiate(pole, new Vector3(300f, 0f, 0f), Quaternion.identity).transform);
        poles[0].GetComponent<MeshRenderer>().material = target_pole_material;

        playerTransforms[0].position = new Vector3(3f, 0f, 0f);
        playerTransforms[1].position = new Vector3(-3f, 0f, 0f);

        //put guide lines
        LineRenderer lr = transform.GetComponentInChildren<LineRenderer>();
        lr.positionCount = polecount;
        for (int i = 0; i < polecount; ++i)
        {
            lr.SetPosition(i, poles[i].position);
        }
        lr.loop = true;
        //start timer
        mainTimerStart = true;
    }

    void EndGame()
    {
        //find winner
        List<int> winners = new List<int>();
        int highscore = 0;

        for (int i = 0; i < playerCount; ++i)
        {
            if (playerScores[i] == highscore) winners.Add(i);
            if (playerScores[i] > highscore)
            {
                winners.Clear();
                winners.Add(i);
            }
        }
        Debug.Log($"Player(s) {winners} wins");
        transform.localPosition -= Vector3.up * 300;
        for (int i = 0; i < polecount; ++i)
        {
            Destroy(poles[i]);
        }
        playerCount = 0;
        transform.GetComponentInChildren<LineRenderer>().SetPositions(null);
    }

    public int AssignPlayerNumber(Transform attendant)
    {
        playerTransforms.Add(attendant);
        playerCount += 1;
        return playerCount;
    }

    private void PointVibrate()
    {
        //Send a packet to trigger vibration
    }

    public bool PointByPole(Transform player)
    {
        int id = playerTransforms.IndexOf(player);
        playerScores[id] += 1;
        //poles[target_pole % polecount].GetComponent<PoleBehavior>().SetTarget(false);
        poles[target_pole % polecount].GetComponent<MeshRenderer>().material = pole_material;
        target_pole += 1;
        //poles[target_pole % polecount].GetComponent<PoleBehavior>().SetTarget(true);
        poles[target_pole % polecount].GetComponent<MeshRenderer>().material = target_pole_material;
        print("SCORE: " + playerScores[0].ToString() + " : " + playerScores[1].ToString() + "\n");
        print("Target: " + (target_pole % polecount).ToString());
        PointVibrate();
        return true;
    }

    public bool PointByTag(Transform player)
    {
        int id = playerTransforms.IndexOf(player);
        playerScores[id] += 1;
        print("SCORE: " + playerScores[0].ToString() + " : " + playerScores[1].ToString() + "\n");
        PointVibrate();
        return true;
    }

    public void SetCurrentPlayerCount(int _playerCount)
    {
        playerCount = _playerCount;
    }

    private void FixedUpdate()
    {

        if (mainTimerStart)
        {
            mainTimer -= Time.deltaTime;
            //Debug.Log(mainTimer);
            //GameObject.Find("Text").GetComponent<Text>().text = ((int)(m_timer / 60)).ToString() + ":" + ((int)(m_timer % 60)).ToString();
        }
        if (mainTimer < 0)
        {
            mainTimer = 0f;
            EndGame();
        }
    }
}
