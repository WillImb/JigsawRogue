using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
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
    public delegate float NumberEffect (List<PieceScriptable> pieces, AffectedStat affectedStat);
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

    /// <summary>
    /// adds effects to their correct list or refreshes their duration
    /// </summary>
    /// <param name="combo"></param>
    public void addEffect(ComboScriptable combo)
    {
        MethodInfo effect = typeof(SpecialComboManager).GetMethod(combo.comboName, BindingFlags.NonPublic | BindingFlags.Instance); //, new Type[] {typeof(List<PieceScriptable>)}
        
        //if the effect is not there or the turn duration is infinite
        if(effect==null || findActiveEffect(combo.comboName).Turns < 0){
            Debug.Log("Combo method doesn't exist, or name does not match any in SpecialComboManager");
            return;
        }
        //duration refresh
        if(additionList.Any(t => t.Effect == effect))
        {
            additionList[additionList.FindIndex(t => t.Effect == effect)] = (effect, combo.turns);
            return;
        } 
        else if(addToMultiplierList.Any(t => t.Effect == effect))
        {
            addToMultiplierList[addToMultiplierList.FindIndex(t => t.Effect == effect)] = (effect, combo.turns);
            return;
        }
        else if(rawMultiplierList.Any(t => t.Effect == effect))
        {
            rawMultiplierList[rawMultiplierList.FindIndex(t => t.Effect == effect)] = (effect, combo.turns);
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

    public bool isEffectActive(string combo)
    {
        return false;
    }

    public (MethodInfo Effect, int Turns) findActiveEffect(string comboName)
    {
        MethodInfo effect = typeof(SpecialComboManager).GetMethod(comboName, BindingFlags.NonPublic | BindingFlags.Instance);
        
        if(additionList.Any(t => t.Effect == effect))
        {
            return additionList[additionList.FindIndex(t => t.Effect == effect)];
        } 
        else if(addToMultiplierList.Any(t => t.Effect == effect))
        {
            return addToMultiplierList[addToMultiplierList.FindIndex(t => t.Effect == effect)];
        }
        else if(rawMultiplierList.Any(t => t.Effect == effect))
        {
            return rawMultiplierList[rawMultiplierList.FindIndex(t => t.Effect == effect)];
        }
        else if(uniqueList.Any(t => t.Effect == effect))
        {
            return uniqueList[uniqueList.FindIndex(t => t.Effect == effect)];
        }
        return default;
    }

    public void removeEffect(string comboName)
    {
        MethodInfo effect = typeof(SpecialComboManager).GetMethod(comboName, BindingFlags.NonPublic | BindingFlags.Instance);

        additionList.RemoveAll(t => t.Effect == effect);
        addToMultiplierList.RemoveAll(t => t.Effect == effect);
        rawMultiplierList.RemoveAll(t => t.Effect == effect);
        uniqueList.RemoveAll(t => t.Effect == effect);
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
    /// instant and raw multiplier
    /// 60% chance to lower enemy damage
    /// and
    /// 30% chance to lower own damage
    /// </summary>
    void AcidRain() // fire water air combo
    {
        if(UnityEngine.Random.Range(0,0.6f) < 0.6f)
        {
            GameManager.instance.enemyDamageReduced = true;
        }
        if(UnityEngine.Random.Range(0,0.3f) < 0.3f)
        {    
            NumberEffect acidRainDamageReduction = AcidRainDamageReduction;
            rawMultiplierListBuffer.Add((acidRainDamageReduction.Method,1));
        }
    }
    float AcidRainDamageReduction(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements)
    {
        if(affectedStat == AffectedStat.Damage)
        {
            return 0.5f;
        }
        return 1;
    }
    
    /// <summary>
    /// instant
    /// 30% chance for enemy to rebound
    /// or
    /// 30% chance to lower enemy damage
    /// </summary>
    void Ashfall() // fire earth air combo
    {
        float randomFloat = UnityEngine.Random.Range(0.0f,1.0f);
        if(randomFloat < 0.3f)
        {
            GameManager.instance.enemyRebound = true;
        }
        else if (randomFloat < 0.6f)
        {
            GameManager.instance.enemyDamageReduced = true;
        }
    }

    void BeachBonfire(){}

    /// <summary>
    /// instant
    /// damage is fire piece combat stat x sum of air pieces combat stat
    /// </summary>
    void BlazingWinds() // fire air air air combo
    {
        List<PieceScriptable> pieces = BoardManager.instance.GetBoardPieces();
        for(int i = 0; i < pieces.Where(p => p.cardType == cardType.air).Sum(p => p.combatValue); i++)
        {
            GameManager.instance.currentEnemy.TakeDamage(pieces.Find(p => p.cardType == cardType.fire).combatValue);
        }
    }
    
    /// <summary>
    /// next turn, add to multiplier
    /// +1 multiplier if >= 2 earth cards
    /// </summary>
    int Boulder(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements) // earth earth earth combo
    {
        return pieces.Count(p => p.cardType == cardType.earth) >= 2 ? 1 : 0;
    }

    /// <summary>
    /// next # turns, unique
    /// +5 health
    /// </summary>
    void Campfire() // fire air combo
    {
        Player.instance.HealHealth(5);
    }

    void Clay(){}

    void ConvergentBoundary(){}

    void Cyclone(){}

    void ElementalRainbow(){}

    /// <summary>
    /// next turn, add to multiplier
    /// +1 multiplier for each card with 30% chance to not trigger or 70% chance for +1 multiplier for each card
    /// </summary>
    int Erosion(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements) // earth air combo
    {
        if (UnityEngine.Random.Range(0, 1) <= 0.3f)
        return 0;

        return pieces.Count;
    }

    /// <summary>
    /// next turn, additiion
    /// +3 damage if combo contains any water air or earth piece
    /// </summary>
    int ExplosiveClay(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements) // water earth air air combo
    {
        if(affectedStat == AffectedStat.Damage)
        {
            if(pieces.Any(piece => piece.cardType == cardType.water|piece.cardType == cardType.air|piece.cardType == cardType.earth))
            return 3;
        }
        return 0;
    }

    /// <summary> 
    /// persistent, addition
    /// +2 to fire pieces stats
    /// persistent, add to multiplier
    /// +1 to multiplier if fire piece
    /// </summary>
    int Fireball(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements) // fire fire fire fire combo
    {
        //see if this is first turn with this effect
        if(findActiveEffect("Fireball").Turns == -1 && affectedStat == AffectedStat.Damage) // cause damage happens first
        {
            print("adding fireball multiplier effect");
            NumberEffect fireballAddToMultiplier = FireballAddToMultiplier;
            addToMultiplierList.Add((fireballAddToMultiplier.Method,-1));
        }
        if (pieces.Any(piece => piece.cardType == cardType.fire))
        {  
            return pieces.Count(piece => piece.cardType == cardType.fire) * 2;
        }
        else
        {
            print("removing fireball effects");
            removeEffect("FireballAddToMultiplier");
            removeEffect("Fireball");
        }
        return 0;
    }
    float FireballAddToMultiplier(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements)
    {
        return 1;
    }

    /// <summary>
    /// next turn, add to multiplier
    /// +1 multiplier if >= 2 fire cards
    /// </summary>
    int Flame(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements) // fire fire fire combo
    {
        return pieces.Count(p => p.cardType == cardType.fire) >= 2 ? 1 : 0;
    }

    /// <summary>
    /// instant
    /// set card type condition
    /// next turn, addition
    /// if two __ cardtypes are played add 5-7 combat stat to combo
    /// </summary>
    void FlashFlood() // water earth air combo
    {
        nextFlashFloodType();
        NumberEffect flashFloodDamage = FlashFloodDamage;
        additionListBuffer.Add((flashFloodDamage.Method, 1));
    }
    float FlashFloodDamage(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements)
    {
        if(affectedStat != AffectedStat.Damage)
        {
            return 0;
        }
        if(pieces.Count(p => p.cardType == getFlashFloodType()) >= 2)
        {
            return UnityEngine.Random.Range(5, 8);
        }
        return 0;
    }
    cardType getFlashFloodType()
    {
        if(FlashFloodType == null)
        {
            nextFlashFloodType();
        }
        return (cardType)FlashFloodType;
    }
    void nextFlashFloodType()
    {
        FlashFloodType = (cardType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(cardType)).Length);
        print("Next Flash Flood is " + FlashFloodType.ToString());
    }
    cardType? FlashFloodType;

    /// <summary>
    /// instant
    /// 50% chance to stun enemy
    /// </summary>
    void Fog() // water air combo
    {
        if (UnityEngine.Random.Range(0,0.5f) < 0.5f)
        {
            // stun effect
            GameManager.instance.enemyStunned = true;
        }
    }

    void ForgingSteel(){}

    /// <summary>
    /// next turn, addition
    /// air cards stats +1
    /// </summary>
    int Gust(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements) // air air combo
    {
        return pieces.Count(piece => piece.cardType == cardType.air);
    }

    void HotCoal(){}

    void Mist(){}

    /// <summary>
    /// instant
    /// next # turns, addition
    /// if > 5 to 7 gold in possesion, sacrifice 5 to 7 gold to add +1 gold per card in next two spells
    /// </summary>
    void MoltenGold() // fire fire earth combo
    {
        if(GoldManager.Instance.Gold < 5)
        {
            return;
        }
        int sacrificeMax = (int)MathF.Min(GoldManager.Instance.Gold, 7.0f);
        GoldManager.Instance.SpendGold(UnityEngine.Random.Range(5, sacrificeMax));
        //put some animation here to show your money going away

        NumberEffect moltenGoldBonus = MoltenGoldBonus;
        additionListBuffer.Add((moltenGoldBonus.Method, 2));
    }
    float MoltenGoldBonus(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements)
    {
        print("adding bonus gold");
        return affectedStat == AffectedStat.Gold ? pieces.Count : 0;
    }

    /// <summary>
    /// next turn, addition
    /// +3 combat stat per card
    /// </summary>
    int Mudslide(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements) // water earth earth combo
    {
        if(affectedStat == AffectedStat.Damage)
        {
            return pieces.Count * 3;
        }
        return 0;
    }

    /// <summary>
    /// instant
    /// next # turns, unique
    /// for three turns burn enemy doing
    /// 1x earth piece damage
    /// 1.5x earth piece damage
    /// 2.25x earth piece damage
    /// </summary>
    void OceanVents()
    {
        UniqueEffect oceanVentsBurn = OceanVentsBurn;
        uniqueList.Add((oceanVentsBurn.Method,3));
        List<PieceScriptable> pieces = BoardManager.instance.GetBoardPieces();
        OceanVentsInitDamage = pieces.Find(p => p.cardType == cardType.earth).combatValue;
    }
    void OceanVentsBurn()
    {
        float damage = OceanVentsInitDamage * Mathf.Pow(1.5f, 3 - findActiveEffect("OceanVentsBurn").Turns);
        GameManager.instance.currentEnemy.TakeDamage((int)MathF.Round(damage));
    } 
    int OceanVentsInitDamage = 0;


    /// <summary>
    /// next turn, addition
    /// earth cards stats +1
    /// </summary>
    int Pebble(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements) // earth earth combo
    {
        return pieces.Count(piece => piece.cardType == cardType.earth);
    }

    void PetrichorMudslide(){}

    /// <summary>
    /// instant
    /// do 3-5 burst of damage with each burst doing 3-7 damage
    /// </summary>
    void Plasma() // fire water earth combo
    {
        for(int i = 0; i < UnityEngine.Random.Range(3, 6); i++)
        {
            GameManager.instance.currentEnemy.TakeDamage(UnityEngine.Random.Range(3, 8));
        }
    }

    /// <summary>
    /// instant
    /// stun the enemy
    /// </summary>
    void Quicksand() // water earth combo
    {
        // stun effect
        GameManager.instance.enemyStunned = true;
    }

    /// <summary>
    /// persistent, addition
    /// +1 to stats of earth card for each turn played with an earth card in a row
    /// </summary>
    /// <returns></returns>
    int Rockslide(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements) // earth earth earth earth combo
    {
        int earthcount = pieces.Count(p => p.cardType == cardType.earth);
        int consecutiveturns = findActiveEffect("Rockslide").Turns * -1;

        if (earthcount * consecutiveturns == 0)
        {
            removeEffect("Rockslide");
        }
        return earthcount * consecutiveturns;
    }

    /// <summary>
    /// instant
    /// you get 50% of missing health back
    /// </summary>
    void Sandbar() // water water water earth combo
    {
        float healValue = (Player.instance.maxHealth - Player.instance.health) * 0.5f;
        Player.instance.HealHealth((int)healValue);
    }

    void SandyFlow(){}

    void Scorch(){}

    /// <summary>
    /// instant
    /// next # turns, unique
    /// going from the lowest to highest health stat of the pieces in this spell
    /// heal yourself by the first health stat
    /// then the combined first and second health stat
    /// finally all three combined together.
    /// </summary>
    void Simmer() // fire water water combo
    {
        // get pieces list
        List<PieceScriptable> pieces = BoardManager.instance.GetBoardPieces();

        // assign orded healing values to simmervalue array
        SimmerValue = pieces.Select(p => p.healingValue).ToArray();
        Array.Sort(SimmerValue);

        // add simmer heal to unique list
        UniqueEffect simmerHeal = SimmerHeal;
        uniqueListBuffer.Add((simmerHeal.Method, 3));
    }
    void SimmerHeal()
    {
        // 4 - 3 = 1
        // 4 - 2 = 2
        // 4 - 1 = 3
        int healValue = SimmerValue.Take(4 - findActiveEffect("SimmerHeal").Turns).Sum();
        Player.instance.HealHealth(healValue);
    }
    int[] SimmerValue = {0,0,0};

    /// <summary>
    /// next # turns, unique
    /// + 5-10 total damage on turn if air piece played
    /// 2 turns without air piece will stun you on third
    /// </summary>
    void Sinkhole() // earth air air combo
    {
        // air check
        List<PieceScriptable> pieces = BoardManager.instance.GetBoardPieces();
        if(pieces.Any(p => p.cardType == cardType.air))
        {
            GameManager.instance.currentEnemy.TakeDamage(UnityEngine.Random.Range(5, 11));
        }
        else
        {
            SinkholeStuncheck = true;
        }
        // second turn check to stun or not
        if(findActiveEffect("Sinkhole").Turns == 1 && SinkholeStuncheck)
        {
            GameManager.instance.playerStunned = true;
            SinkholeStuncheck = false; 
        }
    }
    bool SinkholeStuncheck = false;

    /// <summary>
    /// instant
    /// 50% chance to stun enemy
    /// next turn, raw multiplier
    /// 50% chance for double damage
    /// </summary>
    void Smog() // fire fire fire air combo
    {
        if(UnityEngine.Random.Range(0, 0.5f) < 0.5f)
        {
            GameManager.instance.enemyStunned = true;
        }
        NumberEffect smogDamage = SmogDamage;
        rawMultiplierListBuffer.Add((smogDamage.Method,1));
    }
    float SmogDamage(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements)
    {
        if(affectedStat == AffectedStat.Damage && UnityEngine.Random.Range(0, 0.5f) < 0.5f)
        {
            return 2;
        }
        return 1;
    }

    /// <summary>
    /// next turn, addition
    /// fire cards stats +1
    /// </summary>
    int Spark(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements) // fire fire combo
    {
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
    /// instant
    /// +20 health
    /// </summary>
    void Steam() //fire water combo
    {
        Player.instance.HealHealth(20);
    }

    void SteamRoom(){}

    /// <summary>
    /// instant
    /// next turn, addition
    /// earth or water card +2 stats
    /// next turn, add to multiplier
    /// if earth or water card +2 to multiplier
    /// </summary>
    void Swamp() // water water earth earth combo
    {
        NumberEffect swampAddition = SwampAddition;
        additionListBuffer.Add((swampAddition.Method, 1));
        NumberEffect swampMultiplier = SwampMultiplier;
        addToMultiplierListBuffer.Add((swampMultiplier.Method, 1));
    }
    float SwampAddition(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements)
    {
        return pieces.Count(p => p.cardType == cardType.water || p.cardType == cardType.earth) * 2;
    }
    float SwampMultiplier(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements)
    {
        return pieces.Any(p => p.cardType == cardType.water || p.cardType == cardType.earth) ? 2 : 0;
    }
    
    /// <summary>
    /// next # turns, addition
    /// air cards have their stats doubled
    /// </summary>
    /// <returns></returns>
    int Tornado(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements) // air air air air combo
    {
        if(affectedStat == AffectedStat.Damage)
        return pieces.Where(p => p.cardType == cardType.air).Sum(p => p.combatValue);
        else if(affectedStat == AffectedStat.Gold)
        return pieces.Where(p => p.cardType == cardType.air).Sum(p => p.goldValue);
        else if(affectedStat == AffectedStat.Health)
        return pieces.Where(p => p.cardType == cardType.air).Sum(p => p.healingValue);
        else
        return 0;
    }

    void Tsunami(){}

    /// <summary>
    /// multiple effects, instant
    /// next turn, raw multiplier
    /// 2x damage
    /// next # turns, unique
    /// deal 5-10 damage
    /// </summary>
    void VolcanicRock() // fire earth combo
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
    float VolcanicRockMultiplierEffect(List<PieceScriptable> pieces, AffectedStat affectedStat)
    {  
        return affectedStat.HasFlag(AffectedStat.Damage) ? 2 : 1;
    }
    void VolcanicRockBurnEffect()
    {
        // damage to be dealt
        GameManager.instance.currentEnemy.TakeDamage(UnityEngine.Random.Range(5,11));
    }

    /// <summary>
    /// next turn, add to multiplier
    /// +1 multiplier if >= 2 wave cards
    /// </summary>
    int Wave(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements) // water water water combo
    {
        return pieces.Count(p => p.cardType == cardType.water) >= 2 ? 1 : 0;
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
    /// next turn, add to multiplier
    /// +1 multiplier if >= 2 air cards
    /// </summary>
    int WindTunnel(List<PieceScriptable> pieces, AffectedStat affectedStat = AffectedStat.NoRequirements) // air air air combo
    {
        return pieces.Count(p => p.cardType == cardType.air) >= 2 ? 1 : 0;
    }
    #endregion
}
