
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckManager : MonoBehaviour
{
    public static DeckManager instance;
    public List<GameObject> deck;
    public List<GameObject> hand;
    public List<GameObject> discard;
    [SerializeField]
    private Transform[] handSlots;
    private Piece[] occupied;

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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Shop")
        {
            SetPiecesVisible(false);
        }
        else if (scene.name == "GameScene")
        {
            SetPiecesVisible(true);
        }
    }

    //shuffles deck using Fisher-Yates algo
    public void ShuffleDeck()
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            GameObject temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }
    //draws piece from top of the deck to hand 
    public void DrawPiece()
    {
        if (deck.Count != 0)
        {
            GameObject prefab = deck[deck.Count - 1];
            GameObject spawnedPiece = Instantiate(prefab);
            hand.Add(spawnedPiece);
            deck.RemoveAt(deck.Count - 1);

            ReturnToHand(spawnedPiece.GetComponent<Piece>());
        }
    }

    public void DrawPiecesTillMax()
    {
        for (int i = 0; i < occupied.Length; i++)
        {
            if (occupied[i] == null && deck.Count > 0)
            {
                DrawPiece();
            }
        }
    }

    //discards a specified piece form hand to discard
    public void DiscardPieceFromHand(int index)
    {
        GameObject piece = hand[index];
        discard.Add(piece);
        hand.RemoveAt(index);
        
        RemoveFromHand(piece.GetComponent<Piece>());
        Destroy(piece);
    }

    //removes a piece to hand
    public void RemoveFromHand(Piece piece)
    {
        int index = System.Array.IndexOf(occupied, piece);
        if (index == -1) return;
        occupied[index] = null;
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
                Destroy(piece);
                bm.occupied[i] = null;
            }
        }
        return piecesPlayed;
    }

    //moves all pieces from discard to deck
    public void DiscardToDeck()
    {
        deck.AddRange(discard);
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
    public void AddPiece(GameObject piecePrefab)
    {
        if (piecePrefab == null)
        {
            Debug.LogWarning("Can't add null piece to deck");
            return;
        }

        deck.Add(piecePrefab);
        Debug.Log($"Added {piecePrefab.name} to deck. Deck now has {deck.Count} pieces.");
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

        // temp upgrade
        piece.pieceData.combatValue += 1;
        piece.pieceData.healingValue += 1;
        piece.pieceData.goldValue += 1;

        Debug.Log($"Upgraded {piece.pieceData.pieceName}" + " #" + index);
    }
}
