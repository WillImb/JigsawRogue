using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spellbook : MonoBehaviour
{
    public static Spellbook instance;
    public List<ComboScriptable> combosUnlocked;

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
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // nothing done in on scene loaded atm
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // nothing done in on scene loaded atm
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
}
