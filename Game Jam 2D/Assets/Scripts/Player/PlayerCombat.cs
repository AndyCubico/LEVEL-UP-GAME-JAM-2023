using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator playerAnimator;
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask enemyLayers;
    public int attackDamage = 40;
    [SerializeField] private int normalCost = 5;
    [SerializeField] private int specialDamage = 80;
    [SerializeField] private int specialCost = 50;
    [SerializeField] private float attackCD = 2f;
    float nextAttackTime = 0f;

    //barras
    [SerializeField] private int maxXP;
    [SerializeField] private int currentXp;
    [SerializeField] private int currentLvl;
    public int maxHealth = 100;
    public int currentHealth;
    public float maxEnergy = 100;
    public float currentEnergy;
    public Bar healthBar;
    public Bar energyBar;

    //HealthPotion
    private bool canHeal = true;
    public int potionValue;    
    [SerializeField] private float healingCD;
    [SerializeField] private float healingTime;

    public PlayerMovement move;

    //Recharge
    public float rechargeValue;
    [SerializeField] private float rechargeCD = 5.0f;  
    [SerializeField] private float stunRecharge = 1.0f;
    float nextRechargeTime = 0f;

    //Audio
    private AudioSource audioSource;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip specialClip;
    [SerializeField] private AudioClip healClip;
    [SerializeField] private AudioClip rechargeClip;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip dedClip;//TODO

    public AudioManager audioMan;

    // [Andy] cambiar esto, ponerlo en enemigo
    int xpAmount = 100;

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
        healthBar.SetMaxValue(maxHealth);
        energyBar.SetMaxValue(maxEnergy);
    }
    void Update()
    {
        if (LevelUpMenu.isPaused)
        {
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

        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.JoystickButton5)) && nextAttackTime <=0 && currentEnergy-normalCost >= 0)
        {
            Attack();
            nextAttackTime = attackCD;
        }

        else if((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.JoystickButton4)) && nextAttackTime <= 0 && currentEnergy-specialCost >= 0)
        {
            SpecialAttack();
            nextAttackTime = attackCD;
        }

        else if ((Input.GetKeyDown(KeyCode.Q)|| Input.GetKeyDown(KeyCode.JoystickButton1)) && canHeal)
        {
            StartCoroutine(UsePotion());
        }

        // [Andy] Recharge logic
        if (nextRechargeTime > 0)
        {
            nextRechargeTime -= Time.deltaTime;
        }

        if ((Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.JoystickButton3)) && nextRechargeTime <= 0)
        {
            Recharge();
        }

        else if (Input.GetKeyUp(KeyCode.R) || Input.GetKeyUp(KeyCode.JoystickButton3))
        {
            StartCoroutine(Back2normal());
        }
    }
    private void Attack()
    {
        // [Andy] Play animation
        weaponAnimator.SetTrigger("Attack");

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

        audioMan.PlayAudio(audioSource,attackClip);

        // [Andy] poner a enemigos
        ExperienceManager.Instance.AddExperience(xpAmount);
    }

   private void SpecialAttack()
   {
        // [Andy] Play animation
        weaponAnimator.SetTrigger("Attack");//cambiar a otra animacion

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

        audioMan.PlayAudio(audioSource, specialClip);
   }
   public void TakeDamage(int damage)
   {
        currentHealth -= damage;
        healthBar.SetCurrentValue(currentHealth);
        if (damage>0)
        {
            audioMan.PlayAudio(audioSource, hurtClip);
        }
   }

    public void UseEnergy(float energy)
    {
        currentEnergy -= energy;
        energyBar.SetCurrentValue(currentEnergy);
    }

    private void Recharge()
    {
        if (!move._stopMove)
        {
            playerAnimator.SetTrigger("Recharge");
            playerAnimator.SetBool("isCharging", true);
        }
        if (!audioSource.isPlaying)
        {
            audioMan.PlayAudio(audioSource, rechargeClip);
        }
        if (currentEnergy<maxEnergy)
        {
            UseEnergy(-rechargeValue);
        }
        move._stopMove = true;
        move.rechargeStop = true;
    }

    // [Andy] go back to idle state after recharging
    private IEnumerator Back2normal()
    { 
        StartCoroutine(audioMan.StartFade(audioSource, stunRecharge, 0));
        playerAnimator.SetTrigger("Recharge");
        playerAnimator.SetBool("isCharging", false);
       
        yield return new WaitForSeconds(stunRecharge);

        move._stopMove = false;
        move.rechargeStop = false;
        nextRechargeTime = rechargeCD;
        audioSource.volume = 1.0f;
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private IEnumerator UsePotion()
    {
        // [Andy] Play animation
        playerAnimator.SetTrigger("Heal");
        move.speed = move.speed/2;
        canHeal = false;
        audioMan.PlayAudio(audioSource, healClip);

        yield return new WaitForSeconds(healingTime);

        canHeal = false;
        TakeDamage(-potionValue);
        move.speed *= 2;

        yield return new WaitForSeconds(healingCD);
        canHeal = true;
    }

    // [Andy] LevelUp
    private void OnEnable()
    {
        ExperienceManager.Instance.OnExperienceChange += HandleExperienceChange;
    }
    private void OnDisable()
    {
        ExperienceManager.Instance.OnExperienceChange -= HandleExperienceChange;
    }
    private void HandleExperienceChange(int newXp)
    {
        currentXp += newXp;
        if (currentXp>=maxXP)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        LevelUpMenu.isPaused = true;
        currentLvl++;
        currentXp = 0;
        maxXP += 100;
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
