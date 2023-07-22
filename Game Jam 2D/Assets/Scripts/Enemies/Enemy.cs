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

        StartCoroutine(CheckFlip());
    }

    public void TakeDamage(int damage)
    {
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
            Debug.Log(_LoS.transform.rotation.eulerAngles.z);
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
