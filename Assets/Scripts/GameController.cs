using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject gameMessagePanel;
    [SerializeField] private Text gameText;
    [SerializeField] private GameObject waveParentContainer;

    public enum Difficulty
    {
        Normal,
        BulletHell
    }

    private Difficulty difficultyLevel;
    private TranscendScene transcendScript;

    private int score;
    private float initialTimeStart;

    private float leftBorder;
    private float rightBorder;
    private float bottomBorder;
    private float topBorder;

    public Difficulty DifficultyLevel
    {
        get { return difficultyLevel; }
        set { difficultyLevel = value; }
    }

    void Start()
    {
        initialTimeStart = Time.time;
        score = 0;
        GetTranscendData();
        CalculateScreenBorder();
        UpdateScore();
        StartCoroutine(SpawnWaves());
    }

    void Update()
    {
        float timeLasp = Time.time - initialTimeStart;

        if (timeLasp > 5 && timeLasp < 10)
        {
            gameText.text = "Get Ready, Here They Come!";
        }
        else if (timeLasp > 10 && timeLasp < 12)
        {
            gameMessagePanel.SetActive(false);
        }
    }

    private void GetTranscendData()
    {
        GameObject transcendedGameObject = GameObject.FindGameObjectWithTag("TranscendScene");
        transcendScript = transcendedGameObject.GetComponent<TranscendScene>();
        if (transcendScript.GameMode == 0)
            DifficultyLevel = Difficulty.Normal;
        else if (transcendScript.GameMode == 1)
            DifficultyLevel = Difficulty.BulletHell;
        //TODO get score from transcendScript
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(12.0f); //Wait for Take off from platform
        float waveStartTime = Time.time;
        float waveLimit = 30.0f;
        while (Time.time-waveStartTime < waveLimit)
        {
            GameObject parentGameObject = Instantiate(waveParentContainer, new Vector3(), Quaternion.identity) as GameObject;

            int randomEnemySelector = Random.Range(0, 100)%2;
            if (randomEnemySelector == 0)
            {
                SpawnWaveA(parentGameObject);
            }
            else
            {
                StartCoroutine(SpawnWaveB(parentGameObject));
            }
            yield return new WaitForSeconds(6.0f);  //Wait Between Waves
        }
        //TODO Boss
    }

    private IEnumerator SpawnWaveB(GameObject parentGameObject)
    {
        Vector3 spawnPosition;
        int randomTopEdgeSpawn = Random.Range(0, 100)%2;
        if (randomTopEdgeSpawn == 0)
            spawnPosition = new Vector3(leftBorder, topBorder, 0);
        else
            spawnPosition = new Vector3(rightBorder, topBorder, 0);

        for (int i = 0; i < 5; i++)
        {
            GameObject gameObject = Instantiate(enemies[1], spawnPosition, enemies[1].transform.rotation) as GameObject;
            gameObject.transform.parent = parentGameObject.transform;
            gameObject.GetComponent<Enemy>().SetEnemyType("B");
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void SpawnWaveA(GameObject parentGameObject)
    {
        //Initial Random Location To Spawn V formation
        Vector3 spawnPosition = new Vector3(Random.Range(leftBorder+2,rightBorder-2),topBorder+3,0); //spawn while having into account formation requirements
        GameObject child = Instantiate(enemies[0], spawnPosition, enemies[0].transform.rotation) as GameObject;
        child.transform.parent = parentGameObject.transform;

        //Initiates others in a V formation from the starter
        spawnPosition += new Vector3(1, 1, 0);
        child = Instantiate(enemies[0], spawnPosition, enemies[0].transform.rotation) as GameObject;
        child.transform.parent = parentGameObject.transform;

        spawnPosition += new Vector3(-2, 0, 0);
        child = Instantiate(enemies[0], spawnPosition, enemies[0].transform.rotation) as GameObject;
        child.transform.parent = parentGameObject.transform;

        spawnPosition += new Vector3(-1, 1, 0);
        child = Instantiate(enemies[0], spawnPosition, enemies[0].transform.rotation) as GameObject;
        child.transform.parent = parentGameObject.transform;

        spawnPosition += new Vector3(4, 0, 0);
        child = Instantiate(enemies[0], spawnPosition, enemies[0].transform.rotation) as GameObject;
        child.transform.parent = parentGameObject.transform;
    }

    void UpdateScore()
    {
        scoreText.text = score.ToString();
    }

    public void AddScore(int points)
    {
        if (DifficultyLevel == Difficulty.BulletHell)
            points *= 2;
        score += points;
        if (score < 0)
            score = 0;
        UpdateScore();
    }

    private void CalculateScreenBorder()
    {
        var dist = (transform.position - Camera.main.transform.position).z;
        leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).x;
        rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
        bottomBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).y;
        topBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, dist)).y;
    }

    public void GameOver()
    {
        transcendScript.Score = score;
        StartCoroutine(SwitchToGameOverScene());
    }

    private IEnumerator SwitchToGameOverScene()
    {
        yield return new WaitForSeconds(0.5f); //Wait for TSpaceShip To explode
        transcendScript.ChangeScene("GameOver");
    }
}
