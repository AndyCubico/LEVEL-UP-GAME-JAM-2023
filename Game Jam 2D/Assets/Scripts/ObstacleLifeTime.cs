using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleLifeTime : MonoBehaviour
{
    [SerializeField] private float lifeTime = 30.0f;

    //Timer
    private float timer = 0.0f;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= lifeTime)
        {
            DeleteBullet();
        }
    }

    public void DeleteBullet()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        Destroy(GetComponent<SpriteRenderer>().gameObject);
        Destroy(GetComponent<BoxCollider2D>().gameObject);
        Destroy(GetComponent<Rigidbody2D>().gameObject);
    }
}
