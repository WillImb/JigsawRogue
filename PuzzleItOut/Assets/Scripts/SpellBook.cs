using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spellbook : MonoBehaviour
{
    public static Spellbook instance;

    public List<ComboScriptable> combosUnlocked;

    //used to track which combos the player has not unlocked; Only unlocked when player discovers;
    public List<ComboScriptable> combosLocked;

    //Special Combos
    //a alist of all the combos that are discovered and whether the player has the special combo unlocked
    public Dictionary<ComboScriptable, bool> unlockedSpecialCombos;


    void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


        //Temporary unlock all special Combos
        InitializedSpecialUnlocked();
    }


    void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "EndOfDemoScene")
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
                instance = null;
            }
        }
    }

    public bool ComboUnlocked(ComboScriptable combo)
    {
        if (!combosUnlocked.Contains(combo))
        {
            return true;
        }
        return false;
    }

    public bool UnlockCombo(ComboScriptable combo)
    {
        if (ComboUnlocked(combo))
        {
            combosUnlocked.Add(combo);
            return true;
        }
        return false;
    }

    //When the player cast an undiscovered spell call this method
    //adds to the unlocked combos and adds to the special combos dictionary as locked
    public void DiscoverSpellbook(ComboScriptable newCombo)
    {
        combosUnlocked.Add(newCombo);
        combosLocked.Remove(newCombo);

        unlockedSpecialCombos.Add(newCombo, false);

    }

    public bool CheckSpecialUnlockedStatus(ComboScriptable combo)
    {
        return unlockedSpecialCombos[combo];
    }

    public void UnlockSpecialCombo(ComboScriptable combo)
    {
        unlockedSpecialCombos[combo] = true;
    }
    void InitializedSpecialUnlocked()
    {
        unlockedSpecialCombos = new Dictionary<ComboScriptable, bool>();
        if (combosUnlocked != null)
        {
            foreach (ComboScriptable c in combosUnlocked)
            {                
                unlockedSpecialCombos.Add(c, false);
            }
        }        
    }
}
