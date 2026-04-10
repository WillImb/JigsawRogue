using System.Collections.Generic;
using UnityEngine;

public class DeckPanel : MonoBehaviour
{
    private DeckManager deckManager;
    private List<GameObject> deck;

    void Start()
    {
        // assign deck manager
        deckManager = DeckManager.instance;
        deck = deckManager.deck;
    }

    void populateDeckPanel()
    {
        // pseudo-code
        // for each piece in deck
        // make visible
        setDeckVisible(true);
    }

    void setDeckVisible(bool isVisible)
    {
        foreach (GameObject piece in deck)
        {
            if (piece != null)
                piece.SetActive(isVisible);
        }
    }
}
