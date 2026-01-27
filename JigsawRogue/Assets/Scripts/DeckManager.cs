
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


    public void DrawPiece()
    {

    }
    public void DiscardPiece()
    {

    }
    public void ShuffleDeck()
    {

    }
}
