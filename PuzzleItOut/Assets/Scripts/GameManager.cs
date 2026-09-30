using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // persistent round tracker across scene reloads
    public static int currentRound = 1;

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

    public Enemy currentEnemy;
    public bool enemyStunned = false;
    public bool acidRainDamageReduced = false;
    public bool ashfallDamageReduction = false;
    public bool petrichorMudwallDamageReduction = false;
    public bool enemyRebound = false;
    public bool playerStunned = false;

    void Awake()
    {
        if (instance == null)
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

    void StartGame()
    {
        DeckManager.instance.ShuffleDeck();
        DeckManager.instance.SpawnPieces();
        DeckManager.instance.DrawPiecesTillMax();

        // make sure enemy sprite and state match the round type
        if (currentEnemy != null)
        {
            currentEnemy.SetupEnemy();
        }
    }

    /// <summary>
    /// Checks if the current round is a Boss round every 3 rounds
    /// </summary>
    public bool IsBossRound()
    {
        return currentRound % 3 == 0;
    }

    public void DoTurn(int castType)
    {
        attackButton.interactable = false;
        specialToggle.interactable = false;

        Camera.main.GetComponent<CameraShake>().StartShake();

        List<PieceScriptable> currentPieces = BoardManager.instance.GetBoardPieces();
        ComboScriptable combo = BoardManager.instance.activeCombo;

        if (combo == null)
        {
            EndTurn();
            return;
        }

        if (!combo.isForbidden)
        {
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

        if (!isSpecial)
        {
            currentEnemy.TakeDamage(CombatManager.Instance.CalculateDamage(combo, currentPieces));
            float goldAmt = CombatManager.Instance.CalculateGold(combo, currentPieces);
            StartCoroutine(VFXManager.instance.goldCoroutine(goldAmt));

            Player.instance.HealHealth(CombatManager.Instance.CalculateHealth(combo, currentPieces));
        }
        else if (isSpecial)
        {
            SpecialComboManager.Instance.addEffect(combo);
        }

        EndTurn();
    }

    void EndTurn()
    {
        if (currentEnemy.health <= 0)
        {
            DeckManager.instance.DiscardBoard();
            DeckManager.instance.DrawPiecesTillMax();

            BoardManager.instance.ValidateBoard();
            BoardManager.instance.UpdateCostImage();

            currentEnemy.gameObject.SetActive(false);
            return;
        }
        else if (Player.instance.GetHealth() <= 0)
        {
            // reset round count on game over
            currentRound = 1;
            SceneManager.LoadScene(2);
            return;
        }

        if (turnState == TurnState.playerTurn)
        {
            SpecialComboManager.Instance.uniqueList.ForEach(e => e.Effect.Invoke(SpecialComboManager.Instance, null));
            DeckManager.instance.DiscardBoard();
            DeckManager.instance.DrawPiecesTillMax();
            turnState = TurnState.enemyTurn;

            BoardManager.instance.ValidateBoard();
            BoardManager.instance.UpdateCostImage();

            Invoke("DoEnemyTurn", 1);
        }
        else if (turnState == TurnState.enemyTurn)
        {
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
        if (enemyStunned)
        {
            enemyStunned = false;
            Invoke("EndTurn", 1);
        }
        else
        {
            VFXManager.instance.SpawnParticle(new Vector3(0, 1, 0), 4);
            currentEnemy.Invoke("DealDamage", .35f);
            Invoke("EndTurn", 1);
        }
    }

    public void WinRound()
    {
        // increment round counter before shop transition
        currentRound++;
        TransitionManager.instance.ActivateTransition("ShopTransition");
    }

    public void SetSpecial()
    {
        isSpecial = specialToggle.isOn;
        BoardManager.instance.UpdateCostImage();
    }
}
