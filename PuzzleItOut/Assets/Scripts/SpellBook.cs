using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spellbook : MonoBehaviour
{
    public static Spellbook instance;

    public List<ComboScriptable> combosUnlocked;
    public int lvl;
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

    public void AddToSpellbook(GameObject clickedObject)
    {
        
    }
}
