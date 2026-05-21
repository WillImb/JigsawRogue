using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
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

    public void DealDamage(int damage)
    {
        return;
    }

    public int FindCombo(List<PieceScriptable> currentCombo)
    {
        List<cardType> submittedTypes = currentCombo
            .Select(p => p.cardType)
            .OrderBy(t => (int)t)
            .ToList();

        //Debug.Log($"Submitted types: {string.Join(", ", submittedTypes)}");

        for (int i = 0; i < Spellbook.instance.combosUnlocked.Count; i++)
        {
            List<cardType> comboTypes = Spellbook.instance.combosUnlocked[i].requiredTypes
                .OrderBy(t => (int)t)
                .ToList();

            if (submittedTypes.SequenceEqual(comboTypes))
            {
                //Debug.Log($"Match found — '{Spellbook.instance.combosUnlocked[i].comboName}'");
                return i;
            }
        }

        Debug.Log("No match found");
        return -1;
    }

    public int CalculateDamage(ComboScriptable combo, List<PieceScriptable> pieces)
    {
        int result = 0;
        // base value from pieces
        result += pieces.Sum(p => p.combatValue);

        // addition value from effects
        for (int index = 0 ; index < SpecialComboManager.Instance.additionList.Count; index++)
        {
            result += (int)SpecialComboManager.Instance.additionList[index].Effect.Invoke(SpecialComboManager.Instance, new object[] {pieces});
        }

        // value from multiplier
        int multiplier = (int)combo.multiplier;

        for (int index = 0 ; index < SpecialComboManager.Instance.addToMultiplierList.Count; index++)
        {
            multiplier += (int)SpecialComboManager.Instance.addToMultiplierList[index].Effect.Invoke(SpecialComboManager.Instance, new object[] {pieces});
        }
        // add to multiplier value from effects
        result *= multiplier;

        return result;
    }

    public int CalculateGold(ComboScriptable combo, List<PieceScriptable> pieces)
    {
        int result = 0;

        result += pieces.Sum(p => p.goldValue);

        for (int index = 0 ; index < SpecialComboManager.Instance.additionList.Count; index++)
        {
            result += (int)SpecialComboManager.Instance.additionList[index].Effect.Invoke(SpecialComboManager.Instance, new object[] {pieces});
        }

        int multiplier = (int)combo.multiplier;

        for (int index = 0 ; index < SpecialComboManager.Instance.addToMultiplierList.Count; index++)
        {
            multiplier += (int)SpecialComboManager.Instance.addToMultiplierList[index].Effect.Invoke(SpecialComboManager.Instance, new object[] {pieces});
        }

        result *= multiplier;
        //int result = combo.Gold(pieces);
        return result;
    }

    public int CalculateHealth(ComboScriptable combo, List<PieceScriptable> pieces)
    {
        int result = 0;

        result += pieces.Sum(p => p.healingValue);

        for (int index = 0 ; index < SpecialComboManager.Instance.additionList.Count; index++)
        {
            result += (int)SpecialComboManager.Instance.additionList[index].Effect.Invoke(SpecialComboManager.Instance, new object[] {pieces});
        }

        int multiplier = (int)combo.multiplier;

        for (int index = 0 ; index < SpecialComboManager.Instance.addToMultiplierList.Count; index++)
        {
            multiplier += (int)SpecialComboManager.Instance.addToMultiplierList[index].Effect.Invoke(SpecialComboManager.Instance, new object[] {pieces});
        }
        
        result *= multiplier;
        //int result = combo.Health(pieces);
        return result;
    }
}