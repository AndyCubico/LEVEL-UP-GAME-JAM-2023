using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BOSS_STATE
{
    BOSS_STATE_NONE,
    BOSS_STATE_100,
    BOSS_STATE_50,
    BOSS_STATE_25,
    BOSS_STATE_DEAD,
}
public class BossMove : MonoBehaviour
{
    [SerializeField] private LayerMask layerEnemiesCanSpawnOn;

    [SerializeField] private float minTimeToWalk;
    [SerializeField] private float maxTimeToWalk;
    [SerializeField] private float MaxHP;
    [SerializeField] private float CurrentHP;
    private float timer;
    private Vector2 pos;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        MoveBoss();

        if (timer >= Random.Range(minTimeToWalk, maxTimeToWalk))
        {
            pos = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
            pos.Normalize();
            timer = 0;
        }
    }
    private void MoveBoss()
    {
        GetComponentInParent<Rigidbody2D>().position += pos * 2 * Time.deltaTime;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(GetComponentInParent<Rigidbody2D>().position, 3.5f);

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

        if (isInvalidCollision)
        {
            pos = -pos;
            timer = 0;
        }
    }
}
