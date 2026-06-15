
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Author(s): Anthony L
 * Date: 6.15.26
 * Notes:
 *  - Currently, there's a bug where your pieces from the discard pile aren't returned to you
 */
public class DeckManager : MonoBehaviour
{
    public static DeckManager instance;
    public List<GameObject> deck;
    public List<GameObject> physicalDeck;
    public List<GameObject> hand;
    public List<GameObject> discard;
    [SerializeField] private Transform[] handSlots;
    [SerializeField] private Piece[] occupied;

    public Transform deckSpawn;
    public Transform discardSpawn;

    public GameObject pieceHalo;

    // prefabs for each of the pieces
    // sprite order: f, w, e, a
    [SerializeField] private List<GameObject> piecePrefabs;

    // event for updating deck
    public static event Action OnDeckUpdated;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        occupied = new Piece[handSlots.Length];
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Will be used for instantiating new pieces
    /// </summary>
    private void Start()
    {
        deck.Clear();

        // create x instances of each piece depending on the max capacity of the deck

    }

    /// <summary>
    /// Spawns deck pieces into the scene if they have not already been created
    /// </summary>
    public void SpawnPieces()
    {
        if (physicalDeck.Count + hand.Count < deck.Count)
        {
            int difference = deck.Count - physicalDeck.Count + hand.Count;
            foreach (GameObject g in deck)
            {
                physicalDeck.Add(Instantiate(g, deckSpawn.position, Quaternion.identity));
            }
        }
    }

    /// <summary>
    /// Handles scene transitions and updates deck visibility and references
    /// </summary>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Shop" || scene.name == "LoseScene" || scene.name == "Main Menu")
        {
            SetPiecesVisible(false);
        }
        else if (scene.name == "GameScene")
        {
            // reassign piece halo
            pieceHalo = GameObject.Find("PieceHalo");

            SetPiecesVisible(true);

            ReassignPieceHalo(scene);
        }
    }

    /// <summary>
    /// Randomizes the order of pieces in the physical deck
    /// </summary>
    public void ShuffleDeck()
    {
        for (int i = physicalDeck.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            GameObject temp = physicalDeck[i];
            physicalDeck[i] = physicalDeck[randomIndex];
            physicalDeck[randomIndex] = temp;
        }
    }

    /// <summary>
    /// Draws a random piece from the deck and places it into the hand
    /// </summary>
    public void DrawPiece()
    {
        if (physicalDeck.Count != 0)
        {
            //GameObject prefab = deck[deck.Count - 1];
            //GameObject spawnedPiece = Instantiate(prefab);

            //get random piece
            int randomIndex = UnityEngine.Random.Range(0, physicalDeck.Count);

            hand.Add(physicalDeck[randomIndex]);

            physicalDeck.RemoveAt(randomIndex);

            // invoke UI refresh
            OnDeckUpdated?.Invoke();

            ReturnToHand(hand[hand.Count - 1].GetComponent<Piece>());
        }
    }

    /// <summary>
    /// Fills all available hand slots with pieces from the deck.
    /// </summary>
    public void DrawPiecesTillMax()
    {
        for (int i = 0; i < occupied.Length; i++)
        {
            if (occupied[i] == null && physicalDeck.Count > 0)
            {

                DrawPiece();
            }
        }
        if (hand.Count <= 5)
        {
            DiscardToDeck();
            DrawPiecesTillMax();
        }
    }

    /// <summary>
    /// Moves a hand piece into the discard pile
    /// </summary>
    public void DiscardPieceFromHand(int index)
    {
        GameObject piece = hand[index];
        discard.Add(piece);
        hand.RemoveAt(index);

        RemoveFromHand(piece.GetComponent<Piece>());
        piece.transform.position = discardSpawn.position;
        //Destroy(piece);
    }

    /// <summary>
    /// Removes a piece from the hand collection
    /// </summary>
    public void RemoveFromHand(Piece piece)
    {
        hand.Remove(piece.gameObject);
        //int index = System.Array.IndexOf(occupied, piece);
        //if (index == -1) return;
        //occupied[index] = null;
    }

    /// <summary>
    /// Discards all pieces currently placed on the board
    /// </summary>
    public List<Piece> DiscardBoard()
    {
        BoardManager bm = BoardManager.instance;
        List<Piece> piecesPlayed = new List<Piece>();
        for (int i = 0; i < bm.occupied.Length; i++)
        {
            if (bm.occupied[i] != null)
            {
                piecesPlayed.Add(bm.occupied[i]);
                discard.Add(bm.occupied[i].gameObject);
                GameObject piece = bm.slots[i].GetChild(0).gameObject;

                VFXManager.instance.SpawnParticle(piece.transform.position, 5);
                //Destroy(piece);
                RemoveFromHand(piece.GetComponent<Piece>());
                piece.transform.parent = null;
                // piece.transform.rotation = Quaternion.identity;
                piece.transform.position = discardSpawn.position;

                for (int j = 0; j < occupied.Length; j++)
                {
                    if (occupied[j] != null && occupied[j].gameObject == piece)
                    {
                        occupied[j] = null;
                        break;
                    }
                }

                bm.occupied[i] = null;
            }
        }

        return piecesPlayed;
    }

    /// <summary>
    /// Returns all discarded pieces back into the deck
    /// </summary>
    public void DiscardToDeck()
    {
        physicalDeck.AddRange(discard);
        foreach (GameObject g in physicalDeck)
        {
            g.transform.position = deckSpawn.position;
        }
        discard.Clear();
    }

    /// <summary>
    /// Places a piece into an available hand slot
    /// </summary>
    public void ReturnToHand(Piece piece)
    {
        int index = System.Array.IndexOf(occupied, piece);
        if (index == -1)
        {
            index = System.Array.IndexOf(occupied, null);
            if (index == -1) return;
            occupied[index] = piece;
        }
        piece.LockToSlot(handSlots[index]);
    }

    /// <summary>
    /// Adds a new piece to the deck collection.
    /// </summary>
    public void AddPiece(GameObject piece)
    {
        if (piece == null)
        {
            Debug.LogWarning("Can't add null piece to deck");
            return;
        }

        deck.Add(piece);

        // trigger UI refresh 
        OnDeckUpdated?.Invoke();

        Debug.Log($"Added {piece.name} to deck. Deck now has {deck.Count} pieces.");
    }

    /// <summary>
    /// Toggles visibility of all pieces
    /// </summary>
    void SetPiecesVisible(bool visible)
    {
        foreach (GameObject piece in hand)
        {
            if (piece != null)
                piece.SetActive(visible);
        }

        foreach (GameObject piece in discard)
        {
            if (piece != null)
                piece.SetActive(visible);
        }
    }

    /// <summary>
    /// Applies an upgrade to a deck piece if it can be afforded
    /// </summary>
    public void UpgradePiece(int index)
    {
        // upgrade cost
        int upgradeCost = 1;

        // make sure player can afford upgrade
        if (!GoldManager.Instance.CanAfford(upgradeCost))
        {
            Debug.Log("Not enough gold to upgrade");
            return;
        }

        if (index < 0 || index >= deck.Count)
        {
            Debug.LogWarning("Invalid deck index");
            return;
        }

        GameObject pieceObj = deck[index];
        Piece piece = pieceObj.GetComponent<Piece>();

        if (piece == null || piece.pieceData == null)
        {
            Debug.LogWarning("Missing Piece or PieceData");
            return;
        }

        // spend gold
        GoldManager.Instance.SpendGold(upgradeCost);

        // temp upgrade
        piece.pieceData.combatValue += 1;
        piece.pieceData.healingValue += 1;
        piece.pieceData.goldValue += 1;

        Debug.Log($"Upgraded {piece.pieceData.pieceName}" + " #" + index);
    }

    /// <summary>
    /// Finds PieceHalo when re-entering GameScene and reassigns it to all pieces
    /// </summary>
    void ReassignPieceHalo(Scene scene)
    {
        if (scene.name == "GameScene")
        {
            SetPiecesVisible(true);

            pieceHalo = GameObject.Find("PieceHalo");

            foreach (GameObject pieceObj in hand)
            {
                if (pieceObj != null)
                {
                    Piece piece = pieceObj.GetComponent<Piece>();

                    if (piece != null)
                    {
                        piece.pieceHalo = pieceHalo;
                    }
                }
            }
        }
    }
}
