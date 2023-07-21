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
public enum BOSS_MODE
{
    BOSS_MODE_BULLET,
    BOSS_MODE_SUN,
    BOSS_MODE_REST,
    BOSS_MODE_OBSTACLE,
}
public class BossBehaviour : MonoBehaviour
{
    [SerializeField] private LayerMask layerEnemiesCanSpawnOn;
    [SerializeField] private BOSS_STATE boss_State;
    [SerializeField] private BOSS_MODE boss_Mode;

    [SerializeField] private GameObject smallBullet;
    [SerializeField] private GameObject bigBullet;

    [SerializeField] private float minTimeToWalk;
    [SerializeField] private float maxTimeToWalk;
    [SerializeField] private float MaxHP;
    [SerializeField] private float CurrentHP;
    [SerializeField] private int num;

    private float timer;
    private float Globaltimer;
    private float TimerBullet;
    private Vector2 pos;
    private int angulo = 0;

    private void Start()
    {
        boss_State = BOSS_STATE.BOSS_STATE_100;
        boss_Mode = BOSS_MODE.BOSS_MODE_BULLET;
    }

    // Update is called once per frame
    void Update()
    {
        switch (boss_State)
        {
            case BOSS_STATE.BOSS_STATE_NONE:
                break;
            case BOSS_STATE.BOSS_STATE_100:
                Behavior_1();
                break;
            case BOSS_STATE.BOSS_STATE_50:

                break;
            case BOSS_STATE.BOSS_STATE_25:

                break;
            case BOSS_STATE.BOSS_STATE_DEAD:

                break;
        }  
    }

    private void Behavior_1()
    {
        timer += Time.deltaTime;
        Globaltimer += Time.deltaTime;
        TimerBullet += Time.deltaTime;
        MoveBoss();

        switch (boss_Mode)
        {
            case BOSS_MODE.BOSS_MODE_BULLET:

                if (Globaltimer >= Random.Range(5f, 15f))
                {
                    num = Random.Range(0, 4);
                    Globaltimer = 0;
                    angulo = 0;
                }

                switch (num)
                {
                    case 0:
                        if (TimerBullet >= Random.Range(0.2f, 0.6f))
                        {
                            angulo += 75;
                            BulletSurrounded(angulo, 8, smallBullet);
                            TimerBullet = 0;
                        }
                        break;
                    case 1:
                        if (TimerBullet >= Random.Range(0.2f, 0.3f))
                        {
                            angulo += 25;
                            BulletSurrounded(angulo, 8, smallBullet);
                            TimerBullet = 0;
                        }
                        break;
                    case 2:
                        if (TimerBullet >= Random.Range(0.5f, 1.0f))
                        {
                            angulo += 10;
                            BulletSurrounded(angulo, 4, bigBullet);
                            TimerBullet = 0;
                        }
                        break;
                    case 3:
                        if (TimerBullet >= Random.Range(0.5f, 1.0f))
                        {
                            angulo += 45;
                            BulletSurrounded(angulo, 4, bigBullet);
                            TimerBullet = 0;
                        }
                        break;
                }
                break;
        }

        if (timer >= Random.Range(minTimeToWalk, maxTimeToWalk))
        {
            pos = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
            pos.Normalize();
            timer = 0;
        }
    }

    private void BulletSurrounded(int angulo, int Bnum, GameObject prefab)
    {
        for (int i = 0; i < Bnum; i++)
        {
            float angle = 2 * Mathf.PI * i / Bnum;
            float x = 3.5f * Mathf.Cos(angle + angulo) + (transform.position.x);
            float y = 3.5f * Mathf.Sin(angle + angulo) + (transform.position.y);

            Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity);
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
