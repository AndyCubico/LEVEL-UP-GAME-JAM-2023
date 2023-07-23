using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private LayerMask layerEnemiesCanSpawnOn;

    [SerializeField] private GameObject enemySPrefab;
    [SerializeField] private GameObject enemyLPrefab;
    [SerializeField] private GameObject barr;

    [SerializeField] private GameObject purifiedImg;
    [SerializeField] private GameObject corruptedImg;
    [SerializeField] private GameObject interactUI;

    [SerializeField] public FloatSO doors;

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

    private void Start()
    {
        purifiedImg.SetActive(false);
        corruptedImg.SetActive(true);
    }

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
                ExperienceManager.Instance.AddExperience(20000);
                doors.Value++;
            }
        }
        else 
        {
            x_ = 0;
            barr.transform.localScale = new Vector3 (0, 0.0625f, 0);
        }

        if (Saved)
        {
            purifiedImg.SetActive(true);
            corruptedImg.SetActive(false);
        }
    }

    private void Spawn()
    {
        if (StartSpawn && timer >= Random.Range(minSecSpawn,maxSecSpawn) && Saved == false && IsSpawnable())
        {
            switch (Random.Range(0, 4))
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

    private bool IsSpawnable()
    {
        Vector2 spawnPosition = Vector2.zero;
        bool ret = false;
        int attemptCount = 0;
        int attemptMax = 200;

        while (!ret && attemptCount < attemptMax)
        {
            x = Random.Range(GameObject.Find("Player").transform.position.x - 5, GameObject.Find("Player").transform.position.x + 5);
            y = Random.Range(GameObject.Find("Player").transform.position.y - 5, GameObject.Find("Player").transform.position.y + 5);

            if ((x <= transform.position.x - 9 || x >= transform.position.x + 9) || (y <= transform.position.y - 9 || y >= transform.position.y + 9) && (x <= GameObject.Find("Player").transform.position.x - 1 || x >= GameObject.Find("Player").transform.position.x + 1) || (y <= GameObject.Find("Player").transform.position.y - 2 || y >= GameObject.Find("Player").transform.position.y + 2))
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(x, y), 3.5f);

                bool isInvalidCollision = false;
                foreach (Collider2D collider in colliders)
                {
                    if (((1 << collider.gameObject.layer) & layerEnemiesCanSpawnOn) != 0)
                    {
                        Debug.Log(collider.gameObject.layer);
                        isInvalidCollision = true;
                        break;
                    }
                }

                if (!isInvalidCollision)
                {
                    ret = true;
                }
            }

            attemptCount++;
        }

        if (!ret) Debug.LogWarning("Could not find a valid spawn position");

        return ret;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartSpawn = true;
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartSpawn = false;
            interactUI.SetActive(false);
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
