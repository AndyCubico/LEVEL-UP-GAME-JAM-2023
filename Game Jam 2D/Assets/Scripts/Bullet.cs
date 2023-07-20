using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed = 10.0f;
    [SerializeField] private float _bulletLifetime = 2.0f;
    public Vector2 _target;
    private Rigidbody2D _rb;

    //Timer
    private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _target = GameObject.Find("Player").GetComponent<Rigidbody2D>().position;
        _target -= (Vector2)transform.position;
        _target.Normalize();
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (_bulletLifetime < timer)
        {
            Debug.Log("delete");
            DeleteBullet();
            timer = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        _rb.MovePosition((Vector2)transform.position + (_target * _bulletSpeed * Time.deltaTime));
    }
    public void DeleteBullet()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
        Destroy(GetComponent<SpriteRenderer>().gameObject);
        Destroy(GetComponent<CapsuleCollider2D>().gameObject);
        Destroy(GetComponent<Rigidbody2D>().gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DeleteBullet();
    }
}
