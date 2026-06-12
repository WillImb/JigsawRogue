using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum IngredientType { Combat, Health, Gold, Probability }

[CreateAssetMenu(fileName = "Combo Object", menuName = "ScriptableObjects/ComboScriptableObject", order = 2)]
public class ComboScriptable : ScriptableObject
{
    [Header("Identity")]
    public string comboName;

    [Header("Required Card Types")]
    public List<cardType> requiredTypes;

    [Header("Is this combo A Forbidden Combo?")]
    public bool isForbidden;

    // [Header("Active Ingredients")]
    // public List<IngredientType> activeIngredients;

    [Header("Multiplier")]
    public float multiplier = 2f;

    [Header("Combo Effects")]
    public bool specialUnlocked = false;
    public SpecialComboManager.priority priority;
    public int turns = 1;

  
    //New mana Calculator
    public int GetManaCost()
    {
        if(requiredTypes == null)
        {
            return 0;
        }
        else if (GameManager.instance.isSpecial)
        {
            return requiredTypes.Count * 5;
        }
        else
        {
            return requiredTypes.Count * 2 - 3;
        }
    }
}