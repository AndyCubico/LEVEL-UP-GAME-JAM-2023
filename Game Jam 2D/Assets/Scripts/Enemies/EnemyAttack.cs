using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private GameObject bullet;
    [SerializeField] private float _atkCD = 1.0f;

    public bool targetFound = false;
    public bool atkOn = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableCoroutine(IEnumerator enumerator)
    {
        if (!atkOn)
        {
            StartCoroutine(enumerator);
            atkOn = true;
        }
    }
    public void DisableCoroutine(IEnumerator enumerator)
    {
        if (atkOn)
        {
            StopCoroutine(enumerator); 
            targetFound = false;
            atkOn = false;
        }
    }

    public void Shoot(GameObject prefab, GameObject spawner)
    {

        
    }

    public IEnumerator ShootBullet(GameObject prefab, GameObject spawner)
    {
        while (GetComponent<Enemy>().state == EnemyState.ATTACK)
        {
            if (targetFound)
            {
                bullet = Instantiate(prefab, spawner.transform.position, Quaternion.identity);
                yield return new WaitForSeconds(_atkCD);
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
