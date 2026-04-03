using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float money;
    public Button attackButton;
    public int round;

    public Enemy currentEnemy; // change to type enemy when rish adds script

    public enum TurnState
    {
        playerTurn,
        enemyTurn
    }

    TurnState turnState = TurnState.playerTurn;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartGame();
    }

    void OnEnable()
    {
        // Always unsubscribe before subscribing to prevent double-registration
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            GameObject buttonObj = GameObject.Find("Attack Button");
            if (buttonObj != null)
            {
                attackButton = GameObject.FindWithTag("AttackButton")?.GetComponent<Button>();
                if (attackButton != null)
                {
                    // reassign the listener for do turn
                    attackButton.onClick.RemoveAllListeners();
                    attackButton.onClick.AddListener(DoTurn);
                }
            }
            else
            {
                Debug.LogWarning("Attack Button not found!");
            }

            GameObject enemyObj = GameObject.Find("Enemy");
            if (enemyObj != null)
            {
                currentEnemy = GameObject.FindWithTag("Enemy")?.GetComponent<Enemy>();
            }
            else
            {
                Debug.LogWarning("Enemy not found!");
            }
        }
    }

    void StartGame()
    {
        round = 1;

        DeckManager.instance.ShuffleDeck();

        for (int i = DeckManager.instance.hand.Count; i < 6; i++)
        {
            DeckManager.instance.DrawPiece();
        }
    }

    public void DoTurn()
    {
        // prevents this code from from running if we're already in the middle of a scene transition
        if (!attackButton.interactable || currentEnemy.health <= 0)
        {
            return;
        }

        attackButton.interactable = false;

        List<PieceScriptable> currentPieces = BoardManager.instance.GetBoardPieces();

        int comboIndex = CombatManager.Instance.FindCombo(currentPieces);
        ComboScriptable combo = comboIndex >= 0 ? Spellbook.instance.combosUnlocked[comboIndex] : null;

        if (combo != null)
        {
            currentEnemy.TakeDamage(CombatManager.Instance.CalculateDamage(combo, currentPieces));
            CombatManager.Instance.CalculateGold(combo, currentPieces);
            CombatManager.Instance.CalculateHealth(combo, currentPieces);
        }
        else
        {
            currentEnemy.TakeDamage(0);
        }

        EndTurn();
    }

    void EndTurn()
    {
        Debug.Log("EndTurn Called by: " + turnState);

        if (currentEnemy.health <= 0)
        {
            round++;
            Debug.Log("Round: " + round);

            // after 2 rounds show demo screen (game starts on round 1)
            if (round > 2)
            {
                SceneManager.LoadScene(5);
            }
            else
            {
                TransitionManager.instance.ActivateTransition("ShopTransition");
            }

            return;
        }
        else if (Player.instance.GetHealth() <= 0)
        {
            //lose
            SceneManager.LoadScene(2);
            return;
        }

        //The end of the Players turn
        if (turnState == TurnState.playerTurn)
        {
            DeckManager.instance.DiscardBoard();
            DeckManager.instance.DrawPiecesTillMax();
            //switdh to enemy's turn
            turnState = TurnState.enemyTurn;
            Invoke("DoEnemyTurn", 1);
        }
        else if (turnState == TurnState.enemyTurn)
        {
            //switch to enemy's turn
            attackButton.interactable = true;
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