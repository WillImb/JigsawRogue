using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    public List<ComboScriptable>  spellBook;

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
        //go through each possible combo
        for (int i = 0; i < spellBook.Count; i++)
        {
            //Easy way, if we standardize the way the combos are orders -> ex. when combo is submitted order it fire, water, earth, wind
            if (spellBook[i].comboList == currentCombo)
            {
                return i;
            }
        }

        //just return -1 if none found
        return -1;
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
