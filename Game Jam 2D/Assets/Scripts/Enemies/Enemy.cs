using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    IDLE,
    ATTACK,
    FOLLOW,
    RETREAT,
    DEAD
}

public class Enemy : MonoBehaviour
{
    public GameObject FloatingText;

    private GameObject _target;
    private Animator enemyAnimator;
    [SerializeField] private GameObject _LoS;

    [SerializeField] private int maxHealth = 100;
    int currentHealth;
    private AudioSource audioSource;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip dedClip;
    private AudioManager audioMan;
    public EnemyState state;
    public int xpAmount = 100;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioMan = GetComponent<AudioManager>();
        currentHealth = maxHealth;
        enemyAnimator = GetComponent<Animator>();

        _target = GameObject.Find("Player");
        StartCoroutine(CheckFlip());
    }

    public void TakeDamage(int damage)
    {
        
        //state = EnemyState.ATTACK;

        audioMan.PlayAudio(audioSource, hurtClip);
        ShowFloatingText(damage);

        currentHealth -= damage;
        enemyAnimator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            state = EnemyState.DEAD;
            Die();
        }
    }

    private IEnumerator CheckFlip()
    {
        while (state != EnemyState.DEAD)
        {
            if (_LoS.transform.rotation.eulerAngles.z >= 90 && _LoS.transform.rotation.eulerAngles.z <= 270)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void RotateToTarget()
    {
        Vector3 vectorToTarget = _target.transform.position - _LoS.transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        _LoS.transform.rotation = Quaternion.Slerp(_LoS.transform.rotation, q, Time.deltaTime * 2);

        if (_LoS.transform.rotation.eulerAngles.z >= 90 && _LoS.transform.rotation.eulerAngles.z <= 270)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    private void ShowFloatingText(int dam)
    {
        var go = Instantiate(FloatingText, transform.position, Quaternion.identity, transform);
        int daños = dam;
        go.GetComponent<TextMesh>().text = daños.ToString();
    }

    void Die()
    {
        audioMan.PlayAudio(audioSource, dedClip);
        ExperienceManager.Instance.AddExperience(xpAmount);

        Debug.Log("Enemy ded");

        enemyAnimator.SetBool("IsDed", true);

        GetComponent<Collider2D>().enabled = false;
        GetComponent<FollowAI>().enabled = false;
    }
}
