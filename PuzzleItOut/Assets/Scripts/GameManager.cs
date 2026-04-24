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
        PanelManager.instance.DisableButtons("2");
        DeckManager.instance.gameObject.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {

    }

    void StartGame()
    {
        DeckManager.instance.ShuffleDeck();

        DeckManager.instance.DrawPiecesTillMax();

    }

    public void DoTurn()
    {
        attackButton.interactable = false;
        Camera.main.GetComponent<CameraShake>().StartShake();

        List<PieceScriptable> currentPieces = BoardManager.instance.GetBoardPieces();

        int comboIndex = CombatManager.Instance.FindCombo(currentPieces);
        ComboScriptable combo = comboIndex >= 0 ? Spellbook.instance.combosUnlocked[comboIndex] : null;

        

        if (combo != null)
        {
            currentEnemy.TakeDamage(CombatManager.Instance.CalculateDamage(combo, currentPieces));
            float goldAmt = CombatManager.Instance.CalculateGold(combo, currentPieces);
            StartCoroutine(VFXManager.instance.goldCoroutine(goldAmt));

            Player.instance.HealHealth(CombatManager.Instance.CalculateHealth(combo, currentPieces));           
        }
        else
        {
            currentEnemy.TakeDamage(0);
        }

        EndTurn();
    }
    void EndTurn()
    {
        if (currentEnemy.health <= 0)
        {
            
            //win
            //SceneManager.LoadScene(4)
            TransitionManager.instance.ActivateTransition("ShopTransition");
            currentEnemy.gameObject.SetActive(false);
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
            DeckManager.instance.DrawPiecesTillMax();
            //switdh to enemy's turn
            turnState = TurnState.enemyTurn;
            Invoke("DoEnemyTurn",1);

        }
        else if (turnState == TurnState.enemyTurn)
        {
            //switch to enemy's turn
            
            turnState = TurnState.playerTurn;
        }
        
    }
    void DoEnemyTurn()
    {
        VFXManager.instance.SpawnParticle(new Vector3(0, 1, 0), 4);
        currentEnemy.Invoke("DealDamage",.35f);
        Invoke("EndTurn",1);
    }
   
}
