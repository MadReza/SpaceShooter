using UnityEngine;
using System.Collections;

public class Upgrade : MonoBehaviour
{
    private Player playerGameObject;

	// Use this for initialization
	void Start ()
	{
	    playerGameObject = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            playerGameObject.GotUpgrade();
            Destroy(gameObject);
        }
    }

}
