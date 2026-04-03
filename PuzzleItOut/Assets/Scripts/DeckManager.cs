using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckManager : MonoBehaviour
{
    public static DeckManager instance;
    public List<GameObject> deck;
    public List<Piece> hand;
    public List<Piece> discard;
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
        discard = new List<Piece>();
        hand = new List<Piece>();
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
        if (scene.name == "EndOfDemoScene")
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
                instance = null;
            }
        }
        else if (scene.name == "Shop")
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
            Piece piece = spawnedPiece.GetComponent<Piece>();

            hand.Add(piece);
            deck.RemoveAt(deck.Count - 1);

            ReturnToHand(piece);
        }
    }

    public void DrawPiecesTillMax()
    {
        for (int i = 0; i < occupied.Length; i++)
        {
            if (occupied[i] == null)
            {
                if (deck.Count > 0)
                {
                    DrawPiece();
                }
                else if (discard.Count > 0)
                {
                    Piece piece = discard[discard.Count - 1];
                    discard.RemoveAt(discard.Count - 1);
                    piece.gameObject.SetActive(true);
                    ReturnToHand(piece);
                }
            }
        }
    }

    //discards a specified piece form hand to discard
    public void DiscardPieceFromHand(int index)
    {
        Piece piece = hand[index];
        hand.RemoveAt(index);

        RemoveFromHand(piece);
        piece.gameObject.SetActive(false);
        discard.Add(piece);
    }

    //Discards all pieces on board
    public List<Piece> DiscardBoard()
    {
        BoardManager bm = BoardManager.instance;
        List<Piece> piecesPlayed = new List<Piece>();
        for (int i = 0; i < bm.occupied.Length; i++)
        {
            if (bm.occupied[i] != null)
            {
                Piece piece = bm.occupied[i];
                piecesPlayed.Add(piece);

                discard.Add(piece);
                bm.occupied[i] = null;
                piece.gameObject.SetActive(false);
            }
        }
        return piecesPlayed;
    }

    //removes a piece to hand
    public void RemoveFromHand(Piece piece)
    {
        int index = System.Array.IndexOf(occupied, piece);
        if (index == -1) return;
        occupied[index] = null;
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
        foreach (Piece piece in hand)
        {
            if (piece != null)
                piece.gameObject.SetActive(visible);
        }

        foreach (Piece piece in discard)
        {
            if (piece != null)
                piece.gameObject.SetActive(visible);
        }
    }
}