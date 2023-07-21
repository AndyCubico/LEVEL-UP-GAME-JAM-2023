using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Potions : MonoBehaviour
{
    public PlayerCombat player;
    public Image[] potions;
    public Sprite fullPotion;
    public Sprite emptyPotion;

    // Update is called once per frame
    void Update()
    {
        if (player.currentPotions > player.maxPotions)
        {
            player.currentPotions = player.maxPotions;
        }
        for (int i = 0; i < potions.Length; i++)
        {
            if (i<player.currentPotions )
            {
                potions[i].sprite = fullPotion;
            }
            else
            {
                potions[i].sprite = emptyPotion;
            }
            if (i<player.maxPotions)
            {
                potions[i].enabled = true;
            }
            else
            {
                potions[i].enabled = false;
            }
        }
    }
}
