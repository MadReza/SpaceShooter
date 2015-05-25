using System;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject spawnLocation;
    [SerializeField] private float fireRate = 5.0f;
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private GameObject explosion;
    [SerializeField] private AudioClip explosionAudioClip;

    private Wave waveController;
    private Rigidbody2D _rigidbody2D;
    private GameObject playerGameObject;
    private float nextFire = 0;
    private AudioSource fireAudioSource;
    private float initialTime;
    private bool leftStartingSide;

    private bool isShuttingDown;

    private float leftBorder;
    private float rightBorder;
    private float bottomBorder;
    private float topBorder;

    private enum EnemyType
    {
        A,
        B,
        Boss //Maybe ?
    }

    private EnemyType enemyType;

    // Use this for initialization
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        CalculateScreenBorder();
        GetParent();
        GetAudioSources();
        GetPlayer();
        nextFire = 10.0f;   //First Fire after Spawn
        initialTime = Time.time;
        StartingSide();
        StartCoroutine(Shoot());
    }

    private void GetParent()
    {
        waveController = GetComponentInParent<Wave>();
    }

    //For EnemyB Types
    private void StartingSide()
    {
        if (transform.position.x < 0)
        {
            leftStartingSide = true;
            return;
        }
        leftStartingSide = false;
    }

    void OnDestroy()
    {
        if (!isShuttingDown)
        {
           //Moved to Died
        }
    }

    public void Died()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(explosionAudioClip, transform.position);
        switch (enemyType)
        {
            case EnemyType.A:
                waveController.ChildDied(100);
                break;
            case EnemyType.B:
                waveController.ChildDied(200);
                break;
        }
        Destroy(gameObject);
    }

    void OnApplicationQuit()
    {
        isShuttingDown = true;  //Disable Creating objects when the application is closed.
    }

    private void GetPlayer()
    {
        playerGameObject = GameObject.FindWithTag("Player");
    }

    private void GetAudioSources()
    {
        fireAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
        if (transform.position.y < bottomBorder)
            Destroy(gameObject);
    }

    private void Move()
    {
        switch (enemyType)
        {
            case EnemyType.A:
                AMove();
                break;
            case EnemyType.B:
                BMove();
                //TODO Face Player Always.
                break;
            case EnemyType.Boss:
                break;
        }
    }

    private void BMove()
    {
        float x = leftBorder + Mathf.PingPong(Time.time-initialTime, rightBorder*2); //Because 0 is the middle
        if (!leftStartingSide)
            x = -x;
        transform.localPosition = new Vector3(x,transform.localPosition.y-speed*Time.deltaTime,0);
    }

    private void AMove()
    {
        _rigidbody2D.velocity = -Vector2.up*speed;
    }

    private IEnumerator Shoot()
    {
        while (true)
        {
            switch (enemyType)
            {
                case EnemyType.A:
                    AShooting();
                    break;
                case EnemyType.B:
                    BShooting();
                    break;
                case EnemyType.Boss:
                    break;
            }

            float shootingWaitTime = nextFire - Time.time;
            if (shootingWaitTime <= 0)  //While Out Of bounds.
            {
                shootingWaitTime = 1;
            }

            yield return new WaitForSeconds(shootingWaitTime);
        }
    }

    private void AShooting()
    {
        if (InBounds()) 
        {
            nextFire = Time.time + fireRate;
            fireAudioSource.Play();

            GameObject BulletObject =
                Instantiate(bulletPrefab, spawnLocation.transform.position, spawnLocation.transform.rotation) as GameObject;
            BulletObject.tag = "EnemyBullet";
            Bullet bullet = BulletObject.GetComponent<Bullet>();
            bullet.speed = 1.5f;
            bullet.SetDirection(-Vector2.up);
        }
    }

    private bool InBounds()
    {
        if (transform.position.x > leftBorder && transform.position.x < rightBorder &&
            transform.position.y > bottomBorder && transform.position.y < topBorder)
        {
            return true;
        }
        return false;
    }

    private void BShooting()
    {
        if (InBounds()) 
        {
            nextFire = Time.time + fireRate;
            fireAudioSource.Play();

            GameObject BulletObject =
                Instantiate(bulletPrefab, spawnLocation.transform.position, spawnLocation.transform.rotation) as GameObject;
            BulletObject.tag = "EnemyBullet";
            Bullet bullet = BulletObject.GetComponent<Bullet>();
            bullet.speed = 1.5f;
            bullet.SetTargetCoordinate(playerGameObject.transform.position);
        }
    }

    public void SetEnemyType(String type)
    {
        switch (type)
        {
            case "A":
                enemyType = EnemyType.A;
                break;
            case "B":
                enemyType = EnemyType.B;
                break;
            case "Boss":    //Maybe...
                enemyType = EnemyType.Boss;
                break;
        }
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
