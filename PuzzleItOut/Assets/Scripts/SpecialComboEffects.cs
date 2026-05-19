using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpecialComboEffects : MonoBehaviour
{
    public delegate void SpecialComboEffect();
    public delegate int SpecialAdditionEffect(List<PieceScriptable> pieces);
    public delegate int SpecialAddtoMultiplierEffect(List<PieceScriptable> pieces);
    public delegate int SpecialRawMultiplierEffect(List<PieceScriptable> pieces);

    List<Action> effectListBuffer;
    List<SpecialAdditionEffect> additionList;
    List<SpecialAddtoMultiplierEffect> addToMultiplierList;
    List<SpecialRawMultiplierEffect> rawMultiplierList;



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
    int Spark(List<PieceScriptable> pieces) // fire fire combo
    {
        return pieces.Count(piece => piece.cardType == cardType.fire);
    }

    /// <summary>
    /// next turn, addition
    /// water cards stats +1
    /// </summary>
    int Splash(List<PieceScriptable> pieces) // water water combo
    {
        return pieces.Count(piece => piece.cardType == cardType.water);
    }

    /// <summary>
    /// next turn, addition
    /// earth cards stats +1
    /// </summary>
    int Pebble(List<PieceScriptable> pieces) // earth earth combo
    {
        return pieces.Count(piece => piece.cardType == cardType.earth);
    }

    /// <summary>
    /// next turn, addition
    /// air cards stats +1
    /// </summary>
    int Gust(List<PieceScriptable> pieces) // air air combo
    {
        return pieces.Count(piece => piece.cardType == cardType.air);
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
    int Flame(List<PieceScriptable> pieces) // fire fire fire combo
    {
        return pieces.Count(piece => piece.cardType == cardType.fire) >= 2 ? 1 : 0;
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
