using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * Class: DeckPanel
 * Date: 4.17.26
 * Notes:
 *  - Displays all pieces currently in the players deck
 *  - 
 */
public class DeckPanel : MonoBehaviour
{
    // reference to deck manager singleton
    private DeckManager deckManager;

    // local copy of deck list
    private List<GameObject> deck;

    // UI container to spawn piece UI elements under
    [SerializeField] private Transform contentParent;

    // prefab used to visually represent a piece in UI
    [SerializeField] private GameObject pieceUIPrefab;

    void Start()
    {
        // grab deck manager instance and current deck
        deckManager = DeckManager.instance;
        deck = deckManager.deck;

        // populate UI with current deck contents
        PopulateDeckPanel();
    }

    /*
     * Instantiates a UI element for each piece in the players deck
     * and assigns its sprite + stats text
     */
    public void PopulateDeckPanel()
    {
        foreach (GameObject piecePrefab in deck)
        {
            // create UI object under the content parent
            GameObject uiPiece = Instantiate(pieceUIPrefab, contentParent);

            // reset position (avoid weird layout issues)
            RectTransform rect = uiPiece.GetComponent<RectTransform>();
            // rect.localRotation = Quaternion.identity;
            rect.anchoredPosition = Vector2.zero;

            // image setup
            // assign sprite that does not contain the element design
            Image image = uiPiece.GetComponent<Image>();
            Piece piece = piecePrefab.GetComponent<Piece>();

            if (piece != null && image != null)
            {
                image.sprite = piece.baseSprite;
            }
            else
            {
                Debug.LogWarning($"Missing Piece or baseSprite on {piecePrefab.name}");
            }

            // text setup
            // skip if piece or its data is missing
            if (piece == null || piece.pieceData == null)
            {
                Debug.LogWarning($"Missing Piece or PieceData on {piecePrefab.name}");
                continue;
            }

            // find text child object
            Transform textChild = uiPiece.transform.Find("Text");

            if (textChild != null)
            {
                TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();

                if (tmp != null)
                {
                    var data = piece.pieceData;

                    // assign formatted stats text
                    tmp.text =
                        $"{data.pieceName}\n\n" +
                        $"Combat - {data.combatValue}\n" +
                        $"Healing - {data.healingValue}\n" +
                        $"Gold - {data.goldValue}";
                }
            }
            else
            {
                Debug.LogWarning($"No 'Text' child found on UI prefab");
            }
        }
    }
}