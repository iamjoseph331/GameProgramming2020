using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public float m_timer = 600;
    
    private int player_count;
    private List<Transform> players;
    private List<int> score_players;
    private List<Transform> poles;
    private int target_pole;
    private bool m_timer_start = false;

    [Tooltip("Pole settings")]
    public int polecount = 4;
    public GameObject pole;
    public Material pole_material;
    public Material target_pole_material;

    private void Start()
    {  
        poles = new List<Transform>(); 
    }

    public void StartGame()
    {
        //initialize
        
        target_pole = 0;
        poles.Add(Instantiate(pole, new Vector3(0f, 0f, 0f), Quaternion.identity).transform);
        poles.Add(Instantiate(pole, new Vector3(0f, 0f, 300f), Quaternion.identity).transform);
        poles.Add(Instantiate(pole, new Vector3(300f, 0f, 300f), Quaternion.identity).transform);
        poles.Add(Instantiate(pole, new Vector3(300f, 0f, 0f), Quaternion.identity).transform);
        poles[target_pole].GetComponent<MeshRenderer>().material = target_pole_material;
        
        //put guide lines
        LineRenderer lr = transform.GetComponentInChildren<LineRenderer>();
        lr.positionCount = polecount;
        for (int i = 0; i < polecount; ++i) {
            lr.SetPosition(i, poles[i].position);
        }
        lr.loop = true;
        //start timer
        m_timer_start = true;
    }

    void EndGame() {
        
        for (int i = 0; i < polecount; ++i) {
            Destroy(poles[i]);
        }
        player_count = 0;
        transform.GetComponentInChildren<LineRenderer>().SetPositions(null);
    }

    private void PointVibrate() {
        GameObject.Find("VibrationController").GetComponent<VibrationController>().PulseRight(0.5f, 150, 175);
        GameObject.Find("VibrationController").GetComponent<VibrationController>().PulseLeft(0.5f, 150, 175);
    }

    public bool PointByPole(Transform player) {
        PointVibrate();
        return true;
    }

    public bool PointByTag(Transform player) {
        PointVibrate();
        return true;
    }

    private void Update()
    {
        if (m_timer_start)
        {
            m_timer -= Time.deltaTime;
        }
        if (m_timer < 0)
        {
            m_timer = 0f;
            EndGame();
        }
    }
}
