using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerCombat : MonoBehaviour
{
    public GameObject FloatingText;
    private Animator playerAnimator;
    [SerializeField] private ParticleSystem attackParticle;
    [SerializeField] private ParticleSystem specialParticle;
    [SerializeField] private ParticleSystem healParticle;
    [SerializeField] private ParticleSystem rechargeParticle;
    [SerializeField] private ParticleSystem hitParticle;
    [SerializeField] private ParticleSystem deathParticle;
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask enemyLayers;

    [SerializeField] public BoolSO isBossAlive;
    [SerializeField] private BoolSO isPlayerInLight;
    [SerializeField] private GameObject SunLight;

    public int attackDamage = 40;
    public float normalCost = 5f;
    public int specialDamage = 80;
    public float specialCost = 50f;
    [SerializeField] private float attackCD = 2f;
    float nextAttackTime = 0f;

    //barras
    public int maxXP;
    public int currentXp;
    public int currentLvl;
    public int maxHealth = 100;
    public int currentHealth;
    public float maxEnergy = 100;
    public float currentEnergy;
    public Bar healthBar;
    public Bar energyBar;
    public Bar xpBar;
    public TMP_Text textXPbar;
    public TMP_Text textEnergyBar;
    public TMP_Text textHealthBar;
    public TMP_Text textLevelNumber;

    //HealthPotion
    private bool canHeal = true;
    public int potionValue;
    public int maxPotions = 3;
    public int currentPotions = 3;
    [SerializeField] private float healingCD;
    [SerializeField] private float healingTime;

    private PlayerMovement move;

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
    [SerializeField] private AudioClip dedClip;

    private AudioManager audioMan;

    // [Andy] debug
    int xpAmount = 100;

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioMan = GetComponent<AudioManager>();
        move = GetComponent<PlayerMovement>();
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
        healthBar.SetMaxValue(maxHealth);
        energyBar.SetMaxValue(maxEnergy);
        xpBar.SetMaxValue(maxXP);
        xpBar.SetTo0();
        textXPbar.text = currentXp + "/" + maxXP;
        textEnergyBar.text = currentEnergy + "/" + maxEnergy;
        textHealthBar.text = currentHealth + "/" + maxHealth;
        textLevelNumber.text = currentLvl.ToString();

    }
    void Update()
    {
        if (LevelUpMenu.isPaused || PauseMenu.isPausedMenu)
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

        else if ((Input.GetKeyDown(KeyCode.Q)|| Input.GetKeyDown(KeyCode.JoystickButton1)) && canHeal && currentPotions != 0)
        {
            StartCoroutine(UsePotion());
        }

        // [Andy] Recharge logic
        if (nextRechargeTime > 0)
        {
            nextRechargeTime -= Time.deltaTime;
        }

        if ((Input.GetKeyDown(KeyCode.R)) && nextRechargeTime <= 0 && isBossAlive.Value == false)
        {
            SetLightToScene();
        }

        if (Input.GetKey(KeyCode.R) && nextRechargeTime <= 0 && isPlayerInLight.Value)
        {
            Recharge();
        }

        else if (Input.GetKeyUp(KeyCode.R) && isPlayerInLight.Value)
        {
            StartCoroutine(Back2normal());
        }


        //if ((Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.JoystickButton3)) && nextRechargeTime <= 0)
        //{
        //    Recharge();
        //}
        //else if (Input.GetKeyUp(KeyCode.R) && move.rechargeStop)
        //{

        //    StartCoroutine(Back2normal());
        //}
        if (Input.GetKeyDown(KeyCode.L))
        {
            ExperienceManager.Instance.AddExperience(xpAmount);
        }
    }
    private void Attack()
    {
        attackParticle.Play();
        // [Andy] Play animation
        weaponAnimator.SetTrigger("Attack");

        // [Andy] Detect enemies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // [Andy] Damage
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hiteado el man " + enemy.name);
            if (enemy.gameObject.layer == 6)
            {
                enemy.GetComponent<Enemy>().TakeDamage((int)(attackDamage * (currentEnergy / maxEnergy)));// [Andy] damage reduction whit energy remaining
            }
            else if (enemy.gameObject.layer == 9)
            {
                enemy.GetComponent<BossBehaviour>().TakeDamage((int)(attackDamage * (currentEnergy / maxEnergy)));
            }
        }

        // [Andy] spend energy
        UseEnergy(normalCost);

        audioMan.PlayAudio(audioSource,attackClip);
        //ShowFloatingText();
    }

    //private void ShowFloatingText()
    //{
    //    var go = Instantiate(FloatingText, transform.position, Quaternion.identity, transform);
    //    int daños = (int)(attackDamage * (currentEnergy / maxEnergy));
    //    go.GetComponent<TextMesh>().text = daños.ToString();
    //}

    private void SpecialAttack()
    {
        // [Andy] Play animation
        weaponAnimator.SetTrigger("Attack");//cambiar a otra animacion
        specialParticle.Play();

        // [Andy] Detect enemies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // [Andy] Damage
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Special Hiteado el man " + enemy.name);
            if (enemy.gameObject.layer == 6)
            {
                enemy.GetComponent<Enemy>().TakeDamage(specialDamage);
            }
            else if (enemy.gameObject.layer == 9)
            {
                enemy.GetComponent<BossBehaviour>().TakeDamage(specialDamage);
            }
        }

        // [Andy] spend energy
        UseEnergy(specialCost);

        audioMan.PlayAudio(audioSource, specialClip);
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetCurrentValue(currentHealth);
        textHealthBar.text = currentHealth + "/" + maxHealth;
        if (damage>0)
        {
            audioMan.PlayAudio(audioSource, hurtClip);
            hitParticle.Play();
            playerAnimator.SetTrigger("Hurt");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        audioMan.PlayAudio(audioSource, dedClip);
        Debug.Log("Player ded");
        textHealthBar.text = 0 + "/" + maxHealth;
        deathParticle.Play();
        playerAnimator.SetBool("isDed", true);
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

    private void RestarLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UseEnergy(float energy)
    {
        currentEnergy -= energy;
        textEnergyBar.text = (int)currentEnergy + "/" + maxEnergy;
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

    private void SetLightToScene()
    {
        SunLight.GetComponent<InvertSun>().Radius = 5.0f;
        SunLight.GetComponent<InvertSun>().RadiusDiff = 0.0f;
        Instantiate(SunLight, new Vector3(transform.position.x/2, transform.position.y/2, transform.position.z + 0.5f), Quaternion.identity);
    }

    private IEnumerator UsePotion()
    {
        // [Andy] Play animation
        healParticle.Play();
        //playerAnimator.SetTrigger("Heal");
        move.speed = move.speed/2;
        canHeal = false;
        audioMan.PlayAudio(audioSource, healClip);

        yield return new WaitForSeconds(healingTime);

        canHeal = false;
        TakeDamage(-potionValue);
        if (currentHealth>maxHealth)
        {
            currentHealth = maxHealth;
            healthBar.SetCurrentValue(currentHealth);
            textHealthBar.text = currentHealth + "/" + maxHealth;
        }
        move.speed *= 2;
        currentPotions--;

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
        xpBar.SetCurrentValue(currentXp);
        textXPbar.text = currentXp + "/" + maxXP;
        if (currentXp>=maxXP)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        LevelUpMenu.isPaused = true;
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
