using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float playerHealth;
    public float money;

    public int turn; //0 for player 1 for enemy;

    public Enemy currentEnemy; // change to type enemy when rish adds script

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void StartGame()
    {
        DeckManager.instance.DrawPiece();
        DeckManager.instance.DrawPiece();
        DeckManager.instance.DrawPiece();
        DeckManager.instance.DrawPiece();
        DeckManager.instance.DrawPiece();
        DeckManager.instance.DrawPiece();
    }


   
}
