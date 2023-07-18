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
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxEnergy = 100;
    [SerializeField] private float currentEnergy;
    public Bar healthBar;
    public Bar energyBar;

    //Recharge
    private bool canRecharge = true;
    [SerializeField] private float rechargeTime;
    [SerializeField] private float rechargeCooldown;
    [SerializeField] private float rechargeValue;
    public PlayerMovement move;

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
        if (move._stopMove)
        {
            if (currentEnergy<maxEnergy)
            {
                UseEnergy(-rechargeValue);
            }

            return;
        }

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

        else if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Recharge());
        }
    }
    private IEnumerator Recharge()
    {
        // [Andy] Play animation
        playerAnimator.SetTrigger("Recharge");

        canRecharge = false;
        move._stopMove = true;
        move.rechargeStop = true;

        yield return new WaitForSeconds(rechargeTime);

        canRecharge = false;
        move._stopMove = false;
        move.rechargeStop = false;

        yield return new WaitForSeconds(rechargeCooldown);
        canRecharge = true;
    }

    private void Attack()
    {
        // [Andy] Play animation
        playerAnimator.SetTrigger("Attack");

        // [Andy] Detect enemies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // [Andy] Damage
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hiteado el man " + enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage((int)(attackDamage*(currentEnergy/maxEnergy)));// [Andy] damage reduction whit energy remaining
        }

        // [Andy] spend energy
        UseEnergy(normalCost);
    }

    private void SpecialAttack()
    {
        // [Andy] Play animation
        playerAnimator.SetTrigger("Attack");//cambiar a otra animacion

        // [Andy] Detect enemies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // [Andy] Damage
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Special Hiteado el man " + enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage(specialDamage);
        }

        // [Andy] spend energy
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

    public void UseEnergy(float energy)
    {
        currentEnergy -= energy;
        energyBar.SetCurrentValue(currentEnergy);
    }
}
