using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private Animator meleeEnemyAnimator;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int attackDamage = 5;
    public float _atkCD = 1.0f;
    public float nextAttackTime = 0f;
    public bool atkOn = false;
    public bool targetFound = false;

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

    public void Attack()
    {
        Debug.Log("ATACADO");

        // [Andy] Play animation
        //meleeEnemyAnimator.SetTrigger("Attack");

        // [Andy] Detect enemies
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);

        nextAttackTime = _atkCD;
        hitPlayer.GetComponent<PlayerCombat>().TakeDamage(attackDamage);// [Andy] damage reduction whit energy remaining
       
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}