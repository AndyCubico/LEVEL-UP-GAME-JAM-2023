using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float _bulletSpeed = 10.0f;
    [SerializeField] private float bulletLifetime = 2.0f;
    public Vector2 target;
    private Rigidbody2D rb;

    //Timer
    private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = (Vector2)transform.position;
        target -= (Vector2)GameObject.Find("Boss").transform.position;
        target.Normalize();
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (bulletLifetime < timer)
        {
            DeleteBullet();
            timer = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition((Vector2)transform.position + (target * _bulletSpeed * Time.deltaTime));
    }

    public void DeleteBullet()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        Destroy(GetComponent<SpriteRenderer>().gameObject);
        Destroy(GetComponent<CircleCollider2D>().gameObject);
        Destroy(GetComponent<Rigidbody2D>().gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerCombat>().TakeDamage(10);
            DeleteBullet();
        }

        if (collision.gameObject.layer == 11 || collision.gameObject.layer == 10)
        {
            DeleteBullet();
        }
    }
}
