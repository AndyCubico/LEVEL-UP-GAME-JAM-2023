using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject enemySPrefab;
    [SerializeField] private GameObject enemyLPrefab;
    [SerializeField] private GameObject barr;
    [SerializeField] private int minSecSpawn = 2;
    [SerializeField] private int maxSecSpawn = 5;
    [SerializeField] private float completeBar = 0.3f;
    [SerializeField] private float completeBarSpeed = 0.0001f;
    [SerializeField] private float timer;
    [SerializeField] private bool StartSpawn = false;
    [SerializeField] private bool Purif = false;
    [SerializeField] private bool Saved = false;
    private float x, y;
    float x_ = 0;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        Spawn();
        Purify();
    }

    private void Purify()
    {

        if (Purif && Saved == false)
        {
            if (Input.GetKey("e"))
            {
                x_ += completeBarSpeed;
                barr.transform.localScale = new Vector3(x_, 0.0625f, 0);
                barr.transform.position = new Vector3(GameObject.Find("Player").transform.position.x, GameObject.Find("Player").transform.position.y - 2, 0);
            }
            if (x_ >= completeBar)
            {
                Saved = true;
            }
        }
        else 
        {
            x_ = 0;
            barr.transform.localScale = new Vector3 (0, 0.0625f, 0);
        }

    }

    private void Spawn()
    {
        if (StartSpawn && timer >= Random.RandomRange(minSecSpawn,maxSecSpawn) && Saved == false)
        {
            GetXY();
            switch (Random.RandomRange(0, 4))
            {
                case 0:
                    Instantiate(enemyLPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    break;
                default:
                    Instantiate(enemySPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    break;
            }

            timer = 0;
        }
    }

    private void GetXY()
    {
        x = Random.RandomRange(GameObject.Find("Player").transform.position.x - 5, GameObject.Find("Player").transform.position.x + 5);
        y = Random.RandomRange(GameObject.Find("Player").transform.position.y - 5, GameObject.Find("Player").transform.position.y + 5);

        if ((x <= transform.position.x - 9 || x >= transform.position.x + 9) || (y <= transform.position.y - 9 || y >= transform.position.y + 9) && (x <= GameObject.Find("Player").transform.position.x - 1 || x >= GameObject.Find("Player").transform.position.x + 1) || (y <= GameObject.Find("Player").transform.position.y - 2 || y >= GameObject.Find("Player").transform.position.y + 2))
        {

        }
        else{ GetXY(); }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartSpawn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartSpawn = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Purif = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Purif = false;
        }
    }
}
