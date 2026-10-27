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

    public bool isStorymode;
    public int level;
    public GameObject storyManagerPrefab;
    public SliderManager sliderManager;

    public Transform enemySpawner;

    public enum TurnState
    {
        playerTurn,
        enemyTurn
    }

    TurnState turnState = TurnState.playerTurn;

    public EnemyParent currentEnemy;


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
        if (isStorymode)
        {
            Instantiate(storyManagerPrefab);
            //For the future - In story manager load saved data from files

            currentEnemy = Instantiate(StoryManager.instance.enemiesPrefab[level].gameObject,enemySpawner).GetComponent<EnemyParent>();
            sliderManager.enemy = currentEnemy;

            if (StoryManager.instance.HasDialog(level))
            {
                StartCoroutine(StoryManager.instance.ExchangeDialog(level));
            }


            

        }

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


        List<Piece> currentPieces = BoardManager.instance.GetBoardPieces();

         
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

            //gold is added when gold particle collideds with money bag [In VFXManager -> MoneyBag script]... should change this later. Currently just ive the player all the gold at once
            float goldAmt = CombatManager.Instance.CalculateGold(combo, currentPieces);
            GoldManager.Instance.AddGold((int)goldAmt);

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

            WinRound();
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
            currentEnemy.Invoke("Attack",.35f);
            Invoke("EndTurn",1);
        }
    }

    public void WinRound()
    {
        TransitionManager.instance.ActivateTransition("ShopTransition");
        level++;
    }

    public void SetSpecial()
    {
       
        isSpecial = specialToggle.isOn;
        BoardManager.instance.UpdateCostImage();

        //Check if player does not have special combo unlocked
        if (specialToggle.isOn)
        {
            if (!Spellbook.instance.CheckSpecialUnlockedStatus(BoardManager.instance.activeCombo))
            {
                PanelManager.instance.DisableButtons("2");
            }
        }
        else
        {
            if (BoardManager.instance.activeCombo)
            {
                PanelManager.instance.EnableButtons("2");
            }

        }

    }
    public void ResetSpecialCheck()
    {
        specialToggle.isOn = false;
        isSpecial = false;
    }
   

}
