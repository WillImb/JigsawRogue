using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Combo Object", menuName = "ScriptableObjects/ComboScriptableObject", order = 2)]
public class ComboScriptable : ScriptableObject
{
    public string comboName;
    public List<PieceScriptable> comboList;
    

    public float Damage()
    {
        float damageTotal = 0;
        foreach(PieceScriptable p in comboList)
        {
            damageTotal += p.baseDamange;
        }
        return damageTotal;
    }
    public float Gold()
    {
        float goldTotal = 0;
        foreach (PieceScriptable p in comboList)
        {
            goldTotal += p.baseDamange;
        }
        return goldTotal;
    }
    public float Health()
    {
        float healthTotal = 0;
        foreach (PieceScriptable p in comboList)
        {
            healthTotal += p.baseDamange;
        }
        return healthTotal;
    }

}


