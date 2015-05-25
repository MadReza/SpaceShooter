using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [SerializeField] public float speed = 3;

    private Rigidbody2D _rigidbody2D;
    private GameController gameController;

    private int wrapped = 0;

    private float leftBorder;
    private float rightBorder;
    private float bottomBorder;
    private float topBorder;

    private GameController.Difficulty gameDifficulty;

	// Use this for initialization
	void Start ()
	{
        CalculateScreenBorder();
	    GetGameController();
	    gameDifficulty = gameController.DifficultyLevel;
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
        gameController.AddScore(points);
        enemy.GetComponent<Enemy>().Died();
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
	    if (gameDifficulty == GameController.Difficulty.Normal && OutOfBounds())
	        Destroy(gameObject);
	    else if (OutOfBounds())
	        WrapBullets();
	}

    private void WrapBullets()
    {
        if (wrapped >= 3) //We start at 0
        {
            Destroy(gameObject);
        }
        wrapped++;
        Vector3 temp = transform.position;
        if (temp.x < leftBorder || temp.x > rightBorder)
        {
            temp.x *= -1;

        }
        else if (temp.y > topBorder || temp.y < bottomBorder)
        {
            temp.y *= -1;
        }
        transform.position = temp;
    }

    private bool OutOfBounds()
    {
        if (transform.position.x < leftBorder || transform.position.x > rightBorder ||
            transform.position.y < bottomBorder || transform.position.y > topBorder)
        {
            return true;
        }
        return false;
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
