using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player:MonoBehaviour
{
    public static Player instance;
    public float gold;
    public float maxHealth;
    [SerializeField]
    float health;
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
        playerHealthSlider.value = health / maxHealth;

    }

    public float GetGold()
    {
        return gold;
    }

    public float GetHealth()
    {
        return health;
    }

    // doesnt actually do anything yet
    public void SpendGold(float goldToSpend)
    {
        gold -= goldToSpend;
    }

    // no game over detection
    public void TakeDamage(float damage)
    {
        health -= damage;
        playerHealthSlider.value = health / maxHealth;
    }

    public void HealHealth(float healing)
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        playerHealthSlider.value = health / maxHealth;

    }

}
