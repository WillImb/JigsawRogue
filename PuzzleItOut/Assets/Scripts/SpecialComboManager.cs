using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class SpecialComboManager : MonoBehaviour
{
    public static SpecialComboManager Instance;
    void Awake()
    {
        Instance = this;
    }
#region declarations
    public List<(MethodInfo Effect, int Turns)> additionListBuffer;
    public List<(MethodInfo Effect, int Turns)> addToMultiplierListBuffer;
    public List<(MethodInfo Effect, int Turns)> rawMultiplierListBuffer;
    public List<(MethodInfo Effect, int Turns)> uniqueListBuffer;
    public List<(MethodInfo Effect, int Turns)> additionList;
    public List<(MethodInfo Effect, int Turns)> addToMultiplierList;
    public List<(MethodInfo Effect, int Turns)> rawMultiplierList;
    public List<(MethodInfo Effect, int Turns)> uniqueList;


    /// <summary>
    /// priorties
    /// 0: instant - effect happens immediately, seperate from calculations   
    /// 1: addition - value gets added/subtracted when cards are summing their values
    /// 2: add to multiplier - value gets added to the multiplier after summing base values
    /// 3: raw multiplier - value multiplies total after cards are done calculating their values
    /// 4: unique - effects are seperate from the calculations, and non instant
    /// </summary>
    public delegate int NumberEffect (List<PieceScriptable> pieces, AffectedStat affectedStat);
    public delegate void UniqueEffect ();
    public enum priority
    {
        instant,
        addition,
        addToMultiplier,
        rawMultiplier,
        unique
    }

    [Flags]
    public enum AffectedStat
    {
        NoRequirements = 0,
        Damage = 1,
        Gold = 2,
        Health = 4
    }
    /* occurences
    next turn
    persistent/next # turns
    start of turn/end of turn
    on enemy's turn/enemy's next attack
    */
#endregion
    void Start()
    {
        additionListBuffer = new List<(MethodInfo Effect, int Turns)>();
        addToMultiplierListBuffer = new List<(MethodInfo Effect, int Turns)>();
        rawMultiplierListBuffer = new List<(MethodInfo Effect, int Turns)>();
        uniqueListBuffer = new List<(MethodInfo Effect, int Turns)>();
        additionList = new List<(MethodInfo Effect, int Turns)>();
        addToMultiplierList = new List<(MethodInfo Effect, int Turns)>();
        rawMultiplierList = new List<(MethodInfo Effect, int Turns)>();
        uniqueList = new List<(MethodInfo Effect, int Turns)>();
        clearEffects();
    }
    
#region List Control
    public void clearEffects()
    {
        additionList.Clear();
        additionListBuffer.Clear();
        addToMultiplierList.Clear();
        addToMultiplierListBuffer.Clear();
        rawMultiplierList.Clear();
        rawMultiplierListBuffer.Clear();
        uniqueList.Clear();
        uniqueListBuffer.Clear();
    }

    public void addEffect(ComboScriptable combo)
    {
        MethodInfo effect = typeof(SpecialComboManager).GetMethod(combo.comboName, BindingFlags.NonPublic | BindingFlags.Instance); //, new Type[] {typeof(List<PieceScriptable>)}
        
        if(effect==null){
            Debug.Log("Combo method doesn't exist, or name does not match any in SpecialComboManager");
            return;
        }

        switch (combo.priority)
        {
            case priority.instant:
                effect.Invoke(this, null);
                break;
            case priority.addition:
                additionListBuffer.Add((effect, combo.turns));
                break;
            case priority.addToMultiplier:
                addToMultiplierListBuffer.Add((effect, combo.turns));
                break;
            case priority.rawMultiplier:
                rawMultiplierListBuffer.Add((effect, combo.turns));
                break;
            case priority.unique:
                uniqueListBuffer.Add((effect, combo.turns));
                break;
            default:
                print("priority for "+combo.comboName+" not found");
                break;
        }
    }

    /// <summary>
    /// moves effects from the buffer (they were just casted) to the main lists (will take effect next turn and so on)
    /// </summary>
    public void moveFromBuffer()
    {
        additionList.AddRange(additionListBuffer);
        additionListBuffer.Clear();

        addToMultiplierList.AddRange(addToMultiplierListBuffer);
        addToMultiplierListBuffer.Clear();

        rawMultiplierList.AddRange(rawMultiplierListBuffer);
        rawMultiplierListBuffer.Clear();

        uniqueList.AddRange(uniqueListBuffer);
        uniqueListBuffer.Clear();
    }

    /// <summary>
    /// goes through lists, reduces turn counts by 1, and removes and that are equal to 0
    /// </summary>
    public void cleanTurnLists()
    {        
        for(int index = additionList.Count - 1;index > -1; index--)
        {   
            //reduce count by 1
            additionList[index] = (additionList[index].Effect, additionList[index].Turns - 1);
        }
        additionList.RemoveAll(e => e.Turns == 0);
        
        for(int index = addToMultiplierList.Count - 1;index > -1; index--)
        {   
            addToMultiplierList[index] = (addToMultiplierList[index].Effect, addToMultiplierList[index].Turns - 1);
        }
        addToMultiplierList.RemoveAll(e => e.Turns == 0);
        
        for(int index = rawMultiplierList.Count - 1;index > -1; index--)
        {   
            rawMultiplierList[index] = (rawMultiplierList[index].Effect, rawMultiplierList[index].Turns - 1);
        }
        rawMultiplierList.RemoveAll(e => e.Turns == 0);
        
        for(int index = uniqueList.Count - 1;index > -1; index--)
        {   
            uniqueList[index] = (uniqueList[index].Effect, uniqueList[index].Turns - 1);
        }
        uniqueList.RemoveAll(e => e.Turns == 0);
    }
#endregion

#region Combo Effects
    /// <summary>
    /// next turn, addition
    /// fire cards stats +1
    /// </summary>
    int Spark(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements) // fire fire combo
    {
        Debug.Log(affectedStat);
        return pieces.Count(piece => piece.cardType == cardType.fire);
    }

    /// <summary>
    /// next turn, addition
    /// water cards stats +1
    /// </summary>
    int Splash(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements) // water water combo
    {
        return pieces.Count(piece => piece.cardType == cardType.water);
    }

    /// <summary>
    /// next turn, addition
    /// earth cards stats +1
    /// </summary>
    int Pebble(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements) // earth earth combo
    {
        return pieces.Count(piece => piece.cardType == cardType.earth);
    }

    /// <summary>
    /// next turn, addition
    /// air cards stats +1
    /// </summary>
    int Gust(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements) // air air combo
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
    int Flame(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements) // fire fire fire combo
    {
        return pieces.Count(piece => piece.cardType == cardType.fire) >= 2 ? 1 : 0;
    }

    /// <summary>
    /// next # turns, add to multiplier
    /// +0 to +3 for next 3 turns
    /// </summary>
    int Wildfire(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements) // fire air air combo
    {
        return UnityEngine.Random.Range(0,4);
    }

    /// <summary>
    /// next # turns, unique
    /// +5 health
    /// </summary>
    void Campfire() // fire air combo
    {
        Player.instance.HealHealth(5);
    }

    /// <summary>
    /// multiple effects, instant
    /// next turn, raw multiplier
    /// 2x damage
    /// next # turns, unique
    /// deal 5-10 damage
    /// </summary>
    void VolcanicRock()
    {
        // NumberEffect VolcanicRockMultiplierEffect = delegate (List<PieceScriptable> pieces, AffectedStat affectedStat)
        // {  
        //     return affectedStat.HasFlag(AffectedStat.Damage) ? 2 : 1;
        // };
        NumberEffect volcanicRockMultiplierEffect = VolcanicRockMultiplierEffect;
        rawMultiplierListBuffer.Add((volcanicRockMultiplierEffect.Method, 1));

        // UniqueEffect VolcanicRockBurnEffect = delegate ()
        // {
        //     // damage to be dealt
        //     UnityEngine.Random.Range(5,11);
        // };
        UniqueEffect volcanicRockBurnEffect = VolcanicRockBurnEffect;
        uniqueListBuffer.Add((volcanicRockBurnEffect.Method, 3));
    }
    int VolcanicRockMultiplierEffect(List<PieceScriptable> pieces, AffectedStat affectedStat)
    {  
        return affectedStat.HasFlag(AffectedStat.Damage) ? 2 : 1;
    }
    void VolcanicRockBurnEffect()
    {
        // damage to be dealt
        GameManager.instance.currentEnemy.TakeDamage(UnityEngine.Random.Range(5,11));
    }
#endregion
}
