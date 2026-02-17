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


   
}
