using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private GameObject bullet;
    [SerializeField] private float _atkCD = 1.0f;

    public bool targetFound = false;
    public bool atkOn = false;
    private AudioManager audioMan;
    private AudioSource audioSource;
    [SerializeField] private AudioClip attackClip;
    private Animator enemyAnimator;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioMan = GetComponent<AudioManager>();
        enemyAnimator = GetComponent<Animator>();
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

    public IEnumerator ShootBullet(GameObject prefab, GameObject spawner)
    {
        while (GetComponent<Enemy>().state == EnemyState.ATTACK)
        {
            if (targetFound)
            {
                audioMan.PlayAudio(audioSource, attackClip);
                enemyAnimator.SetTrigger("Attack");
                bullet = Instantiate(prefab, spawner.transform.position, spawner.transform.rotation);
                yield return new WaitForSeconds(_atkCD);
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
