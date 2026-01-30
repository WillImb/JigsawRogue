
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

    // ! Warning: Change these to piece objects once completed !

    public void DrawPiece()
    {
        int deckIndex = Random.Range(0,deck.Count);
        hand.Add(deck[deckIndex]);
    }
    public void DiscardPiece(PieceScriptable piece)
    {
        discard.Add(piece);
        hand.Remove(piece);
    }

    //Need more context first...
    public void GetPiece()
    {
        
    }
  
}
