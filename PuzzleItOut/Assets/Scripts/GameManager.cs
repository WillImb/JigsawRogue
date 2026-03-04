using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float money;

    public Button attackButton;


    public enum TurnState
    {
        playerTurn,
        enemyTurn
    }

    TurnState turnState = TurnState.playerTurn;

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
        DeckManager.instance.ShuffleDeck();
        DeckManager.instance.DrawPiece();
        DeckManager.instance.DrawPiece();
        DeckManager.instance.DrawPiece();
        DeckManager.instance.DrawPiece();
        DeckManager.instance.DrawPiece();
        DeckManager.instance.DrawPiece();
    }

    public void DoTurn()
    {
        //called when attack button is clicked
        attackButton.interactable = false;

        //Currently not working because spell book is empty
        ComboScriptable combo = CombatManager.Instance.spellBook[0];
        currentEnemy.TakeDamage(CombatManager.Instance.CalculateDamage(combo));

        CombatManager.Instance.CalculateGold(combo);
        CombatManager.Instance.CalculateHealth(combo);

        EndTurn();

    }
    void EndTurn()
    {
        if (currentEnemy.health <= 0)
        {
            //win
            SceneManager.LoadScene(4);
            return;
        }
        else if (Player.instance.GetHealth() <= 0)
        {
            //lose
            SceneManager.LoadScene(2);
            return;
        }


        //The end of the Players turn
        if(turnState == TurnState.playerTurn)
        {
            
            DeckManager.instance.DiscardBoard();
            DeckManager.instance.DrawPiece();

            //switdh to enemy's turn
            turnState = TurnState.enemyTurn;
            Invoke("DoEnemyTurn",1);

        }
        else if (turnState == TurnState.enemyTurn)
        {
            //switch to enemy's turn
            turnState = TurnState.playerTurn;
            attackButton.interactable = true;
        }
        
    }
    void DoEnemyTurn()
    {
        currentEnemy.DealDamage();
        EndTurn();
    }
   
}
