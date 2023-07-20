using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    private GameObject _bullet;

    // Start is called before the first frame update
    void Start()
    {
        //_rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (GetComponent<Enemy>().state == EnemyState.ATTACK)
        //{
        //    //StartCoroutine(ShootBullet());

        //    _rb.MovePosition((Vector2)transform.position + ( * speed * Time.deltaTime));
        //}
    }

    public IEnumerator ShootBullet(EnemyState state, GameObject prefab, GameObject spawner, float bulletSpeed, Vector2 dir)
    {
        while (state == EnemyState.ATTACK)
        {
            _bullet = Instantiate(prefab, spawner.transform.position, Quaternion.identity /*spawner.transform.rotation*/);
            yield return new WaitForSeconds(0.2f);
            //DeleteBullet();
        }
    }
}