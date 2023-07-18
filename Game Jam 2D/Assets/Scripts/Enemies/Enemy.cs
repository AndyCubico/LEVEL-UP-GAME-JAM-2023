using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator enemyAnimator;
    [SerializeField] private int maxHealth = 100;
    int currentHealth;
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

        if (currentHealth<=0)
        {
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
