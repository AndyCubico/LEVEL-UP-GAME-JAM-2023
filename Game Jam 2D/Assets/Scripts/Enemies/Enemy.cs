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
        //GetComponent<RetreatAndAtkAI>().enabled = false; [Andy] Aquí no va
        //This.enabled = false;
    }
}
