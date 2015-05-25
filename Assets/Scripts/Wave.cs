using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class Wave : MonoBehaviour
{
    [SerializeField] private GameObject upgradeGameObject;
    private GameController GameController;
    private int totalChildrens = 5;
    private int bonusPoints = 0;

	// Use this for initialization
	void Start ()
	{
	    GameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
	}
	

    public void ChildDied(int points)
    {
        totalChildrens--;
        bonusPoints += points;

        if (totalChildrens <= 0)
        {
            GameController.AddScore(bonusPoints);
            Instantiate(upgradeGameObject, new Vector3(), Quaternion.identity);
        }
    }
}
