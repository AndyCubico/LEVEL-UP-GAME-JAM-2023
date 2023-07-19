using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetreatAndAtkAI : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float _speed;
    private GameObject _target;
    [SerializeField] private float _retreatDist;

    private Transform _targetPos;

    // Line of sight
    [SerializeField] private GameObject _LoS;
    private Transform _LoS_Transform;
    [SerializeField] private float _rotationSpeed = 0; // 0 = no rotation
    [SerializeField] private float _visionDistance = 1; // Eye of Sight X
    [SerializeField] private float _visionRange = 1; // Eye of Sight Y

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        _target = GameObject.Find("Player");
        _targetPos = _target.GetComponent<Transform>();
        _LoS_Transform = _LoS.GetComponent<Transform>();

        _LoS.GetComponent<PolygonCollider2D>().points[1].Set(_visionDistance, _visionRange);
        _LoS.GetComponent<PolygonCollider2D>().points[2].Set(_visionDistance, -_visionRange);
    }

    // Update is called once per frame
    void Update()
    {
        Debugged();

        switch (GetComponentInChildren<EoS_CollisionsManager>().coll_state)
        {
            case EOS_COLLISION.ON_COLLISION_ENTER:
                //GetComponent<Enemy>().state = EnemyState.ATTACK;
                break;
            case EOS_COLLISION.ON_COLLISION_STAY:
                if (Vector2.Distance(transform.position, _targetPos.position) < _retreatDist)
                {
                    GetComponent<Enemy>().state = EnemyState.RETREAT;
                }
                else
                {
                    GetComponent<Enemy>().state = EnemyState.ATTACK;
                }
                break;
            case EOS_COLLISION.ON_COLLISION_EXIT:
                GetComponent<Enemy>().state = EnemyState.IDLE;
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (GetComponent<Enemy>().state)
        {
            case EnemyState.IDLE:
                RotateEyeOfSight();
                break;
            case EnemyState.ATTACK:
                ActiveRaycast();
                // Do Atk
                break;
            case EnemyState.FOLLOW:
                break;
            case EnemyState.RETREAT:
                rb.MovePosition(rb.position - _target.GetComponent<Rigidbody2D>().position.normalized * _speed * Time.fixedDeltaTime);

                break;
            case EnemyState.DEAD:
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
        Vector2 direction = (_target.GetComponent<Rigidbody2D>().position - rb.position).normalized;
        RaycastHit2D hitInfo = Physics2D.Raycast(_LoS_Transform.position, direction, _visionDistance);

        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.tag == "Player")
            {
                Debug.DrawLine(_LoS_Transform.position, hitInfo.point, Color.red);
                // Shoot
            }
            else
            {
                Debug.DrawLine(_LoS_Transform.position, hitInfo.point, Color.yellow);
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
