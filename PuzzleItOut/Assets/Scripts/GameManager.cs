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

    TurnState turnState;

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

    public void DoTurn()
    {
        //called when attack button is clicked
        attackButton.interactable = false;
        CombatManager.Instance.CalculateDamage();
        CombatManager.Instance.CalculateGold();
        CombatManager.Instance.CalculateHealth();

        EndTurn();

    }
    void EndTurn()
    {
        if (currentEnemy.health <= 0)
        {
            //win
            SceneManager.LoadScene(3);
            return;
        }
        else if (Player.instance.GetHealth() <= 0)
        {
            //lose
            SceneManager.LoadScene(2);
            return;
        }

        if(turnState == TurnState.playerTurn)
        {
            
            DeckManager.instance.DiscardBoard();
            DeckManager.instance.DrawPiece();
            turnState = TurnState.enemyTurn;
            DoEnemyTurn();

        }
        else if (turnState == TurnState.enemyTurn)
        {
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
