using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public GameObject playerPrefab;
    private GameLogic gameLogic;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Debug.Log("Instance already exsists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        gameLogic = gameObject.GetComponent<GameLogic>();

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        Server.Start(50, 26950);
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    private static int CountPlayers()
    {
        int playerCount = 0;
        foreach (Client _client in Server.clients.Values)
        {
            if (_client.player != null)
            {
                playerCount += 1;
            }
        }
        return playerCount;
    }

    public Player InstantiatePlayer()
    {
        Player _player = Instantiate(playerPrefab, transform.position, Quaternion.identity).GetComponent<Player>();
        if (CountPlayers() >= 1)
        {
            gameObject.GetComponent<GameLogic>().CountDown();
            foreach(Client _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    gameObject.GetComponent<GameLogic>().AssignPlayerNumber(_client.player.transform);
                }
            }
            gameObject.GetComponent<GameLogic>().AssignPlayerNumber(_player.transform);
        }
        return _player;
    }
}
