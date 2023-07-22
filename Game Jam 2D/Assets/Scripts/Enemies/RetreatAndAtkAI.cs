using UnityEngine;

public class RetreatAndAtkAI : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float _speed;
    private GameObject _target;
    [SerializeField] private float _retreatDist;

    private Vector2 _targetPos;

    // Line of sight
    [SerializeField] private GameObject _LoS;
    private Transform _LoS_Transform;
    [SerializeField] private float _rotationSpeed = 0; // 0 = no rotation
    [SerializeField] private float _visionDistance = 1; // Eye of Sight X
    [SerializeField] private float _visionRange = 1; // Eye of Sight Y

    // Atk
    private EnemyAttack _enemyAttack;
    [SerializeField] private GameObject _spawnPoint;
    [SerializeField] private GameObject _bulletPrefab;
    private Animator enemyAnimator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        _target = GameObject.Find("Player");
        _LoS_Transform = _LoS.GetComponent<Transform>();

        _LoS.GetComponent<PolygonCollider2D>().points[1].Set(_visionDistance, _visionRange);
        _LoS.GetComponent<PolygonCollider2D>().points[2].Set(_visionDistance, -_visionRange);

        _enemyAttack = GetComponent<EnemyAttack>(); 
        enemyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (GetComponentInChildren<EoS_CollisionsManager>().coll_state)
        {
            case EOS_COLLISION.ON_COLLISION_ENTER:
                break;
            case EOS_COLLISION.ON_COLLISION_STAY:
                if ((int) Vector2.Distance(transform.position, _targetPos) >= (_visionDistance / 2)) // To Do: se la suda bastante :')
                {
                    GetComponent<Enemy>().state = EnemyState.IDLE;
                }

                if (Vector2.Distance(transform.position, _targetPos) < _retreatDist)
                {
                    GetComponent<Enemy>().state = EnemyState.RETREAT;
                }
                else
                {
                    GetComponent<Enemy>().state = EnemyState.ATTACK;
                }
                break;
            case EOS_COLLISION.ON_COLLISION_EXIT:
                //GetComponent<Enemy>().state = EnemyState.IDLE;
                break;
        }
    }

    private void FixedUpdate()
    {
        _targetPos = _target.GetComponent<Rigidbody2D>().position;

        switch (GetComponent<Enemy>().state)
        {
            case EnemyState.IDLE:
                RotateEyeOfSight(); 
                _enemyAttack.DisableCoroutine(_enemyAttack.ShootBullet(_bulletPrefab, _spawnPoint));
                enemyAnimator.SetBool("isWalking", false);

                break;
            case EnemyState.ATTACK:
                ActiveRaycast();
                _enemyAttack.EnableCoroutine(_enemyAttack.ShootBullet(_bulletPrefab, _spawnPoint));

                break;
            case EnemyState.FOLLOW:
                break;
            case EnemyState.RETREAT:

                Vector2 vector2 = (_targetPos - (Vector2)transform.position);
                vector2.Normalize();
                rb.MovePosition((Vector2)transform.position - (vector2 * _speed * Time.fixedDeltaTime));

                //rb.MovePosition(rb.position - _target.GetComponent<Rigidbody2D>().position.normalized * _speed * Time.fixedDeltaTime);
                _enemyAttack.DisableCoroutine(_enemyAttack.ShootBullet(_bulletPrefab, _spawnPoint));
                enemyAnimator.SetBool("isWalking", true);

                break;
            case EnemyState.DEAD:
                _enemyAttack.DisableCoroutine(_enemyAttack.ShootBullet(_bulletPrefab, _spawnPoint));

                break;
        }
    }

    private void Debugged()
    {
        if (Input.GetKeyDown(KeyCode.F7))
        {
            Debug.Log("Enemy RS State: " + GetComponent<Enemy>().state);
            Debug.Log("EoS State: " + GetComponentInChildren<EoS_CollisionsManager>().coll_state);
        }
    }

    public void SetTarget(GameObject newTarget)
    {
        _target = newTarget;
    }
    public void SetTargetPlayer()
    {
        _target = GameObject.Find("Player");
    }

    private void ActiveRaycast()
    { 
        GetComponentInChildren<PolygonCollider2D>().enabled = false;
        Vector2 direction = (_target.GetComponent<Rigidbody2D>().position - rb.position);
        RaycastHit2D hitInfo = Physics2D.Raycast(_LoS_Transform.position, direction.normalized, _visionDistance);

        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.tag == "Player")
            {
                Debug.DrawLine(_LoS_Transform.position, hitInfo.point, Color.red);

                // Shoot
                _enemyAttack.targetFound = true;

                Vector3 vectorToTarget = _target.transform.position - _LoS_Transform.position;
                float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                _LoS_Transform.rotation = Quaternion.Slerp(_LoS_Transform.rotation, q, Time.deltaTime * 2);
            }
            else
            {
                Debug.DrawLine(_LoS_Transform.position, hitInfo.point, Color.yellow);
                _enemyAttack.targetFound = false;                
            }
        }

        GetComponentInChildren<PolygonCollider2D>().enabled = true;
    }

    private void RotateEyeOfSight()
    {
        _LoS_Transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);

        // WIP
        //bool goingCW = false;

        //Debug.Log(_LoS_Transform.rotation.eulerAngles.z + " cw " + goingCW);

        //if (_LoS_Transform.rotation.eulerAngles.z >= 285.0f)
        //{
        //    goingCW = true;
        //}
        //else if (_LoS_Transform.rotation.eulerAngles.z <= 75.0f)
        //{
        //    goingCW = false;
        //}

        //if (goingCW)
        //{
        //    _LoS_Transform.Rotate(Vector3.back * _rotationSpeed * Time.deltaTime);
        //}
        //else
        //{
        //    _LoS_Transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
        //}
    }
}
