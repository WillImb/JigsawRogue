
using System.Collections.Generic;
using UnityEngine;

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
            instance = this;
        else
            Destroy(gameObject);

        occupied = new Piece[handSlots.Length];
    }

    void Start()
    {

    }

    void Update()
    {
        
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
        GameObject prefab = deck[deck.Count-1];
        GameObject spawnedPiece = Instantiate(prefab);
        hand.Add(spawnedPiece);
        deck.RemoveAt(deck.Count-1);

        ReturnToHand(spawnedPiece.GetComponent<Piece>());
    }

    //discards a specified piece form hand to discard
    public void DiscardPiece(int index)
    {
        GameObject piece = hand[index];
        discard.Add(piece);
        hand.RemoveAt(index);
        
        RemoveFromHand(piece.GetComponent<Piece>());
        Destroy(piece);
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

    //removes a piece to hand
    public void RemoveFromHand(Piece piece)
    {
        int index = System.Array.IndexOf(occupied, piece);
        if (index == -1) return;
        occupied[index] = null;
    }
}
