
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Author(s): Anthony L
 * Date: 5.15.26
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
    [SerializeField]
    private Piece[] occupied;

    public Transform deckSpawn;
    public Transform discardSpawn;


    

    // piece halo needs to be reassigned when coming back into game scene
    // that or piece halo can be turned into a prefab and assigned on start
    public GameObject pieceHalo;

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

    private void Start()
    {
       
    }

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

    //shuffles deck using Fisher-Yates algo
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
    //draws piece from top of the deck to hand 
    public void DrawPiece()
    {
        if (physicalDeck.Count != 0)
        {

            //GameObject prefab = deck[deck.Count - 1];
            //GameObject spawnedPiece = Instantiate(prefab);
            int randomIndex = UnityEngine.Random.Range(0, physicalDeck.Count);

            hand.Add(physicalDeck[randomIndex]);
            physicalDeck.RemoveAt(randomIndex);

            // invoke UI refresh
            OnDeckUpdated?.Invoke();

            ReturnToHand(hand[hand.Count-1].GetComponent<Piece>());
        }
    }

    public void DrawPiecesTillMax()
    {
        for (int i = 0; i < occupied.Length; i++)
        {
            if (occupied[i] == null && physicalDeck.Count > 0)
            {
             
                DrawPiece();
            }
        }
        if(hand.Count <= 5)
        {
            DiscardToDeck();
            DrawPiecesTillMax();
        }

    }

    //discards a specified piece form hand to discard
    public void DiscardPieceFromHand(int index)
    {
        GameObject piece = hand[index];
        discard.Add(piece);
        hand.RemoveAt(index);
        
        RemoveFromHand(piece.GetComponent<Piece>());
        piece.transform.position = discardSpawn.position;
        //Destroy(piece);
    }

    //removes a piece to hand
    public void RemoveFromHand(Piece piece)
    {
        hand.Remove(piece.gameObject);
        //int index = System.Array.IndexOf(occupied, piece);
        //if (index == -1) return;
        //occupied[index] = null;
    }

    //Discards all pieces on board
    public List<Piece> DiscardBoard()
    {
        BoardManager bm = BoardManager.instance;
        List<Piece> piecesPlayed = new List<Piece>();
        for (int i = 0; i < bm.occupied.Length; i++) {
            if (bm.occupied[i] != null)
            {
                piecesPlayed.Add(bm.occupied[i]);
                discard.Add(bm.occupied[i].gameObject);
                GameObject piece = bm.slots[i].GetChild(0).gameObject;

                VFXManager.instance.SpawnParticle(piece.transform.position, 5);
                //Destroy(piece);
                RemoveFromHand(piece.GetComponent<Piece>());
                piece.transform.parent = null;
                piece.transform.rotation = Quaternion.identity;
                piece.transform.position = discardSpawn.position;


                for(int j = 0; j < occupied.Length; j++)
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

    //moves all pieces from discard to deck
    public void DiscardToDeck()
    {
        physicalDeck.AddRange(discard);
        foreach(GameObject g in physicalDeck)
        {
            g.transform.position = deckSpawn.position;
        }
        discard.Clear();
    }

    //returns a piece to hand
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

    // adds a piece to the deck
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
