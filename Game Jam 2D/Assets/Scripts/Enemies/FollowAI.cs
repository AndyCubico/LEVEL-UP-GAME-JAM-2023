using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [Smm]

public class FollowAI : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private GameObject _target;
    [SerializeField] private float _doAtkRange;

    [SerializeField] private bool _follow;
    private Transform _targetPos;

    // Line of sight
    [SerializeField] private GameObject _LoS;
    private Transform _LoS_Transform;
    [SerializeField] private float _rotationSpeed = 0; // 0 = no rotation
    [SerializeField] private float _visionDistance = 1;


    // Start is called before the first frame update
    void Start()
    {
        _targetPos = _target.GetComponent<Transform>();
        _LoS_Transform = _LoS.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, _targetPos.position) < _doAtkRange)
        {
            // TO DO: call atk function
        }
        else if (_follow)
        {
            transform.position = Vector2.MoveTowards(transform.position, _targetPos.position, _speed * Time.deltaTime);
        }

        // Line of Sight
        if (!_follow)
        {
            _LoS_Transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
        }

        RaycastHit2D hitInfo = Physics2D.Raycast(_LoS_Transform.position, _LoS_Transform.right, _visionDistance);

        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.tag == "Player")
            {
                Debug.DrawLine(_LoS_Transform.position, hitInfo.point, Color.red);
                _follow = true;
            }
            else
            {
                Debug.DrawLine(_LoS_Transform.position, hitInfo.point, Color.yellow);
            }    
        }
        else
        {
            Debug.DrawLine(_LoS_Transform.position, _LoS_Transform.position + _LoS_Transform.right * _visionDistance, Color.green);
            _follow = false;
        }
    }
}
