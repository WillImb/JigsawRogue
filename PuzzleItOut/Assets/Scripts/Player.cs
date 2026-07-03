using System;
using UnityEngine;

/*
 * Author(s): Anthony L, 
 * Date: 5.26.26
 * Notes:
 *  - 
 */
public class Player : MonoBehaviour
{
    public static Player instance;

    public int maxHealth;
    public int maxMana;

    public int health;
    public int mana;

    public bool completedTutorial;

    public event Action<int, int> OnHealthChanged;
    public event Action<int, int> OnManaChanged;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        health = maxHealth;
        mana = maxMana;

        OnHealthChanged?.Invoke(health, maxHealth);
        OnManaChanged?.Invoke(mana, maxMana);
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetMana()
    {
        return mana;
    }

    public void TakeDamage(int damage)
    {
        if (SpecialComboManager.Instance.ConvergentBoundaryDamageAbsorption)
        {
            SpecialComboManager.Instance.ConvergentBoundaryDamage = damage * 0.5f;
            damage = (int)(damage * 0.5f);
            SpecialComboManager.Instance.ConvergentBoundaryDamageAbsorption = false;
        }
        if (GameManager.instance.petrichorMudwallDamageReduction)
        {
            damage = (int)(damage * 0.75);
            GameManager.instance.petrichorMudwallDamageReduction = false;
        }

        health -= damage;
        if (health < 0)
            health = 0;

        OnHealthChanged?.Invoke(health, maxHealth);
        
        VFXManager.instance.SpawnNumber(new Vector3(5.5f, 0, 0), damage);
    }

    public void HealHealth(int healing)
    {
        health += healing;

        if (health > maxHealth)
            health = maxHealth;

        OnHealthChanged?.Invoke(health, maxHealth);
    }

    public void SpendMana(int manaToSpend)
    {
        mana -= manaToSpend;

        if (mana < 0)
            mana = 0;

        OnManaChanged?.Invoke(mana, maxMana);
    }

    public void ResetMana()
    {
        mana = maxMana;
        OnManaChanged?.Invoke(mana, maxMana);
    }
}