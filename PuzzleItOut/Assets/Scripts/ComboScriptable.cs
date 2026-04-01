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

    public float Damage(List<PieceScriptable> pieces)
    {
        float sum = pieces.Sum(p => p.combatValue);
        return activeIngredients.Contains(IngredientType.Combat) ? sum * multiplier : sum;
    }

    public float Health(List<PieceScriptable> pieces)
    {
        float sum = pieces.Sum(p => p.healingValue);
        return activeIngredients.Contains(IngredientType.Health) ? sum * multiplier : sum;
    }

    public float Gold(List<PieceScriptable> pieces)
    {
        float sum = pieces.Sum(p => p.goldValue);
        return activeIngredients.Contains(IngredientType.Gold) ? sum * multiplier : sum;
    }

    public float Probability(List<PieceScriptable> pieces)
    {
        float sum = pieces.Sum(p => p.probabilityValue);
        return activeIngredients.Contains(IngredientType.Probability) ? sum * multiplier : sum;
    }
}