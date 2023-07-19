using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetreatAndAtkAI : MonoBehaviour
{
    [SerializeField] private float _speed;
    private GameObject _target;
    [SerializeField] private float _retreatDist; 

    [SerializeField] private bool _retreat;
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
        _target = GameObject.Find("Player");
        _targetPos = _target.GetComponent<Transform>();
        _LoS_Transform = _LoS.GetComponent<Transform>();

        _LoS.GetComponent<PolygonCollider2D>().points[1].Set(_visionDistance, _visionRange);
        _LoS.GetComponent<PolygonCollider2D>().points[2].Set(_visionDistance, -_visionRange);
    }

    // Update is called once per frame
    void Update()
    {   
        if (_retreat)
        {
            transform.position = Vector2.MoveTowards(transform.position, _targetPos.position, -_speed * Time.deltaTime);
        }

        // Line of Sight
        if (!_retreat)
        {
            StartCoroutine(RotateEyeOfSight());
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
        RaycastHit2D hitInfo = Physics2D.Raycast(_LoS_Transform.position, _LoS_Transform.right, _visionDistance);

        //[Smm] Pruebas para hacer el rayito mas ancho
        //RaycastHit2D hitInfo = Physics2D.BoxCast(_LoS_Transform.position, new Vector2(_rcWidth, _rcHeight), _LoS_Transform.rotation.z, new Vector2(_visionDistance, _visionDistance));
        //RaycastHit2D hitInfo = BoxCast(_LoS_Transform.position, new Vector2(_rcWidth, _rcHeight), _LoS_Transform.rotation.z, new Vector2(_visionDistance, _visionDistance));

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
    }

    IEnumerator RotateEyeOfSight()
    {
        if (_LoS_Transform.transform.rotation.z < 75)
        {
            _LoS_Transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        else if (_LoS_Transform.transform.rotation.z > -75)
        {
            _LoS_Transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (Vector2.Distance(transform.position, _targetPos.position) < _retreatDist)
        //{
        //    Debug.Log("Retreat");
        //    _retreat = true;
        //}
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Vector2.Distance(transform.position, _targetPos.position) < _retreatDist)
        {
            Debug.Log("Retreat");
            _retreat = true;
        }
        else
        {
            _retreat = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }
}
