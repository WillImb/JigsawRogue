using System.Collections.Generic;
using UnityEngine;

public class Spellbook : MonoBehaviour
{
    public static Spellbook instance;
    public List<ComboScriptable> combosUnlocked;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool ComboUnlocked(ComboScriptable combo)
    {
        if(!combosUnlocked.Contains(combo))
        {
            return true;
        }
        return false;
    }

    public bool UnlockCombo(ComboScriptable combo)
    {
        if(ComboUnlocked(combo))
        {
            combosUnlocked.Add(combo);
            return true;
        }
        return false;
    }
}
