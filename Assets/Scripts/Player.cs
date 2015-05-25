using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float tilt = 10.0f;
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private int playerUpgrade = 1; //Also Life. Death at 0
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject explosion;
    [SerializeField] private AudioClip explosionAudioClip;
    [SerializeField] private List<GameObject> spawnLocations; //0 center, 1 left, 2 right. //TODO make this cleaner...
    [SerializeField] private List<GameObject> lifeObjects;
    private List<Renderer> lifes = new List<Renderer>();
    
    private Rigidbody2D _rigidbody2D;
    private Renderer _renderer;

    private GameController gameController;
    private AudioSource fireAudioSource;
    private Vector3 initialSpawn;

    private bool invicible = false;
    private float invicibleStart;
    [SerializeField] private float invicibleLimite = 3.0f;

    private float leftBorder;
    private float rightBorder;
    private float bottomBorder;
    private float topBorder;

    private float nextFire = 0;

    private bool isShuttingDown;

    // Use this for initialization
	void Start ()
	{
	    initialSpawn = transform.position;
	    _rigidbody2D = GetComponent<Rigidbody2D>();
	    _renderer = GetComponent<Renderer>();
        CalculateScreenBorder();
        GetGameController();
	    GetAudioSources();
	    GetLifeBars();
        CheckStatus();
	}
    private void GetGameController()
    {
        GameObject gameControllerGameObject = GameObject.FindWithTag("GameController");
        if (gameControllerGameObject != null)
        {
            gameController = gameControllerGameObject.GetComponent<GameController>();
        }
        else
        {
            Debug.Log("GameController could not be found!");
        }
    }

    private void GetLifeBars()
    {
        foreach (var lifeObject in lifeObjects)
        {
            Renderer renderLife = lifeObject.GetComponent<Renderer>();
            renderLife.enabled = false;
            lifes.Add(renderLife);
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        Destroy(coll.gameObject);
        gameController.AddScore(-300);
        GotHit();
    }

    private void GetAudioSources()
    {
        fireAudioSource = GetComponent<AudioSource>();
    }

    void OnDestroy()
    {
        //if (!isShuttingDown)
        //{
        //    Instantiate(explosion, transform.position, Quaternion.identity);
        //    AudioSource.PlayClipAtPoint(explosionAudioClip, transform.position);
        //}
    }

    void OnApplicationQuit()
    {
        isShuttingDown = true;  //Disable Creating objects when the application is closed.
    }

    void FixedUpdate()
    {
        Move();
        Clamp();
        Tilt();
    }

    private void Clamp()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, leftBorder, rightBorder),
            Mathf.Clamp(transform.position.y, bottomBorder, topBorder),
            0.0f
            );
    }

    // Update is called once per frame
	void Update ()
	{
	    Shoot();
	    if (invicible)
	    {
	        ChangeLayer("Invincible");
	        InitiateInvicibility();
	    }
	}

    private void ChangeLayer(string name)
    {
        if (gameObject.layer != LayerMask.NameToLayer(name))
        {
            gameObject.layer = LayerMask.NameToLayer(name);
        }
    }

    private void InitiateInvicibility()
    {
        float timeRemaining = invicibleLimite - (Time.time - invicibleStart);
        Color colorValues = _renderer.material.color;
        

        if (timeRemaining <= 0)
        {
            colorValues.a = 255;
            _renderer.material.color = colorValues;
            invicible = false;
            ChangeLayer("Player");
            return;
        }

        var lerp = Mathf.PingPong(Time.time, invicibleLimite)/invicibleLimite;
        colorValues.a = Mathf.Lerp(0.0f, 1.0f, lerp);
        _renderer.material.color = colorValues;

    }

    private void Shoot()
    {
        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            fireAudioSource.Play();
            switch (playerUpgrade)
            {
                case 1: //Spawn Middle
                    Instantiate(bulletPrefab, spawnLocations[0].transform.position, spawnLocations[0].transform.rotation);
                    break;
                case 2: //Spawn Sides
                    Instantiate(bulletPrefab, spawnLocations[1].transform.position, spawnLocations[1].transform.rotation);
                    Instantiate(bulletPrefab, spawnLocations[2].transform.position, spawnLocations[2].transform.rotation);
                    break;
                case 3: //Spawn All 3, Angle the 2 sides
                    Instantiate(bulletPrefab, spawnLocations[0].transform.position, spawnLocations[0].transform.rotation);
                    GameObject leftBulletObject = Instantiate(bulletPrefab, spawnLocations[1].transform.position, spawnLocations[1].transform.rotation) as GameObject;
                    GameObject rightBulletObject = Instantiate(bulletPrefab, spawnLocations[2].transform.position, spawnLocations[2].transform.rotation) as GameObject;
                    Bullet leftBullet = leftBulletObject.GetComponent<Bullet>();
                    Bullet rightBullet = rightBulletObject.GetComponent<Bullet>();
                    leftBullet.SetDirection(Vector2.up - new Vector2(0.25f,0));
                    rightBullet.SetDirection(Vector2.up + new Vector2(0.25f, 0));
                    break;
            }
        }
    }

    //Called in FixedUpdate. No need for DeltaTime.
    private void Move()
    {
        // Obtain input information (See "Horizontal" and "Vertical" in the Input Manager)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, vertical, 0.0f);
        _rigidbody2D.velocity = direction*speed;
    }

    private void Tilt()
    {
        transform.rotation = Quaternion.Euler(0.0f, _rigidbody2D.velocity.x * -tilt, 0.0f);
    }

    private void CalculateScreenBorder()
    {
        var dist = (transform.position - Camera.main.transform.position).z;
        var width = GetComponent<Renderer>().bounds.size.x;
        var height = GetComponent<Renderer>().bounds.size.y;
        leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).x + width/2;
        rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x - width/2;
        bottomBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).y + height/2;
        topBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, dist)).y - height/2;
    }

    public void GotHit()
    {
        playerUpgrade--;
        Instantiate(explosion, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(explosionAudioClip, transform.position);
        SpawnPlayer();
        CheckStatus();
        invicible = true;
        invicibleStart = Time.time;
    }

    public void GotUpgrade()
    {
        playerUpgrade++;
        if (playerUpgrade > 3)  //Add MaxUpgrade
        {
            playerUpgrade = 3;
            gameController.AddScore(200);
        }
        CheckStatus();
    }

    private void SpawnPlayer()
    {
        transform.position = initialSpawn;
        //TODO temporary Imortality
    }

    private void CheckStatus()
    {
        switch (playerUpgrade)
        {
            case 0:
                lifes[0].enabled = false;
                GameOver();
                //TODO Show Game Over... Switch To Main Screen
                break;
            case 1:
                lifes[0].enabled = true;
                lifes[1].enabled = false;
                lifes[2].enabled = false;
                break;
            case 2:
                lifes[0].enabled = true;
                lifes[1].enabled = true;
                lifes[2].enabled = false;
                break;
            case 3:
                lifes[0].enabled = true;
                lifes[1].enabled = true;
                lifes[2].enabled = true;
                break;
        }
    }

    private void GameOver()
    {
        Debug.Log("Implement GameOver");
    }
}
