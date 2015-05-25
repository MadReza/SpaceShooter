using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class TranscendScene : MonoBehaviour
{
    private int gameMode = 0;
    private int score = 0;

    public int GameMode
    {
        get { return gameMode; }
        set
        {
            gameMode = value;
            ChangeScene("Level1");
        }
    }

    public int Score
    {
        get { return score; }
        set { score = value; }
    }

    // Use this for initialization
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeScene(String name)
    {
        Application.LoadLevel(name);
    }
}