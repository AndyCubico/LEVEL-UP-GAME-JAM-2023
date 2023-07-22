using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum HP_UPGRADE
{
    MAX_HP,
    HEAL_UPGRADE,
    NUMBER_POTIONS
}

public enum ENERGY_UPGRADE
{
    MAX_ENERGY,
    RECHARGE_VALUE
}

public enum ATTACK_UPGRADE
{
    ATTACK_DAMAGE,
    ATTACK_ENERGY,
    SPECIAL_DAMAGE,
    SPECIAL_ENERGY
}

public class LevelUpMenu : MonoBehaviour
{
    public GameObject levelMenu;
    public static bool isPaused;//[Andy] pause game when level up menu appears
    public PlayerCombat player;
    private HP_UPGRADE hp_upgrade;
    private ENERGY_UPGRADE energy_upgrade;
    private ATTACK_UPGRADE attack_upgrade;

    public TMP_Text textHealth;
    public TMP_Text textEnergy;
    public TMP_Text textAttack;

    float randHealth;
    float randEnergy;
    float randAttack;

    public int hp = 10;
    public int heal = 5;
    public int potion = 1;
    public float energy = 10f;
    public float recharge = 0.1f;
    public int Adamage = 5;
    public float Acost = 0.2f;
    public int Sdamage = 10;
    public float Scost = 5f;

    private void Start()
    {
        levelMenu.SetActive(false);
    }

    private void Update()
    {
        if (isPaused)
        {
            if (Time.timeScale!=0f)
            {
                PauseGame();
            }
        }

    }
    public void PauseGame()
    {
        player.textXPbar.text = player.currentXp + "/" + player.maxXP;
        levelMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        //[Andy] random number to choose level up options
        randHealth = Random.Range(0f, 10f);
        if (randHealth<=5f)
        {
            hp_upgrade = HP_UPGRADE.MAX_HP;
            textHealth.text = "Max HP";
        }
        else if (randHealth > 5f && randHealth<=9f)
        {
            hp_upgrade = HP_UPGRADE.HEAL_UPGRADE;
            textHealth.text = "Healing";
        }
        else if (randHealth > 9f)
        {
            hp_upgrade = HP_UPGRADE.NUMBER_POTIONS;
            textHealth.text = "Potions number";
        }

        randEnergy = Random.Range(0f, 10f);

        if (randEnergy <= 5f)
        {
            energy_upgrade = ENERGY_UPGRADE.MAX_ENERGY;
            textEnergy.text = "Max Energy";
        }
        else
        {
            energy_upgrade = ENERGY_UPGRADE.RECHARGE_VALUE;
            textEnergy.text = "Recharge rate";
        }

        randAttack = Random.Range(0f, 10f);

        if (randAttack <= 5f)
        {
            attack_upgrade = ATTACK_UPGRADE.ATTACK_DAMAGE;
            textAttack.text = "Attack damage";
        }
        else if (randAttack > 5f && randAttack <= 7f)
        {
            attack_upgrade = ATTACK_UPGRADE.ATTACK_ENERGY;
            textAttack.text = "Attack energy cost";
        }
        else if (randAttack > 7f && randAttack <= 9f)
        {
            attack_upgrade = ATTACK_UPGRADE.SPECIAL_DAMAGE;
            textAttack.text = "Special damage";
        }
        else if(randAttack > 9f)
        {
            attack_upgrade = ATTACK_UPGRADE.SPECIAL_ENERGY;
            textAttack.text = "Special energy cost";
        }
    }

    public void ResumeGame()
    {
        levelMenu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
        // [Andy] Se podría poner en una función de reset, si hace falta lo hago
        player.currentHealth = player.maxHealth;
        player.healthBar.SetCurrentValue(player.currentHealth);
        player.healthBar.SetMaxValue(player.maxHealth);
        player.currentEnergy = player.maxEnergy;
        player.energyBar.SetCurrentValue(player.currentEnergy);
        player.energyBar.SetMaxValue(player.maxEnergy);
        player.currentPotions = player.maxPotions;
        player.currentLvl++;
        player.currentXp = 0;
        player.maxXP += 100;
        player.xpBar.SetMaxValue(player.maxXP);
        player.xpBar.SetTo0();
        player.textXPbar.text = player.currentXp + "/" + player.maxXP;
        player.textEnergyBar.text = player.currentEnergy + "/" + player.maxEnergy;
        player.textHealthBar.text = player.currentHealth + "/" + player.maxHealth;
        player.textLevelNumber.text = player.currentLvl.ToString();
    }

    public void HealthUpgrade()
    {
        switch (hp_upgrade)
        {
            case HP_UPGRADE.MAX_HP:
                player.maxHealth += hp;
                break;
            case HP_UPGRADE.HEAL_UPGRADE:
                player.potionValue += heal;
                break;
            case HP_UPGRADE.NUMBER_POTIONS:
                player.maxPotions += potion;
                break;
            default:
                break;
        }
        ResumeGame();
    }

    public void EnergyUpgrade()
    {
        switch (energy_upgrade)
        {
            case ENERGY_UPGRADE.MAX_ENERGY:
                player.maxEnergy += energy;
                break;
            case ENERGY_UPGRADE.RECHARGE_VALUE:
                player.rechargeValue += recharge;
                break;
            default:
                break;
        }
        
        ResumeGame();
    }

    public void AttackUpgrade()
    {
        switch (attack_upgrade)
        {
            case ATTACK_UPGRADE.ATTACK_DAMAGE:
                player.attackDamage += Adamage;
                break;
            case ATTACK_UPGRADE.ATTACK_ENERGY:
                player.normalCost -= Acost;
                break;
            case ATTACK_UPGRADE.SPECIAL_DAMAGE:
                player.specialDamage += Sdamage;
                break;
            case ATTACK_UPGRADE.SPECIAL_ENERGY:
                player.specialCost -= Scost;
                break;
            default:
                break;
        }
        ResumeGame();
    }
}
