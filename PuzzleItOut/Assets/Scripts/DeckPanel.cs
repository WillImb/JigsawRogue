using System.Collections.Generic;
using UnityEngine;

public class DeckPanel : MonoBehaviour
{
    private DeckManager deckManager;
    private List<GameObject> deck;

    [SerializeField] private Transform contentParent;   // ui container to set pieces off of
    [SerializeField] private GameObject pieceUIPrefab;  // ui version of piece (need to replace)

    void Start()
    {
        deckManager = DeckManager.instance;
        deck = deckManager.deck;

        PopulateDeckPanel();
    }

    /// <summary>
    /// Adds visual representations for each piece in players deck into deck panel
    /// </summary>
    public void PopulateDeckPanel()
    {
        // clear old UI if any (this shouldnt be necessary but just in case)
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // for each piece in deck create a visual variant
        foreach (GameObject piecePrefab in deck)
        {
            GameObject uiPiece = Instantiate(pieceUIPrefab, contentParent);

            // reset transformations
            RectTransform rect = uiPiece.GetComponent<RectTransform>();
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;

            // copy data of original piece
            Piece original = piecePrefab.GetComponent<Piece>();
            Piece ui = uiPiece.GetComponent<Piece>();

            if (original != null && ui != null)
            {
                ui.CopyFrom(original);
            }
        }
    }
}