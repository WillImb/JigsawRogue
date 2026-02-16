using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float playerHealth;
    public float money;

    public bool paused = false;
    public int turn; //0 for player 1 for enemy;

    public GameObject currentEnemy; // change to type enemy when rish adds script

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            paused = !paused;
        }
    }


    public int FindCombo(List<List<PieceScriptable>> spellBook, List<PieceScriptable> currentCombo)
    {
        //go through each possible combo -- later probably change this too a list of combo scriptables or such
        for(int i = 0; i < spellBook.Count; i++)
        {
            //Easy way, if we standardize the way the combos are orders -> ex. when combo is submitted order it fire, water, earth, wind
            if (spellBook[i] == currentCombo)
            {
                return i;
            }
        }

        //just return -1 if none found
        return -1;
    }
}
