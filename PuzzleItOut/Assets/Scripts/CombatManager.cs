using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void DealDamage(float damage)
    {
        return;
    }

    

    public int FindCombo(List<PieceScriptable> currentCombo)
    {
        // organizing combo
        // Sorts combo by the enum order in the PieceScriptable. fire, water, air, earth
        
        currentCombo.Sort(CompareByCardType);
        //go through each possible combo
        for (int i = 0; i < Spellbook.instance.combosUnlocked.Count; i++)
        {
            //Easy way, if we standardize the way the combos are orders -> ex. when combo is submitted order it fire, water, earth, wind
            if (Spellbook.instance.combosUnlocked[i].comboList == currentCombo)
            {
                return i;
            }
        }

        //just return -1 if none found
        return -1;
    }
    public static int CompareByCardType(PieceScriptable piece1, PieceScriptable piece2) {
        return piece1.cardType.CompareTo(piece2.cardType);
    }
    public float CalculateDamage(ComboScriptable combo)
    {
        return combo.Damage();
    }
    public float CalculateGold(ComboScriptable combo)
    {
        return combo.Gold();
    }

    public float CalculateHealth(ComboScriptable combo)
    {
        return combo.Health();
    }
}
