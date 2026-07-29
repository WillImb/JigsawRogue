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

    public int shield;
    public int overhealth;

    public bool completedTutorial;

    public event Action<int, int> OnHealthChanged;
    public event Action<int, int> OnManaChanged;
    public event Action<int> OnShieldChanged;
    public event Action<int> OnOverhealthChanged;

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
        shield = 0;
        overhealth = 0;

        OnHealthChanged?.Invoke(health, maxHealth);
        OnManaChanged?.Invoke(mana, maxMana);
        OnShieldChanged?.Invoke(shield);
        OnOverhealthChanged?.Invoke(overhealth);
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetMana()
    {
        return mana;
    }

    public int GetShield()
    {
        return shield;
    }

    public int GetOverhealth()
    {
        return overhealth;
    }

    public void PrintStatus()
    {   
        print("Status");
        print("Shield(s): " + GetShield());
        print("Overhealth: " + GetOverhealth());
        print("Health: " + GetHealth());
        print("Mana: "+ GetMana());
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

        if(shield > damage)
        {
            shield -= damage;
            damage = 0;
            OnShieldChanged?.Invoke(shield);
        }
        else if (damage > shield)
        {
            damage -= shield;
            shield = 0;
            OnShieldChanged?.Invoke(shield);
        }

        if (overhealth > damage)
        {
            overhealth -= damage;
            damage = 0;
            OnOverhealthChanged?.Invoke(overhealth);
        }
        else if (damage > overhealth)
        {
            damage -= overhealth;
            overhealth = 0;
            OnOverhealthChanged?.Invoke(overhealth);
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