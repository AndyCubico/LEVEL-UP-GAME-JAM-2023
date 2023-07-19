using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetreatAndAtkAI : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float _speed;
    private GameObject _target;
    [SerializeField] private float _retreatDist; 

    public EnemyState state;
    private Transform _targetPos;

    // Line of sight
    [SerializeField] private GameObject _LoS;
    private Transform _LoS_Transform;
    [SerializeField] private float _rotationSpeed = 0; // 0 = no rotation
    [SerializeField] private float _visionDistance = 1; // Eye of Sight X
    [SerializeField] private float _visionRange = 1; // Eye of Sight Y

    //private EoS_CollisionsManager collisionsManager;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        _target = GameObject.Find("Player");
        _targetPos = _target.GetComponent<Transform>();
        _LoS_Transform = _LoS.GetComponent<Transform>();

        _LoS.GetComponent<PolygonCollider2D>().points[1].Set(_visionDistance, _visionRange);
        _LoS.GetComponent<PolygonCollider2D>().points[2].Set(_visionDistance, -_visionRange);

        //collisionsManager = _LoS.GetComponent<EoS_CollisionsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug();

        switch (GetComponentInChildren<EoS_CollisionsManager>().coll_state)
        {
            case EOS_COLLISION.ON_COLLISION_ENTER:
                state = EnemyState.ATTACK;
                break;
            case EOS_COLLISION.ON_COLLISION_STAY:
                if (Vector2.Distance(transform.position, _targetPos.position) < _retreatDist)
                {
                    //Debug.Log("Retreat");
                    state = EnemyState.RETREAT;
                }
                else
                {
                    //Debug.Log("Shoot");
                    state = EnemyState.ATTACK;
                }
                break;
            case EOS_COLLISION.ON_COLLISION_EXIT:
                //Debug.Log("Exit vision range");
                state = EnemyState.IDLE;
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case EnemyState.IDLE:
                StartCoroutine(RotateEyeOfSight());
                break;
            case EnemyState.ATTACK:
                ActiveRaycast();
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

    private void Debug()
    {
        if (Input.GetKeyDown(KeyCode.F7))
        {
            //Debug.Log("Enemy RS State: " + state);
            //Debug.Log("EoS State: " + collisionsManager.coll_state);
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
        RaycastHit2D hitInfo = Physics2D.Raycast(_LoS_Transform.position, _targetPos.position, _visionDistance);

        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.tag == "Player")
            {
                //Debug.DrawLine(_LoS_Transform.position, hitInfo.point, Color.red);
                // Shoot
            }
            else
            {
                //Debug.DrawLine(_LoS_Transform.position, hitInfo.point, Color.yellow);
            }
        }
    }

    IEnumerator RotateEyeOfSight()
    {
        if (_LoS_Transform.transform.localRotation.eulerAngles.z < 75)
        {
            _LoS_Transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        else if (_LoS_Transform.transform.localRotation.eulerAngles.z > 0)
        {
            _LoS_Transform.Rotate(Vector3.back * _rotationSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
