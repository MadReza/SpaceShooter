using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private Text scoreText;

    private enum Difficulty
    {
        Normal,
        BulletHell
    }

    private Difficulty difficulty;

    private int score;
    private float initialTimeStart;

    private float leftBorder;
    private float rightBorder;
    private float bottomBorder;
    private float topBorder;

    void Start()
    {
        initialTimeStart = Time.time;
        score = 0;
        GetTranscendData();
        CalculateScreenBorder();
        UpdateScore();
        StartCoroutine(SpawnWaves());
    }

    private void GetTranscendData()
    {
        GameObject transcendedGameObject = GameObject.FindGameObjectWithTag("TranscendScene");
        TranscendScene transcendScript = transcendedGameObject.GetComponent<TranscendScene>();
        if (transcendScript.GameMode == 0)
            difficulty = Difficulty.Normal;
        else if (transcendScript.GameMode == 1)
            difficulty = Difficulty.BulletHell;
        //TODO get score from transcendScript
    }

    IEnumerator SpawnWaves()
    {
        //Vector3 spawnPosition = new Vector3(0, 0, 0);
        //Instantiate(enemy1, spawnPosition, enemy1.transform.rotation);

        yield return new WaitForSeconds(12.0f); //Wait for Take off from platform
        float waveStartTime = Time.time;
        float waveLimit = 30.0f;
        while (Time.time-waveStartTime < waveLimit)
        {
            int randomEnemySelector = Random.Range(0, 100)%2;
            if (randomEnemySelector == 0)
            {
                SpawnWaveA();
            }
            else
            {
                StartCoroutine(SpawnWaveB());
            }
            yield return new WaitForSeconds(5.0f);  //Wait Between Waves
        }
        //TODO Boss
    }

    private IEnumerator SpawnWaveB()
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
            gameObject.GetComponent<Enemy>().SetEnemyType("B");
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void SpawnWaveA()
    {
        //Initial Random Location To Spawn V formation
        Vector3 spawnPosition = new Vector3(Random.Range(leftBorder+2,rightBorder-2),topBorder+3,0); //spawn while having into account formation requirements
        Instantiate(enemies[0], spawnPosition, enemies[0].transform.rotation);

        //Initiates others in a V formation from the starter
        spawnPosition += new Vector3(1, 1, 0);
        Instantiate(enemies[0], spawnPosition, enemies[0].transform.rotation);
        spawnPosition += new Vector3(-2, 0, 0);
        Instantiate(enemies[0], spawnPosition, enemies[0].transform.rotation);
        spawnPosition += new Vector3(-1, 1, 0);
        Instantiate(enemies[0], spawnPosition, enemies[0].transform.rotation);
        spawnPosition += new Vector3(4, 0, 0);
        Instantiate(enemies[0], spawnPosition, enemies[0].transform.rotation);
    }

    void UpdateScore()
    {
        scoreText.text = score.ToString();
    }

    public void AddScore(int points)
    {
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
}
