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

    public void DealDamage(float damage)
    {
        return;
    }



    public int FindCombo(List<PieceScriptable> currentCombo)
    {
        List<cardType> submittedTypes = currentCombo
            .Select(p => p.cardType)
            .OrderBy(t => (int)t)
            .ToList();

        Debug.Log($"Submitted types: {string.Join(", ", submittedTypes)}");

        for (int i = 0; i < Spellbook.instance.combosUnlocked.Count; i++)
        {
            List<cardType> comboTypes = Spellbook.instance.combosUnlocked[i].requiredTypes
                .OrderBy(t => (int)t)
                .ToList();

            if (submittedTypes.SequenceEqual(comboTypes))
            {
                Debug.Log($"Match found — '{Spellbook.instance.combosUnlocked[i].comboName}'");
                return i;
            }
        }

        Debug.Log("No match found");
        return -1;
    }
    public static int CompareByCardType(PieceScriptable piece1, PieceScriptable piece2) {
        return piece1.cardType.CompareTo(piece2.cardType);
    }

    public float CalculateDamage(ComboScriptable combo, List<PieceScriptable> pieces)
    {
        float result = combo.Damage(pieces);
        Debug.Log($"'{combo.comboName}' dealt {result} damage");
        return result;
    }

    public float CalculateGold(ComboScriptable combo, List<PieceScriptable> pieces)
    {
        float result = combo.Gold(pieces);
        Debug.Log($"'{combo.comboName}' generated {result} gold");
        return result;
    }

    public float CalculateHealth(ComboScriptable combo, List<PieceScriptable> pieces)
    {
        float result = combo.Health(pieces);
        Debug.Log($"'{combo.comboName}' healed {result} health");
        return result;
    }
}
