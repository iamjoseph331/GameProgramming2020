using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Guesser : MonoBehaviour
{
    private int maxQuestionCount = 3;
    public int LapGoal = 5;
    public int LapCounter = 0;
    public bool guessed = false;
    public int guess;
    public Transform StartingPosition;
    public Text _question;

    private bool end = false;

    public int groundTruth = 1;
    // Start is called before the first frame update

    [Serializable]
    public class Post
    {
        public int id;
        public string question;
        public int answer;
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
            }
            else
            {
                print("ERROR");
            }
        }
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if(other.name == "GoalLine")
        {
            _question.text = "";
            if (guessed == true)
            {
                LapCounter += 1;
            }
            if(LapCounter == LapGoal)
            {
                end = true;
            }
            guessed = false;
        }
        else if (other.name == "AnswerA")
        {
            guess = 1;
            _question.color = Color.yellow;
            _question.text = "No";
        }
        else if (other.name == "AnswerB")
        {
            guess = 2;
            _question.color = Color.yellow;
            _question.text = "Yes";
        }
        else if (other.name == "QuestionAppear")
        {
            _question.color = Color.red;
            _question.text = "Question " + (LapCounter + 1).ToString();
            int rand = UnityEngine.Random.Range(0, maxQuestionCount);
            StartCoroutine(GetRequest(string.Concat("https://my-json-server.typicode.com/iamjoseph331/Database/posts/", rand.ToString()))); 
        }
        else if (other.name == "TransferZone")
        {
            
            if (guess != groundTruth)
            {
                transform.position = StartingPosition.position;
                transform.parent.GetChild(2).rotation = StartingPosition.rotation;
                _question.text = "Wrong!";
            }
            else
            {
                guessed = true;
                _question.text = "Correct!";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
