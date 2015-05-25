using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOverGameController : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    // Use this for initialization
    private void Start()
    {
        GetTranscendData();
    }

    public void LoadGameMenu()
    {
        Application.LoadLevel("MainMenu");
    }

    private void GetTranscendData()
    {
        GameObject transcendedGameObject = GameObject.FindGameObjectWithTag("TranscendScene");
        TranscendScene transcendScript = transcendedGameObject.GetComponent<TranscendScene>();

        scoreText.text = transcendScript.Score.ToString();
    }
}
