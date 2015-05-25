using UnityEngine;
using System.Collections;

public class DestroyByTime : MonoBehaviour
{

    [SerializeField] private float lifeTime;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
