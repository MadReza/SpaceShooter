using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 3;

    private float leftBorder;
    private float rightBorder;
    private float bottomBorder;
    private float topBorder;

	// Use this for initialization
	void Start ()
	{
	    GetComponent<Rigidbody2D>().velocity = transform.up*speed;
        CalculateScreenBorder();
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
