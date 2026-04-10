using System.Collections.Generic;
using UnityEngine;

public class DeckPanel : MonoBehaviour
{
    private DeckManager deckManager;
    private List<GameObject> deck;

    [SerializeField] private Transform contentParent;   // UI container (Grid / Vertical Layout)
    [SerializeField] private GameObject pieceUIPrefab;  // UI version of piece

    void Start()
    {
        deckManager = DeckManager.instance;
        deck = deckManager.deck;

        PopulateDeckPanel();
    }

    public void PopulateDeckPanel()
    {
        // Clear old UI
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Create one UI piece per deck entry
        foreach (GameObject piecePrefab in deck)
        {
            GameObject uiPiece = Instantiate(pieceUIPrefab, contentParent);

            // Reset transform for UI layout
            RectTransform rect = uiPiece.GetComponent<RectTransform>();
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;

            // Copy data
            Piece original = piecePrefab.GetComponent<Piece>();
            Piece ui = uiPiece.GetComponent<Piece>();

            if (original != null && ui != null)
            {
                ui.CopyFrom(original);
            }
        }
    }
}