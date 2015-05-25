using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [SerializeField] public float speed = 3;

    private Rigidbody2D _rigidbody2D;
    private GameController gameController;

    private float leftBorder;
    private float rightBorder;
    private float bottomBorder;
    private float topBorder;

	// Use this for initialization
	void Start ()
	{
        CalculateScreenBorder();
	    GetGameController();
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

    void OnTriggerEnter2D(Collider2D other) //TODO: Make GameObject take care of themselves.
    {
        Debug.Log(other.gameObject.tag);
        switch (other.tag)
        {
            case "EnemyA":
                HitEnemy(other.gameObject, 100);
                break;
            case "EnemyB":
                HitEnemy(other.gameObject, 200);
                break;
            case "Player":
                HitPlayer();
                break;
            default:
                return; 
        }
    }

    private void HitPlayer()
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.GotHit();
        Destroy(gameObject);
    }

    void HitEnemy(GameObject enemy, int points)
    {
        if (tag == "EnemyBullet")
            return;
        gameController.AddScore(100);
        Destroy(enemy);
        Destroy(gameObject);
    }

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.velocity = transform.up * speed;        
    }

    public void SetDirection(Vector2 direction)
    {
        _rigidbody2D.velocity = direction*speed;
    }

    public void SetTargetCoordinate(Vector2 targetLocation)
    {
        Vector3 direction = new Vector3(targetLocation.x, targetLocation.y) - transform.position;
        _rigidbody2D.velocity = direction*speed;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    OutOfBounds();
	}

    private void OutOfBounds()
    {
        if (transform.position.x < leftBorder || transform.position.x > rightBorder ||
            transform.position.y < bottomBorder || transform.position.y > topBorder)
        {
            Destroy(gameObject);
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
