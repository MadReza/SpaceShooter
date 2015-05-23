using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float tilt = 10.0f;
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private int playerUpgrade = 0;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject explosion;
    [SerializeField] private AudioClip explosionAudioClip;
    [SerializeField] private List<GameObject> spawnLocations; //0 center, 1 left, 2 right. //TODO make this cleaner...
    
    private Rigidbody2D _rigidbody2D;

    private AudioSource bulletAudioSource;

    private float leftBorder;
    private float rightBorder;
    private float bottomBorder;
    private float topBorder;

    private float nextFire = 0;

    private bool isShuttingDown;

    // Use this for initialization
	void Start ()
	{
	    _rigidbody2D = GetComponent<Rigidbody2D>();
        CalculateScreenBorder();
	    GetAudioSources();
	}

    private void GetAudioSources()
    {
        bulletAudioSource = GetComponent<AudioSource>();
    }

    void OnDestroy()
    {
        if (!isShuttingDown)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(explosionAudioClip, transform.position);
        }
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
	}

    private void Shoot()
    {
        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            bulletAudioSource.Play();
            switch (playerUpgrade)
            {
                case 0: //Spawn Middle
                    Instantiate(bulletPrefab, spawnLocations[0].transform.position, spawnLocations[0].transform.rotation);
                    break;
                case 1: //Spawn Sides
                    Instantiate(bulletPrefab, spawnLocations[1].transform.position, spawnLocations[1].transform.rotation);
                    Instantiate(bulletPrefab, spawnLocations[2].transform.position, spawnLocations[2].transform.rotation);
                    break;
                case 2: //Spawn All 3
                    Instantiate(bulletPrefab, spawnLocations[0].transform.position, spawnLocations[0].transform.rotation);
                    Instantiate(bulletPrefab, spawnLocations[1].transform.position, spawnLocations[1].transform.rotation);
                    Instantiate(bulletPrefab, spawnLocations[2].transform.position, spawnLocations[2].transform.rotation);
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
}
