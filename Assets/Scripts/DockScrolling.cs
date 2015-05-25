using UnityEngine;
using System.Collections;

public class DockScrolling : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0;
    private Vector2 savedOffset;
    private Renderer _renderer;
    private float initialTime;

    // Use this for initialization
    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        savedOffset = _renderer.sharedMaterial.GetTextureOffset("_MainTex");
        initialTime = Time.time;
    }

    // Update is called once per frame
    private void Update()
    {
        scrollSpeed = (Time.time-initialTime)*0.10f;

        if (scrollSpeed < 1.0f)
        {
            float y = Mathf.Repeat((Time.time-initialTime)*scrollSpeed, 1);
            Vector2 offset = new Vector2(savedOffset.x, y);
            _renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y-scrollSpeed, transform.position.z);
        }
        if (transform.position.y < -6.0f)   //TODO change magic number of edge of screen
            Destroy(gameObject);

    }

    void OnDisable()
    {
        _renderer.sharedMaterial.SetTextureOffset("_MainTex", savedOffset);
    }
}
