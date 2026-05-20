using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpecialComboManager : MonoBehaviour
{
    public static SpecialComboManager Instance;
    void Awake()
    {
        Instance = this;
    }
    //List<Action> effectListBuffer;
    public List<(string Name, int Turns)> additionListBuffer;
    public List<(string Name, int Turns)> addToMultiplierListBuffer;
    public List<(string Name, int Turns)> rawMultiplierListBuffer;
    public List<(string Name, int Turns)> uniqueListBuffer;
    public List<(string Name, int Turns)> additionList;
    public List<(string Name, int Turns)> addToMultiplierList;
    public List<(string Name, int Turns)> rawMultiplierList;
    public List<(string Name, int Turns)> uniqueList;

    /// <summary>
    /// priorties
    /// 0: instant - effect happens immediately, seperate from calculations   
    /// 1: addition - value gets added/subtracted when cards are summing their values
    /// 2: add to multiplier - value gets added to the multiplier after summing base values
    /// 3: raw multiplier - value multiplies total after cards are done calculating their values
    /// 4: unique - effects are seperate from the calculations, and non instant
    /// </summary>
    public enum priority
    {
        instant,
        addition,
        addToMultiplier,
        rawMultiplier,
        unique
    }

    /* occurences
    next turn
    persistent/next # turns
    start of turn/end of turn
    on enemy's turn/enemy's next attack
    */
    void Start()
    {
        additionListBuffer = new List<(string Name, int Turns)>();
        addToMultiplierListBuffer = new List<(string Name, int Turns)>();
        rawMultiplierListBuffer = new List<(string Name, int Turns)>();
        uniqueListBuffer = new List<(string Name, int Turns)>();
        additionList = new List<(string Name, int Turns)>();
        addToMultiplierList = new List<(string Name, int Turns)>();
        rawMultiplierList = new List<(string Name, int Turns)>();
        uniqueList = new List<(string Name, int Turns)>();
        clearEffects();
    }
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
        switch (combo.priority)
        {
            case priority.instant:
                Invoke(combo.comboName, 0.0f);
                break;
            case priority.addition:
                additionListBuffer.Add((combo.comboName, combo.turns));
                break;
            case priority.addToMultiplier:
                addToMultiplierListBuffer.Add((combo.comboName, combo.turns));
                break;
            case priority.rawMultiplier:
                rawMultiplierListBuffer.Add((combo.comboName, combo.turns));
                break;
            case priority.unique:
                uniqueListBuffer.Add((combo.comboName, combo.turns));
                break;
            default:
                print("priority for "+combo.comboName+" not found");
                break;
        }
    }

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
        //print("cleaning lists");
        
        for(int index = additionList.Count - 1;index > -1; index--)
        {   
            //reduce count by 1
            additionList[index] = (additionList[index].Name, additionList[index].Turns - 1);
        }
        additionList.RemoveAll(e => e.Turns == 0);
        
        for(int index = addToMultiplierList.Count - 1;index > -1; index--)
        {   
            addToMultiplierList[index] = (addToMultiplierList[index].Name, addToMultiplierList[index].Turns - 1);
        }
        addToMultiplierList.RemoveAll(e => e.Turns == 0);
        
        for(int index = rawMultiplierList.Count - 1;index > -1; index--)
        {   
            rawMultiplierList[index] = (rawMultiplierList[index].Name, rawMultiplierList[index].Turns - 1);
        }
        rawMultiplierList.RemoveAll(e => e.Turns == 0);
        
        for(int index = uniqueList.Count - 1;index > -1; index--)
        {   
            uniqueList[index] = (uniqueList[index].Name, uniqueList[index].Turns - 1);
        }
        uniqueList.RemoveAll(e => e.Turns == 0);
    }
    
    // Combo Effects
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

    int Wildfire(List<PieceScriptable> pieces) // fire air air combo
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

}
