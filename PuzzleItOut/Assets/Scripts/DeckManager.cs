
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager instance;   
    

    public List<PieceScriptable> deck;
    public List<PieceScriptable> hand;
    public List<PieceScriptable> discard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //moves piece from deck to hand
    public void DrawPiece()
    {
        int randomIndex = Random.Range(0, deck.Count);
        hand.Add(deck[randomIndex]);
        deck.RemoveAt(randomIndex);
    }

    //discards a specified piece form hand to discard
    public void DiscardPiece(int index)
    {        
        discard.Add(deck[index]);
        hand.RemoveAt(index);
    }

    //moves all pieces from discard to deck
    public void ShuffleDeck()
    {
        deck.AddRange(discard);
        discard.Clear();
    }
}
