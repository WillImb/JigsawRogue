using System;
using System.Collections.Generic;
using UnityEngine;

public class SpecialComboEffects : MonoBehaviour
{
    public delegate void SpecialComboEffect();
    public delegate int SpecialAdditionEffect(int value);
    List<SpecialComboEffect> effectListBuffer;
    List<SpecialAdditionEffect> additionList;

    delegate void MultiTurnEffect(int turncounter, MultiTurnEffect func);
    /*
    priorties
    0: instant - effect happens immediately, seperate from calculations
        
    1: addition - value gets added/subtracted when cards are summing their values
    2: add to multiplier - value gets added to the multiplier after summing base values
    3: raw multiplier - value multiplies total after cards are done calculating their values

    4: unique - effects are seperate from the calculations, and non instant

    occurences
    next turn
    persistent/next # turns
    start of turn/end of turn
    on enemy's turn/enemy's next attack
    */

    /// <summary>
    /// next turn, addition
    /// fire cards stats +1
    /// </summary>
    void Spark() // fire fire combo
    {
        
    }

    /// <summary>
    /// next turn, addition
    /// water cards stats +1
    /// </summary>
    void Splash() // water water combo
    {
        
    }

    /// <summary>
    /// next turn, addition
    /// earth cards stats +1
    /// </summary>
    void Pebble() // earth earth combo
    {
        
    }

    /// <summary>
    /// next turn, addition
    /// air cards stats +1
    /// </summary>
    void Gust() // air air combo
    {
        
    }

    /// <summary>
    /// instant
    /// +20 health
    /// </summary>
    void Steam() //fire water combo
    {
        Player.instance.HealHealth(20);
    }

    /// <summary>
    /// next turn, add to multiplier
    /// +1 multiplier if >= 2 fire cards
    /// </summary>
    void Flame() // fire fire fire combo
    {
        
    }

    /// <summary>
    /// next # turns, add to multiplier
    /// +0 to +3 for next 3 turns
    /// </summary>
    /// <param name="turncounter"></param>
    void Wildfire(int turncounter = 3)
    {


        // if it has more turns left
        if (turncounter <= 0)
        return;

        //add to list
        Wildfire(turncounter-1);
    }
}
