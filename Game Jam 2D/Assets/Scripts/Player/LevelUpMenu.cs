using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUpMenu : MonoBehaviour
{
    public GameObject levelMenu;
    public static bool isPaused;
    public PlayerCombat player;

    private void Start()
    {
        levelMenu.SetActive(false);
    }

    private void Update()
    {
        if (isPaused)
        {
            PauseGame();
        }

    }
    public void PauseGame()
    {
        levelMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        levelMenu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
        player.currentHealth = player.maxHealth;
        player.healthBar.SetCurrentValue(player.currentHealth);
        player.healthBar.SetMaxValue(player.maxHealth);
        player.currentEnergy = player.maxEnergy;
        player.energyBar.SetCurrentValue(player.currentEnergy);
        player.energyBar.SetMaxValue(player.maxEnergy);
    }

    public void HealthUpgrade()
    {
        player.maxHealth += 10;
      
        ResumeGame();
    }

    public void EnergyUpgrade()
    {
        player.maxEnergy += 10;
        ResumeGame();
    }

    public void RechargeUpgrade()
    {
        player.rechargeValue += 0.1f;
        ResumeGame();
    }

    public void AttackUpgrade()
    {
        player.attackDamage += 5;
        ResumeGame();
    }

}

