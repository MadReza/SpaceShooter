using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuGameController : MonoBehaviour
{
    [SerializeField] private GameObject transcendPrefab;
    [SerializeField] private List<Button> sceneButtons;

    // Use this for initialization
    private void Start()
    {
        GameObject transcendGameObject = GameObject.FindGameObjectWithTag("TranscendScene");

        if (transcendGameObject == null)
        {
            transcendGameObject = Instantiate(transcendPrefab);
        }

        TranscendScene script = transcendGameObject.GetComponent<TranscendScene>();
        sceneButtons[0].onClick.AddListener(() => { script.GameMode = 0; }); //Normal Mode
        sceneButtons[1].onClick.AddListener(() => { script.GameMode = 1; }); //Bullet Hell Mode
    }
}