using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class TranscendScene : MonoBehaviour
{
    private int gameMode = 0;

    public int GameMode
    {
        get { return gameMode; }
        set
        {
            gameMode = value;
            ChangeScene();
        }
    }

    // Use this for initialization
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void ChangeScene()
    {
        Application.LoadLevel("Level1");
    }
}