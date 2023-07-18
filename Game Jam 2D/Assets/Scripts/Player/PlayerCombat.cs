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
    [SerializeField] private int normalCost = 5;
    [SerializeField] private int specialCost = 50;

    [SerializeField] private float attackCD = 2f;
    float nextAttackTime = 0f;

    //barras
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxEnergy = 100;
    [SerializeField] private int currentEnergy;
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

        if (Input.GetMouseButtonDown(0) && nextAttackTime <=0 && currentEnergy-normalCost >= 0)
        {
            Attack();
            nextAttackTime = attackCD;
        }

        else if(Input.GetMouseButtonDown(1) && nextAttackTime <= 0 && currentEnergy-specialCost >= 0)
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
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage*(currentEnergy/maxEnergy));
        }

        UseEnergy(normalCost);
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
        UseEnergy(specialCost);
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
