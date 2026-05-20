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

    [Header("Active Ingredients")]
    public List<IngredientType> activeIngredients;

    [Header("Multiplier")]
    public float multiplier = 2f;

    [Header("Combo Effects")]
    public SpecialComboManager.priority priority;
    public int turns = 1;
    public int Damage(List<PieceScriptable> pieces)
    {
        int sum = pieces.Sum(p => p.combatValue);
        return activeIngredients.Contains(IngredientType.Combat) ? (int)(sum * multiplier) : sum;
    }

    public int Health(List<PieceScriptable> pieces)
    {
        int sum = pieces.Sum(p => p.healingValue);
        return activeIngredients.Contains(IngredientType.Health) ? (int)(sum * multiplier) : sum;
    }

    public int Gold(List<PieceScriptable> pieces)
    {
        int sum = pieces.Sum(p => p.goldValue);
        return activeIngredients.Contains(IngredientType.Gold) ? (int)(sum * multiplier) : sum;
    }

    public float Probability(List<PieceScriptable> pieces)
    {
        float sum = pieces.Sum(p => p.probabilityValue);
        return activeIngredients.Contains(IngredientType.Probability) ? sum * multiplier : sum;
    }
}