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
    BOSS_MODE_SUN,
    BOSS_MODE_BULLET,
    BOSS_MODE_OBSTACLE,
    BOSS_MODE_DASH,
}
public class BossBehaviour : MonoBehaviour
{
    [SerializeField] private LayerMask layerBossCanNotWalk;
    [SerializeField] private LayerMask layerSunCanNotSpawn;
    [SerializeField] private LayerMask layerEnemiesCanSpawnOn;

    [SerializeField] private TrailRenderer _dashTrail;

    [SerializeField] private BOSS_STATE boss_State;
    [SerializeField] public BOSS_MODE boss_Mode;

    [SerializeField] private GameObject smallBullet;
    [SerializeField] private GameObject bigBullet;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject sun;
    [SerializeField] private GameObject dark;

    [SerializeField] private GameObject enemySPrefab;
    [SerializeField] private GameObject enemyLPrefab;

    [SerializeField] private GameObject obstacle;

    [SerializeField] private float minTimeToWalk;
    [SerializeField] private float maxTimeToWalk;
    [SerializeField] private float bossSpeed;
    [SerializeField] private float MaxHP;
    [SerializeField] private float CurrentHP;

    //Which bullethell mode
    private int num;

    //Change Mode Timer Parameters
    private float maxModeTimer = 15.0f;
    private float minModeTimer = 15.0f;

    //Timers
    private float timer;
    private float ModeTimer;
    private float ShieldTimer;
    private float Globaltimer;
    private float TimerBullet;

    //Shield
    private bool getRid;
    private bool startShield;
    private bool endShield;

    //Move
    private Vector2 pos;
    private Vector2 EnemyPos;

    //Bullets
    private int angulo = 0;
    private float x = 20;

    private float a = 0.01f;
    private int maxEnemySpawn = 0;
    private int maxObstaclesSpawn = 0;

    private void Start()
    {
        boss_State = BOSS_STATE.BOSS_STATE_100;
        boss_Mode = BOSS_MODE.BOSS_MODE_BULLET;
        pos = new Vector2(Random.Range(-1, 1.0f), Random.Range(-1, 1.0f));
        pos.Normalize();

        CurrentHP = MaxHP = 500;
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentHP > MaxHP / 2)
        {
            boss_State = BOSS_STATE.BOSS_STATE_100;
            bossSpeed = 2;
        }
        else if (CurrentHP <= MaxHP / 2 && CurrentHP > MaxHP / 5)
        {
            boss_State = BOSS_STATE.BOSS_STATE_50;
            bossSpeed = 4;
        }
        else if (CurrentHP < MaxHP / 5 && CurrentHP > 0)
        {
            boss_State = BOSS_STATE.BOSS_STATE_25;
            bossSpeed = 6;
        }
        else
        {
            boss_State = BOSS_STATE.BOSS_STATE_DEAD;
        }

        switch (boss_State)
        {
            case BOSS_STATE.BOSS_STATE_NONE:
                break;
            case BOSS_STATE.BOSS_STATE_100:
                Behavior_1();
                smallBullet.GetComponent<BossBullet>()._bulletSpeed = 5f;
                bigBullet.GetComponent<BossBullet>()._bulletSpeed = 5f;
                break;
            case BOSS_STATE.BOSS_STATE_50:
                Behavior_2();
                smallBullet.GetComponent<BossBullet>()._bulletSpeed = 5f;
                bigBullet.GetComponent<BossBullet>()._bulletSpeed = 5f;
                break;
            case BOSS_STATE.BOSS_STATE_25:
                Behavior_3();
                smallBullet.GetComponent<BossBullet>()._bulletSpeed = 8f;
                bigBullet.GetComponent<BossBullet>()._bulletSpeed = 10f;
                break;
            case BOSS_STATE.BOSS_STATE_DEAD:
                DeleteBoss();
                break;
        }

        if (getRid)
        {
            GetRidOfHim();
        }
    }

    private void Behavior_1()
    {
        timer += Time.deltaTime;
        Globaltimer += Time.deltaTime;
        ModeTimer += Time.deltaTime;
        TimerBullet += Time.deltaTime;
        Darker();

        if (ModeTimer >= Random.Range(minModeTimer, maxModeTimer))
        {
            if (boss_Mode == BOSS_MODE.BOSS_MODE_SUN) { ChooseMode(true); }
            else { ChooseMode(false); }
            ModeTimer = 0;
            Globaltimer = 0;
            TimerBullet = 0;

            if (boss_Mode == BOSS_MODE.BOSS_MODE_SUN)
            {
                num = Random.Range(0, 2);
                switch (num)
                {
                    case 0:
                        PointSun();
                        break;
                    case 1:
                        HaloSun();
                        break;
                }
            }
            else if (boss_Mode == BOSS_MODE.BOSS_MODE_DASH)
            {
                StartCoroutine(Dash());
            }
            else if (boss_Mode == BOSS_MODE.BOSS_MODE_OBSTACLE)
            {
                maxObstaclesSpawn = 15;
            }
        }

        switch (boss_Mode)
        {
            case BOSS_MODE.BOSS_MODE_BULLET:
                MoveBoss();

                maxModeTimer = 10.0f;
                minModeTimer = 5.0f;

                if (Globaltimer >= Random.Range(5f, 15f))
                {
                    num = Random.Range(0, 4);
                    Globaltimer = 0;
                    angulo = 0;
                }
                BulletMode();
                break;

            case BOSS_MODE.BOSS_MODE_SUN:
                MoveBossRest();

                maxModeTimer = 15.0f;
                minModeTimer = 10.0f;
                if (CurrentHP < MaxHP)
                {
                    CurrentHP += 1 * Time.deltaTime;
                }

                break;
            case BOSS_MODE.BOSS_MODE_DASH:

                maxModeTimer = 5.0f;
                minModeTimer = 5.0f;

                break;
            case BOSS_MODE.BOSS_MODE_OBSTACLE:
                MoveBoss();

                maxModeTimer = 5.0f;
                minModeTimer = 10.0f;
                SpawnObstacles();

                break;
            default:
                MoveBoss();
                break;

        }

        if (timer >= Random.Range(minTimeToWalk, maxTimeToWalk))
        {
            pos = new Vector2(Random.Range(-1, 1.0f), Random.Range(-1, 1.0f));
            pos.Normalize();
            timer = 0;
        }
    }

    private void Behavior_2()
    {
        timer += Time.deltaTime;
        Globaltimer += Time.deltaTime;
        ModeTimer += Time.deltaTime;
        TimerBullet += Time.deltaTime;
        Darker();

        if (ModeTimer >= Random.Range(minModeTimer, maxModeTimer))
        {
            if (boss_Mode == BOSS_MODE.BOSS_MODE_SUN) { ChooseMode(true); }
            else { ChooseMode(false); }
            ModeTimer = 0;
            Globaltimer = 0;
            TimerBullet = 0;

            if (boss_Mode == BOSS_MODE.BOSS_MODE_SUN)
            {
                num = Random.Range(0, 2);
                switch (num)
                {
                    case 0:
                        PointSun();
                        break;
                    case 1:
                        HaloSun();
                        break;
                }
            }
            else if (boss_Mode == BOSS_MODE.BOSS_MODE_DASH)
            {
                StartCoroutine(Dash());
            }
            else if (boss_Mode == BOSS_MODE.BOSS_MODE_OBSTACLE)
            {
                maxObstaclesSpawn = 15;
            }
        }

        switch (boss_Mode)
        {
            case BOSS_MODE.BOSS_MODE_BULLET:
                MoveBoss();

                maxModeTimer = 20.0f;
                minModeTimer = 15.0f;

                if (Globaltimer >= Random.Range(5f, 15f))
                {
                    num = Random.Range(0, 4);
                    Globaltimer = 0;
                    angulo = 0;
                }
                BulletMode();
                break;

            case BOSS_MODE.BOSS_MODE_SUN:
                MoveBossRest();

                maxModeTimer = 10.0f;
                minModeTimer = 5.0f;
                if (CurrentHP < MaxHP)
                {
                    CurrentHP += 5 * Time.deltaTime;
                }

                break;
            case BOSS_MODE.BOSS_MODE_DASH:

                maxModeTimer = 10.0f;
                minModeTimer = 10.0f;
                if (TimerBullet >= Random.Range(0.2f, 0.6f))
                {
                    angulo += 25;
                    BulletSurrounded(angulo, 4, smallBullet);
                    TimerBullet = 0;
                }
                break;
            case BOSS_MODE.BOSS_MODE_OBSTACLE:
                MoveBoss();

                maxModeTimer = 5.0f;
                minModeTimer = 10.0f;
                SpawnObstacles();

                break;
            default:
                MoveBoss();
                break;

        }

        if (timer >= Random.Range(minTimeToWalk, maxTimeToWalk))
        {
            pos = new Vector2(Random.Range(-1, 1.0f), Random.Range(-1, 1.0f));
            pos.Normalize();
            timer = 0;
        }
    }

    private void Behavior_3()
    {
        timer += Time.deltaTime;
        Globaltimer += Time.deltaTime;
        ModeTimer += Time.deltaTime;
        TimerBullet += Time.deltaTime;
        Darker();

        if (ModeTimer >= Random.Range(minModeTimer, maxModeTimer))
        {
            if (boss_Mode == BOSS_MODE.BOSS_MODE_SUN) { ChooseMode(true); }
            else { ChooseMode(false); }
            ModeTimer = 0;
            Globaltimer = 0;
            TimerBullet = 0;

            if (boss_Mode == BOSS_MODE.BOSS_MODE_SUN)
            {
                num = Random.Range(0, 2);
                switch (num)
                {
                    case 0:
                        PointSun();
                        break;
                    case 1:
                        HaloSun();
                        break;
                }
            }
            else if (boss_Mode == BOSS_MODE.BOSS_MODE_DASH)
            {
                StartCoroutine(Dash());
            }
            else if (boss_Mode == BOSS_MODE.BOSS_MODE_OBSTACLE)
            {
                maxObstaclesSpawn = 15;
            }
        }

        switch (boss_Mode)
        {
            case BOSS_MODE.BOSS_MODE_BULLET:
                MoveBoss();

                maxModeTimer = 20.0f;
                minModeTimer = 15.0f;

                if (Globaltimer >= Random.Range(5f, 15f))
                {
                    num = Random.Range(0, 4);
                    Globaltimer = 0;
                    angulo = 0;
                }
                BulletMode();
                break;

            case BOSS_MODE.BOSS_MODE_SUN:
                MoveBossRest();

                maxModeTimer = 10.0f;
                minModeTimer = 5.0f;
                if (CurrentHP < MaxHP)
                {
                    CurrentHP += 5 * Time.deltaTime;
                }

                break;
            case BOSS_MODE.BOSS_MODE_DASH:

                maxModeTimer = 15.0f;
                minModeTimer = 15.0f;
                if (TimerBullet >= Random.Range(0.2f, 0.6f))
                {
                    angulo += 25;
                    BulletSurrounded(angulo, 8, smallBullet);
                    TimerBullet = 0;
                }
                break;
            case BOSS_MODE.BOSS_MODE_OBSTACLE:
                MoveBoss();

                maxModeTimer = 5.0f;
                minModeTimer = 10.0f;
                SpawnObstacles();

                break;
            default:
                MoveBoss();
                break;

        }

        if (timer >= Random.Range(minTimeToWalk, maxTimeToWalk))
        {
            pos = new Vector2(Random.Range(-1, 1.0f), Random.Range(-1, 1.0f));
            pos.Normalize();
            timer = 0;
        }
    }
    private void ChooseMode(bool SUN)
    {
        if (SUN)
        {
             boss_Mode = (BOSS_MODE)Random.Range(1, 4);
        }
        else 
        {
            boss_Mode = (BOSS_MODE)Random.Range(1, 4);
        }
    }
    private void BulletMode()
    {
        if (boss_State != BOSS_STATE.BOSS_STATE_25)
        {
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
        }
        else 
        {
            switch (num)
            {
                case 0:
                    if (TimerBullet >= Random.Range(0.1f, 0.3f))
                    {
                        angulo += 75;
                        BulletSurrounded(angulo, 10, smallBullet);
                        TimerBullet = 0;
                    }
                    break;
                case 1:
                    if (TimerBullet >= Random.Range(0.1f, 0.3f))
                    {
                        angulo += 25;
                        BulletSurrounded(angulo, 10, smallBullet);
                        TimerBullet = 0;
                    }
                    break;
                case 2:
                    if (TimerBullet >= Random.Range(0.2f, 0.5f))
                    {
                        angulo += 10;
                        BulletSurrounded(angulo, 6, bigBullet);
                        TimerBullet = 0;
                    }
                    break;
                case 3:
                    if (TimerBullet >= Random.Range(0.2f, 0.5f))
                    {
                        angulo += 45;
                        BulletSurrounded(angulo, 6, bigBullet);
                        TimerBullet = 0;
                    }
                    break;
            }
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
        GetComponentInParent<Rigidbody2D>().position += pos * bossSpeed * Time.deltaTime;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(GetComponentInParent<Rigidbody2D>().position, 2.5f);

        bool isInvalidCollision = false;
        foreach (Collider2D collider in colliders)
        {
            if (((1 << collider.gameObject.layer) & layerBossCanNotWalk) != 0)
            {
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

    private void MoveBossRest()
    {
        if (GetComponentInParent<Rigidbody2D>().position.y <= 14)
        {
            pos = (Vector2)GetComponentInParent<Rigidbody2D>().position + new Vector2(0, 14);
            pos.Normalize();
            pos.x = -pos.x;
            GetComponentInParent<Rigidbody2D>().position += pos * bossSpeed * Time.deltaTime;
        }
    }

    // [Smm] Dashing: If dash, multiply the speed by the dash power. When finish, divide to return to the original speed.
    IEnumerator Dash()
    {
        Vector2 speed;
        while (boss_Mode == BOSS_MODE.BOSS_MODE_DASH)
        {

            speed = (Vector2)GameObject.Find("Player").GetComponent<Transform>().position - (Vector2)transform.position;
            speed.Normalize();

            GetComponentInParent<Rigidbody2D>().velocity = speed * 20;

            // [Smm] Pauses the function in this exact line and the next frame or when the time to wait is over, it continues from here. (creo)
            // SDL Delay pero bien, que no peta todo el juego
            yield return new WaitForSeconds(0.5f);

            GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0);

            yield return new WaitForSeconds(1f);
        }
    }

    private void GetRidOfHim()
    {
        if (!startShield && !endShield)
        {
            switch (Random.Range(0, 300))
            {
                case 0:
                    startShield = true;
                    shield.SetActive(true);
                    break;

                default:
                    break;
            }
        }
        if (startShield)
        {
            x += 2 * Time.deltaTime;
            shield.transform.localScale += new Vector3(1.0f * x * Time.deltaTime, 0.62f * x * Time.deltaTime, 0);

            if (shield.transform.localScale.x >= 8.0f) { endShield = true; startShield = false; x = 2; }
        }

        if (endShield)
        {
            ShieldTimer += Time.deltaTime;

            if (ShieldTimer >= Random.Range(1, 2))
            {
                shield.transform.localScale -= new Vector3(1.0f * x * Time.deltaTime, 0.62f * x * Time.deltaTime, 0);
            }

            if (shield.transform.localScale.x <= 1.0f)
            {
                ShieldTimer = 0.0f;
                x = 20;
                endShield = false;
                getRid = false;
                shield.SetActive(false);
            }
        }

    }

    private void Darker()
    {
        if (a <= 0.5 && boss_Mode == BOSS_MODE.BOSS_MODE_SUN)
        {
            SpriteRenderer colorin = dark.GetComponent<SpriteRenderer>();
            a += a * Time.deltaTime;
            colorin.color = new Color(0, 0, 0, a);

        }else if (a > 0.01f && boss_Mode != BOSS_MODE.BOSS_MODE_SUN)
        {
            SpriteRenderer colorin = dark.GetComponent<SpriteRenderer>();
            a -= a * Time.deltaTime;
            colorin.color = new Color(0, 0, 0, a);
        }
    }

    private void PointSun()
    {
        int posx = 0, posy = 0;
        bool ret = false;

        sun.GetComponent<InvertSun>().Radius = Random.Range(3, 6);
        sun.GetComponent<InvertSun>().RadiusDiff = 0;

        while (!ret)
        {
            posx = Random.Range(-4, 4);
            posy = Random.Range(-4, 1);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(posx, posy), sun.GetComponent<InvertSun>().Radius);

            bool isInvalidCollision = false;
            foreach (Collider2D collider in colliders)
            {
                if (((1 << collider.gameObject.layer) & layerSunCanNotSpawn) != 0)
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

        Instantiate(sun, new Vector3(posx, posy, 0), Quaternion.identity);

    }

    private void HaloSun()
    {
        sun.GetComponent<InvertSun>().Radius = Random.Range(10, 16);
        sun.GetComponent<InvertSun>().RadiusDiff = Random.Range(sun.GetComponent<InvertSun>().Radius - 5, sun.GetComponent<InvertSun>().Radius - 2);

        Instantiate(sun, Vector3.zero, Quaternion.identity);
    }

    private void Spawn()
    {
        while (maxEnemySpawn > 0)
        {
            if (IsSpawnable())
            {
                switch (Random.Range(0, 4))
                {
                    case 0:
                        Instantiate(enemyLPrefab, new Vector3(EnemyPos.x, EnemyPos.y, 0), Quaternion.identity);
                        break;
                    default:
                        Instantiate(enemySPrefab, new Vector3(EnemyPos.x, EnemyPos.y, 0), Quaternion.identity);
                        break;
                }
                maxEnemySpawn--;
            }
        } 
    }

    private void SpawnObstacles()
    {
        while (maxObstaclesSpawn > 0)
        {
            if (IsSpawnableObstacle())
            {
                maxObstaclesSpawn--;
                Instantiate(obstacle, new Vector3(EnemyPos.x, EnemyPos.y, 0), Quaternion.identity);
            }
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
            EnemyPos.x = Random.Range(-15, 15);
            EnemyPos.y = Random.Range(-15, 15);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(EnemyPos.x, EnemyPos.y), 3.5f);

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

            attemptCount++;
        }

        return ret;
    }

    private bool IsSpawnableObstacle()
    {
        Vector2 spawnPosition = Vector2.zero;
        bool ret = false;
        int attemptCount = 0;
        int attemptMax = 200;

        while (!ret && attemptCount < attemptMax)
        {
            EnemyPos.x = Random.Range(-15, 15);
            EnemyPos.y = Random.Range(-15, 15);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(EnemyPos.x, EnemyPos.y), 2f);

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

            attemptCount++;
        }

        return ret;
    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        //enemyAnimator.SetTrigger("Hurt");
        Debug.Log("Boss Vida " + CurrentHP);
    }

    private void DeleteBoss()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        Destroy(GetComponent<SpriteRenderer>().gameObject);
        Destroy(GetComponent<CircleCollider2D>().gameObject);
        Destroy(GetComponent<BoxCollider2D>().gameObject);
        Destroy(GetComponent<Rigidbody2D>().gameObject);
        Destroy(shield);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            getRid = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && startShield == false && endShield == false)
        {
            getRid = false;
        }
    }
}
