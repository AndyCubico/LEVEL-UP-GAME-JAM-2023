using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator playerAnimator;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int attackDamage = 40;
    [SerializeField] private int specialDamage = 80;
    [SerializeField] private int specialEnergy = 50;

    [SerializeField] private float attackCD = 2f;
    float nextAttackTime = 0f;

    //barras
    public int maxHealth = 100;
    public int currentHealth;
    public int maxEnergy = 100;
    public int currentEnergy;
    public Bar healthBar;
    public Bar energyBar;


    private void Start()
    {
        playerAnimator = GetComponent<Animator>();

        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
        healthBar.SetMaxValue(maxHealth);
        energyBar.SetMaxValue(maxEnergy);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TakeDamage(20);
        }

        if (nextAttackTime > 0)
        {
            nextAttackTime -= Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0) && nextAttackTime <=0)
        {
            Attack();
            nextAttackTime = attackCD;
        }

        else if(Input.GetMouseButtonDown(1) && nextAttackTime <= 0 && currentEnergy-50>=0)
        {
            SpecialAttack();
            nextAttackTime = attackCD;
        }
    }

    private void Attack()
    {
        //Play animation
        playerAnimator.SetTrigger("Attack");

        //Detect enemies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        //Damage
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hiteado el man " + enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    private void SpecialAttack()
    {
        //Play animation
        playerAnimator.SetTrigger("Attack");//cambiar a otra animacion

        //Detect enemies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        //Damage
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Special Hiteado el man " + enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage(specialDamage);
        }

        //spend energy
        UseEnergy(specialEnergy);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetCurrentValue(currentHealth);
    }

    public void UseEnergy(int energy)
    {
        currentEnergy -= energy;
        energyBar.SetCurrentValue(currentEnergy);
    }
}
