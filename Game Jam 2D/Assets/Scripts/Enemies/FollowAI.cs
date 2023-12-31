using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

// [Smm]

public class FollowAI : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float _speed;
    private GameObject _target;
    [SerializeField] private float _doAtkRange;

    private Vector2 _targetPos;

    // Line of sight
    [SerializeField] private GameObject _LoS;
    private Transform _LoS_Transform;
    [SerializeField] private float _rotationSpeed = 0; // 0 = no rotation
    [SerializeField] private float _visionDistance = 1;

    private RaycastHit2D hitInfo;

    private MeleeAttack _enemyAttack;

    //
    NavMeshAgent meshAgent;
    //Animations + sound
    private Animator enemyAnimator;
    private AudioSource audioSource;
    [SerializeField] private AudioClip attackClip;
    private AudioManager audioMan;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        _target = GameObject.Find("Player");
        _LoS_Transform = _LoS.GetComponent<Transform>();

        meshAgent = GetComponent<NavMeshAgent>();
        meshAgent.updateRotation = false;
        meshAgent.updateUpAxis = false;
        _enemyAttack = GetComponent<MeleeAttack>();
        enemyAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioMan = GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (GetComponentInChildren<EoS_CollisionsManager>().coll_state)
        {
            case EOS_COLLISION.ON_COLLISION_ENTER:
                //GetComponent<Enemy>().state = EnemyState.ATTACK;
                break;
            case EOS_COLLISION.ON_COLLISION_STAY:
                if (Vector2.Distance(transform.position, _targetPos) < _doAtkRange)
                {
                    GetComponent<Enemy>().state = EnemyState.ATTACK;
                }
                else
                {
                    GetComponent<Enemy>().state = EnemyState.FOLLOW;
                }
                break;
            case EOS_COLLISION.ON_COLLISION_EXIT:
                GetComponent<Enemy>().state = EnemyState.IDLE;
                break;
        }

        if (_enemyAttack.nextAttackTime > 0)
        {
            _enemyAttack.nextAttackTime -= Time.deltaTime;
        }

        if (Vector2.Distance(transform.position, _targetPos) < _doAtkRange && _enemyAttack.nextAttackTime <= 0)
        {
            enemyAnimator.SetTrigger("Attack");
            audioMan.PlayAudio(audioSource, attackClip);
            _enemyAttack.Attack();
        }
    }

    private void FixedUpdate()
    {
        _targetPos = _target.GetComponent<Rigidbody2D>().position;

        switch (GetComponent<Enemy>().state)
        {
            case EnemyState.IDLE:
                meshAgent.isStopped = true;
                RotateEyeOfSight();
                enemyAnimator.SetBool("isWalking", false);
                break;
            case EnemyState.ATTACK:
                ActiveRaycast();
                GetComponent<Enemy>().RotateToTarget();
                break;
            case EnemyState.FOLLOW:
                //ActiveRaycast();

                //Vector2 vector2 = (_targetPos - (Vector2)transform.position);
                //vector2.Normalize();
                //rb.MovePosition((Vector2)transform.position + (vector2 * _speed * Time.fixedDeltaTime));
                
                enemyAnimator.SetBool("isWalking", true);
                GetComponent<Enemy>().RotateToTarget();

                if ((int)Vector2.Distance(transform.position, _targetPos) <= meshAgent.stoppingDistance) 
                {
                    GetComponent<Enemy>().state = EnemyState.ATTACK;
                }
                else
                {
                    meshAgent.isStopped = false;
                    meshAgent.SetDestination(_targetPos);
                }

                break;
            case EnemyState.RETREAT:
                break;
            case EnemyState.DEAD:
                break;
        }
    }

    private void ActiveRaycast()
    {
        Vector2 direction = (_target.GetComponent<Rigidbody2D>().position - rb.position).normalized;
        hitInfo = Physics2D.Raycast(_LoS_Transform.position, direction, _visionDistance);
        
        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.tag == "Player")
            {
                Debug.DrawLine(_LoS_Transform.position, hitInfo.point, Color.red);
            }
            else
            {
                Debug.DrawLine(_LoS_Transform.position, hitInfo.point, Color.yellow);
            }
        }
    }
    private void RotateEyeOfSight()
    {
        _LoS_Transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
    }

        public void SetTarget(GameObject newTarget)
    {
        _target = newTarget;
    }
    public void SetTargetPlayer()
    {
        _target = GameObject.Find("Player");
    }
}