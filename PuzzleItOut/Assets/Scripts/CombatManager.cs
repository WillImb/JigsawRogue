using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    //List<ComboScriptable> ComboList;

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

    public void TakeDamage(float damage)
    {
        return;
    }
    private float CalculateDamage()
    {
        float damage = 0;
        return damage;
    }

    public void FindCombo(Piece[] pieces)
    {

    }
}
