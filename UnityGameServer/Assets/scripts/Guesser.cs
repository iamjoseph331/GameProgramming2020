using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.IO;

public class Guesser : MonoBehaviour
{
    private int maxQuestionCount = 7;
    public int LapGoal = 5;
    public int LapCounter = 0;
    public bool guessed = false;
    public int guess;
    public Transform[] StartingPositions;
    public Text _question;
    public TMP_Text _laps;
    public GameObject gameover, gameclear;

    public AudioClip correct, wrong, death, victory;
    bool passCheckpoint = true;
    public bool inOrder = true;

    private bool end = false;

    public int groundTruth = 1;
    string Ansa, Ansb, Ansc, Ansd;
    // Start is called before the first frame update

    string path = "config.ini";
    int twos, threes, fours;
    private void Start()
    {
        string configpath = Path.Combine(Application.dataPath, path);
        print(configpath);
        StreamReader reader = new StreamReader(configpath);
        if (reader != null)
        {
            string[] buf = reader.ReadToEnd().Split(' ');
            twos = Int32.Parse(buf[0]);
            threes = Int32.Parse(buf[1]);
            fours = Int32.Parse(buf[2]);
            reader.Close();
        }
        print("2: " + twos);
        print("3: " + threes);
        print("4: " + fours);
    }

    public void Casual()
    {
        inOrder = true;
    }

    [Serializable]
    public class Post
    {
        public int id;
        public int answer;
        public int taku;
        public string ansa, ansb, ansc, ansd;
        public string question;
    }

    private IEnumerator Countdown()
    {
        float duration = 3f;
        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        _question.text = "";
    }

    private IEnumerator RespawnItem(Transform other)
    {
        float duration = 15f;
        float normalizedTime = 0;
        while(normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        other.GetComponent<MeshRenderer>().enabled = true;
    }

    IEnumerator GetRequest(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Send the request and wait for a response
            yield return request.SendWebRequest();
            if(request.error == null)
            {
                Post p = JsonUtility.FromJson<Post>(request.downloadHandler.text);
                _question.color = Color.black;
                _question.text = p.question;
                groundTruth = p.answer;
                Ansa = p.ansa;
                Ansb = p.ansb;
                Ansc = p.ansc;
                Ansd = p.ansd;
            }
            else
            {
                print("ERROR");
            }
        }
    }

    IEnumerator RegainMovement()
    {
        float duration = 3f;
        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        transform.parent.GetComponentInChildren<KartController>().acceleration = 
            transform.parent.GetComponentInChildren<KartController>().startingAcceleration;
    }

    public void StartingCountdown()
    {
        StartCoroutine(RegainMovement());
    }

    IEnumerator Goal(bool flag)
    {
        if(flag) _question.text = "GOAL!!!";
        else _question.text = "";
        float duration = 0.5f;
        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        StartCoroutine(Goal(!flag));
    }

    private int Min(int a, int b)
    {
        return a > b ? b : a;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.name == "GoalLine")
        {
            if (guessed == true && transform.position.y > StartingPositions[LapCounter].position.y)
            {
                LapCounter += 1;
                passCheckpoint = false;
                _laps.text = " " + Min(5, LapCounter + 1).ToString() + "/5";
            }
            guessed = false;
        }
        else if(other.name == "Slime")
        {
            gameover.SetActive(true);
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.parent.GetComponentInChildren<KartController>().acceleration = 0;
            transform.parent.GetComponentInChildren<KartController>().Speed(0f);
            transform.GetComponent<AudioSource>().clip = death;
            transform.GetComponent<AudioSource>().Play();
            end = true;
        }
        else if (other.name == "AnswerA")
        {
            guess = 1;
            _question.color = Color.yellow;
            _question.text = "A. " + Ansa;

            StartCoroutine(Countdown());
        }
        else if (other.name == "AnswerB")
        {
            guess = 2;
            _question.color = Color.yellow;
            _question.text = "B. " + Ansb;

            StartCoroutine(Countdown());
        }
        else if (other.name == "AnswerC")
        {
            guess = 3;
            _question.color = Color.yellow;
            _question.text = "C. " + Ansc;

            StartCoroutine(Countdown());
        }
        else if (other.name == "AnswerD")
        {
            guess = 4;
            _question.color = Color.yellow;
            _question.text = "D. " + Ansd;

            StartCoroutine(Countdown());
        }
        else if (other.name == "QuestionAppear")
        {
            _question.color = Color.red;
            _question.text = "Question " + (LapCounter + 1).ToString();

            int rand;
            if(LapCounter == 2)
            {
                rand = twos + UnityEngine.Random.Range(0, threes);
            }
            else if(LapCounter == 3)
            {
                rand = twos + threes + UnityEngine.Random.Range(0, fours);
            }
            else
            {
                int tmp = UnityEngine.Random.Range(0, twos);
                while(tmp == 2 || tmp == 3)
                {
                    tmp = UnityEngine.Random.Range(0, twos);
                }
                rand = tmp;
            }
            if(inOrder)
            {
                rand = LapCounter;
            }
            StartCoroutine(GetRequest(string.Concat("https://my-json-server.typicode.com/iamjoseph331/Database/posts/", rand.ToString()))); 
        }
        else if (other.name == "TransferZone")
        {
            if (guess != groundTruth)
            {
                transform.position = StartingPositions[LapCounter].position;
                transform.parent.GetChild(2).rotation = StartingPositions[LapCounter].rotation;
                transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
                _question.text = "Wrong!";
                transform.GetComponent<AudioSource>().clip = wrong;
                transform.GetComponent<AudioSource>().Play();
            }
            else
            {
                guessed = true;
                _question.text = "Correct!";
                transform.GetComponent<AudioSource>().clip = correct;
                transform.GetComponent<AudioSource>().Play();
            }
            StartCoroutine(Countdown());
        }
        else if(other.name == "Accelerate")
        {
            other.transform.GetComponent<MeshRenderer>().enabled = false;
            transform.parent.GetComponentInChildren<KartController>().driftMode = 1;
            transform.parent.GetComponentInChildren<KartController>().Boost();
            StartCoroutine(RespawnItem(other.transform));
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if(LapCounter == LapGoal)
        {
            if(!end)
            {
                StartCoroutine(Goal(true));
                end = true;
                gameclear.SetActive(true);
                transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
                transform.parent.GetComponentInChildren<KartController>().acceleration = 0;
                transform.parent.GetComponentInChildren<KartController>().Speed(0f);
                transform.GetComponent<AudioSource>().clip = victory;
                transform.GetComponent<AudioSource>().Play();
                GameObject.Find("Slime").GetComponent<slimeBehavior>().Stop();
            }
        }
        else if (transform.position.y > StartingPositions[LapCounter].position.y - 2.5)
        {
            passCheckpoint = true;
        }
        else if (passCheckpoint && transform.position.y < StartingPositions[LapCounter].position.y - 2.5)
        {
            transform.position = StartingPositions[LapCounter].position;
            transform.parent.GetChild(2).rotation = StartingPositions[LapCounter].rotation;
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.parent.GetComponentInChildren<KartController>().acceleration = 0;
            transform.parent.GetComponentInChildren<KartController>().Speed(0f);
            StartCoroutine(RegainMovement());
        }
        else if(LapCounter > 0 && !passCheckpoint && transform.position.y < StartingPositions[LapCounter - 1].position.y - 2.5)
        {
            transform.position = StartingPositions[LapCounter].position;
            transform.parent.GetChild(2).rotation = StartingPositions[LapCounter].rotation;
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.parent.GetComponentInChildren<KartController>().acceleration = 0;
            transform.parent.GetComponentInChildren<KartController>().Speed(0f);
            StartCoroutine(RegainMovement());
        }
    }
}
