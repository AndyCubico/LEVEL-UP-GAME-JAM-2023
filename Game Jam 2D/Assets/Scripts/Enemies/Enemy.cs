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
    private Animator enemyAnimator;

    [SerializeField] private int maxHealth = 100;
    int currentHealth;

    public EnemyState state;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        enemyAnimator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        enemyAnimator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            state = EnemyState.DEAD;
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy ded");

        enemyAnimator.SetBool("IsDed", true);

        GetComponent<Collider2D>().enabled = false;
        GetComponent<FollowAI>().enabled = false;
        this.enabled = false;
    }
}
