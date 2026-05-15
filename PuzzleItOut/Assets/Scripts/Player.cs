using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player:MonoBehaviour
{
    public static Player instance;
    public int gold;
    public int maxHealth;
    [SerializeField]
    int health;
    public bool completedTutorial;


    public Slider playerHealthSlider;

    // no combo type yet
    // List<Combo> AllCombos
    // List<Combo? CombosUnlocked

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void Start()
    {
        health = maxHealth;
        playerHealthSlider.value = health / (float)maxHealth;

    }

    public int GetGold()
    {
        return gold;
    }

    public int GetHealth()
    {
        return health;
    }

    // doesnt actually do anything yet
    public void SpendGold(int goldToSpend)
    {
        gold -= goldToSpend;
    }

    // no game over detection
    public void TakeDamage(int damage)
    {
        health -= damage;
        playerHealthSlider.value = health / (float)maxHealth;
    }

    public void HealHealth(int healing)
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        playerHealthSlider.value = health / (float)maxHealth;

    }

}
