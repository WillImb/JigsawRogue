using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public static Player instance;
    float gold;
    float health;
    // no combo type yet
    // List<Combo> AllCombos
    // List<Combo? CombosUnlocked

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
    }
}
