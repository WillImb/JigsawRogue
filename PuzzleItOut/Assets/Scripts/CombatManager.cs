using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    List<ComboScriptable>  spellBook;

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
        if (GameManager.instance.currentEnemy.health <= 0)
        {
            //the win scene index
            SceneManager.LoadScene(3);
        }
        return;
    }

    public void TakeDamage(float damage)
    {

        if(GameManager.instance.playerHealth <= 0)
        {
            //the lose scene index
            SceneManager.LoadScene(2);
        }
        return;
    }
    private float CalculateDamage()
    {
        float damage = 0;
        return damage;
    }

    public int FindCombo(List<PieceScriptable> currentCombo)
    {
        //go through each possible combo
        for (int i = 0; i < spellBook.Count; i++)
        {
            //Easy way, if we standardize the way the combos are orders -> ex. when combo is submitted order it fire, water, earth, wind
            if (spellBook[i].comboList == currentCombo)
            {
                return i;
            }
        }

        //just return -1 if none found
        return -1;
    }
}
