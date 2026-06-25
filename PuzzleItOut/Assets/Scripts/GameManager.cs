using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float money;

    public Button attackButton;
    public Toggle specialToggle;

    public bool isSpecial;


    

    public enum TurnState
    {
        playerTurn,
        enemyTurn
    }

    TurnState turnState = TurnState.playerTurn;

    public Enemy currentEnemy; // change to type enemy when rish adds script
    public bool enemyStunned = false;
    public bool acidRainDamageReduced = false;
    public bool ashfallDamageReduction = false;
    public bool petrichorMudwallDamageReduction = false;
    public bool enemyRebound = false;
    public bool playerStunned = false;


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
        PanelManager.instance.DisableButtons("2,4");
        specialToggle.interactable = false;

        DeckManager.instance.gameObject.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void StartGame()
    {
        DeckManager.instance.ShuffleDeck();

        DeckManager.instance.SpawnPieces();
        DeckManager.instance.DrawPiecesTillMax();


    }

    public void DoTurn(int castType)
    {
        //disable cast buttons
        attackButton.interactable = false;
        specialToggle.interactable = false;

        //camera shake
        Camera.main.GetComponent<CameraShake>().StartShake();


        List<PieceScriptable> currentPieces = BoardManager.instance.GetBoardPieces();

         
        ComboScriptable combo = BoardManager.instance.activeCombo;

        if (combo == null)
        {
            EndTurn();
            return;
        }
        

        //for non forbidden combos
        if (!combo.isForbidden)
        {
            // check mana
            if (Player.instance.GetMana() < combo.GetManaCost())
            {
                attackButton.interactable = true;
                specialToggle.interactable = false;

                return;
            }
            else
            {
                Player.instance.SpendMana(combo.GetManaCost());
            }
        }
        //forbidden combos
        else
        {
            if (isSpecial)
            {

                if (Player.instance.GetHealth() <= 25)
                {
                    attackButton.interactable = true;
                    specialToggle.interactable = false;
                    return;
                }
                else
                {

                    Player.instance.TakeDamage(25);

                }
            }
            else
            {
                if (Player.instance.GetHealth() <= 10)
                {
                    attackButton.interactable = true;
                    specialToggle.interactable = false;
                    return;
                }
                else
                {

                    Player.instance.TakeDamage(10);

                }
            }
        }


        //Cast type can be normal or special cast
        //normal spell
        if (!isSpecial)
        {
            currentEnemy.TakeDamage(CombatManager.Instance.CalculateDamage(combo, currentPieces));
            float goldAmt = CombatManager.Instance.CalculateGold(combo, currentPieces);
            StartCoroutine(VFXManager.instance.goldCoroutine(goldAmt));

            Player.instance.HealHealth(CombatManager.Instance.CalculateHealth(combo, currentPieces));
        }  
        //if a special combo
        else if(isSpecial)
        {
            
            SpecialComboManager.Instance.addEffect(combo);
        }
        // if (combo != null)
        // {
        //     currentEnemy.TakeDamage(CombatManager.Instance.CalculateDamage(combo, currentPieces));
        //     float goldAmt = CombatManager.Instance.CalculateGold(combo, currentPieces);
        //     StartCoroutine(VFXManager.instance.goldCoroutine(goldAmt));

        //     Player.instance.HealHealth(CombatManager.Instance.CalculateHealth(combo, currentPieces));           
        // }
        // else
        // {
        //     currentEnemy.TakeDamage(0);
        // }
        

        EndTurn();
    }
    void EndTurn()
    {
        if (currentEnemy.health <= 0)
        {

            //win
            //SceneManager.LoadScene(4)

            DeckManager.instance.DiscardBoard();
            DeckManager.instance.DrawPiecesTillMax();

            BoardManager.instance.ValidateBoard();
            BoardManager.instance.UpdateCostImage();

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
            SpecialComboManager.Instance.uniqueList.ForEach(e => e.Effect.Invoke(SpecialComboManager.Instance, null));
            DeckManager.instance.DiscardBoard();
            DeckManager.instance.DrawPiecesTillMax();
            //switch to enemy's turn
            turnState = TurnState.enemyTurn;

            BoardManager.instance.ValidateBoard();
            BoardManager.instance.UpdateCostImage();

            Invoke("DoEnemyTurn",1);

        }
        else if (turnState == TurnState.enemyTurn)
        {
            //switch to player's turn
            turnState = TurnState.playerTurn;
            SpecialComboManager.Instance.cleanTurnLists();
            SpecialComboManager.Instance.moveFromBuffer();
            if (playerStunned)
            {
                playerStunned = false;
                EndTurn(); 
            }
        }
        
    }
    void DoEnemyTurn()
    {
        //stunned
        if (enemyStunned)
        {
            enemyStunned = false;
            Invoke("EndTurn",1);
        }
        else // not stunned
        {   
            VFXManager.instance.SpawnParticle(new Vector3(0, 1, 0), 4);
            currentEnemy.Invoke("DealDamage",.35f);
            Invoke("EndTurn",1);
        }
    }

    public void WinRound()
    {
        TransitionManager.instance.ActivateTransition("ShopTransition");
    }

    public void SetSpecial()
    {
       
        isSpecial = specialToggle.isOn;
        BoardManager.instance.UpdateCostImage();
    }

   

}
