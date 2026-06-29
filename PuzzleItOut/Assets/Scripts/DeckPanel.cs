using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * Author(s): Anthony L
 * Date: 6.18.26
 * Notes:
 * - Displays all pieces currently in the players deck
 * - Allows sorting/filtering deck pieces from the Unity Inspector. 
 *   Interacting with sorting will be implemented after this
 */
public class DeckPanel : MonoBehaviour
{
    private DeckManager deckManager;
    private List<GameObject> physicalDeck;

    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject pieceUIPrefab;

    public enum SortMode
    {
        OriginalOrder,
        Combat,
        Healing,
        Gold
    }

    public enum SortOrder
    {
        Ascending,
        Descending
    }

    public enum FilterMode
    {
        AllPieces,
        Fire,
        Water,
        Earth,
        Air
    }

    [Header("Sorting / Filtering")]
    [SerializeField] private SortMode sortMode = SortMode.OriginalOrder;
    [SerializeField] private SortOrder sortOrder = SortOrder.Descending;
    [SerializeField] private FilterMode filterMode = FilterMode.AllPieces;

    [SerializeField] private GameObject sortModeButton;
    [SerializeField] private GameObject sortOrderButton;
    [SerializeField] private GameObject sortFilterButton;

    void OnEnable()
    {
        DeckManager.OnDeckUpdated += PopulateDeckPanel;

        // physicalDeck = deckManager.physicalDeck;

        if (deckManager != null)
        {
            physicalDeck = deckManager.physicalDeck;

            UpdateModeButtonText();
            UpdateOrderButtonText();
            UpdateFilterButtonText();

            PopulateDeckPanel();
        }
    }

    void OnDisable()
    {
        DeckManager.OnDeckUpdated -= PopulateDeckPanel;
    }

    /// <summary>
    /// Gets piece list from DeckManager and converts it into UI elements for DeckPanel
    /// </summary>
    public void PopulateDeckPanel()
    {
        // checks
        if (deckManager == null)
        {
            deckManager = DeckManager.instance;
        }

        if (deckManager == null || deckManager.physicalDeck == null)
        {
            Debug.LogWarning("DeckManager or physical deck is missing");
            return;
        }

        if (contentParent == null || pieceUIPrefab == null)
        {
            Debug.LogWarning("Content Parent or Piece UI Prefab is missing");
            return;
        }

        physicalDeck = deckManager.physicalDeck;

        // reset deck panel
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        List<GameObject> arrangedDeck = GetArrangedDeck();

        // create deck panel pieces
        for (int i = 0; i < arrangedDeck.Count; i++)
        {
            GameObject physicalPiece = arrangedDeck[i];

            GameObject uiPiece = Instantiate(pieceUIPrefab, contentParent);

            DeckPiece deckPieceUI = uiPiece.GetComponent<DeckPiece>();

            if (deckPieceUI != null)
            {
                deckPieceUI.index = physicalDeck.IndexOf(physicalPiece);
            }

            Image image = uiPiece.GetComponentInChildren<Image>();

            Piece piece = physicalPiece.GetComponent<Piece>();

            if (piece == null || piece.pieceData == null)
            {
                Debug.LogWarning($"Missing Piece or PieceData on {physicalPiece.name}");
                continue;
            }

            var data = piece.pieceData;

            cardType type = data.cardType;

            // set piece color
            if (image != null)
            {
                image.color = GetElementColor(data.cardType);
            }

            Transform textChild = uiPiece.transform.Find("Text");

            if (textChild != null)
            {
                TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();

                if (tmp != null)
                {
                    tmp.text =
                        $"{data.pieceName}\n\n" +
                        $"Combat - {data.combatValue}\n" +
                        $"Healing - {data.healingValue}\n" +
                        $"Gold - {data.goldValue}";
                }
            }
            else
            {
                Debug.LogWarning("No 'Text' child found on UI prefab");
            }
        }
    }

    /// <summary>
    /// Gets piece list from DeckManager and converts it into UI elements for DeckPanel based on sorting/filtering parameters
    /// </summary>
    private List<GameObject> GetArrangedDeck()
    {
        IEnumerable<GameObject> arrangedDeck = physicalDeck;

        switch (filterMode)
        {
            case FilterMode.Fire:
                arrangedDeck = arrangedDeck.Where(piece =>
                    piece.GetComponent<Piece>()?.pieceData?.cardType == cardType.fire);
                break;

            case FilterMode.Water:
                arrangedDeck = arrangedDeck.Where(piece =>
                    piece.GetComponent<Piece>()?.pieceData?.cardType == cardType.water);
                break;

            case FilterMode.Earth:
                arrangedDeck = arrangedDeck.Where(piece =>
                    piece.GetComponent<Piece>()?.pieceData?.cardType == cardType.earth);
                break;

            case FilterMode.Air:
                arrangedDeck = arrangedDeck.Where(piece =>
                    piece.GetComponent<Piece>()?.pieceData?.cardType == cardType.air);
                break;

            case FilterMode.AllPieces:
            default:
                break;
        }

        // apply sorting based on chosen mode and order direction
        bool isDescending = sortOrder == SortOrder.Descending;

        switch (sortMode)
        {
            case SortMode.Combat:
                arrangedDeck = isDescending ? arrangedDeck.OrderByDescending(GetCombatValue) : arrangedDeck.OrderBy(GetCombatValue);
                break;

            case SortMode.Healing:
                arrangedDeck = isDescending ? arrangedDeck.OrderByDescending(GetHealingValue) : arrangedDeck.OrderBy(GetHealingValue);
                break;

            case SortMode.Gold:
                arrangedDeck = isDescending ? arrangedDeck.OrderByDescending(GetGoldValue) : arrangedDeck.OrderBy(GetGoldValue);
                break;

            case SortMode.OriginalOrder:
            default:
                // default to descending order
                if (isDescending)
                {
                    arrangedDeck = arrangedDeck.Reverse();
                }
                break;
        }

        return arrangedDeck.ToList();
    }

    /// <summary>
    /// Cycles between different sort modes. Meant to be attached to a button. 
    /// </summary>
    public void SetModeButton()
    {
        // cycles through sort mode enums using modulo
        sortMode = (SortMode)(((int)sortMode + 1) % System.Enum.GetValues(typeof(SortMode)).Length);

        Debug.Log($"Sort Mode: {sortMode}");

        UpdateModeButtonText();

        PopulateDeckPanel();
    }

    /// <summary>
    /// Cycles between ascending and descending order. Meant to be attached to a button. 
    /// </summary>
    public void SetOrderButton()
    {
        // cycles through sort order enums using modulo
        sortOrder = (SortOrder)(((int)sortOrder + 1) % System.Enum.GetValues(typeof(SortOrder)).Length);

        Debug.Log($"Sort Order: {sortOrder}");

        UpdateOrderButtonText();

        PopulateDeckPanel();
    }

    /// <summary>
    /// Cycles between filter modes. Meant to be attached to a button. 
    /// </summary>
    public void SetFilterMode()
    {
        // cycles through filter mode enums using modulo
        filterMode = (FilterMode)(((int)filterMode + 1) % System.Enum.GetValues(typeof(FilterMode)).Length);

        Debug.Log($"Filter Mode: {filterMode}");

        UpdateFilterButtonText();

        PopulateDeckPanel();
    }

    /// <summary>
    /// Updates the text of the sort mode button.
    /// </summary>
    private void UpdateModeButtonText()
    {
        TMP_Text modeButtonText = sortModeButton.GetComponentInChildren<TMP_Text>();

        if (modeButtonText != null)
        {
            modeButtonText.text = sortMode switch
            {
                SortMode.OriginalOrder => "O",
                SortMode.Combat => "C",
                SortMode.Healing => "H",
                SortMode.Gold => "G",
                _ => "?" // default
            };
        }
    }

    /// <summary>
    /// Updates the text of the sort order button.
    /// </summary>
    private void UpdateOrderButtonText()
    {
        TMP_Text orderButtonText = sortOrderButton.GetComponentInChildren<TMP_Text>();

        if (orderButtonText != null)
        {
            orderButtonText.text = sortOrder switch
            {
                SortOrder.Ascending => "A",
                SortOrder.Descending => "D",
                _ => "?" // default
            };
        }
    }

    /// <summary>
    /// Updates the text of the filter button.
    /// </summary>
    private void UpdateFilterButtonText()
    {
        TMP_Text filterButtonText = sortFilterButton.GetComponentInChildren<TMP_Text>();

        if (filterButtonText != null)
        {
            filterButtonText.text = filterMode switch
            {
                FilterMode.AllPieces => "All",
                FilterMode.Fire => "F",
                FilterMode.Water => "W",
                FilterMode.Earth => "E",
                FilterMode.Air => "A",
                _ => "?" // default
            };
        }
    }

    /// <summary>
    /// Updates text for all deck panel buttons
    /// </summary>
    public void UpdateAllButtonText()
    {
        UpdateModeButtonText();
        UpdateOrderButtonText();
        UpdateFilterButtonText();
    }

    private Color GetElementColor(cardType type)
    {
        return type switch
        {
            cardType.fire => HexToColor("#FF7B69"),
            cardType.water => HexToColor("#0BF9FF"),
            cardType.earth => HexToColor("#6DFF8B"),
            cardType.air => HexToColor("#FFFFFF"),
            _ => Color.white
        };
    }

    private static Color HexToColor(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }

    private int GetCombatValue(GameObject pieceObject)
    {
        Piece piece = pieceObject.GetComponent<Piece>();
        return piece != null && piece.pieceData != null ? piece.pieceData.combatValue : 0;
    }

    private int GetHealingValue(GameObject pieceObject)
    {
        Piece piece = pieceObject.GetComponent<Piece>();
        return piece != null && piece.pieceData != null ? piece.pieceData.healingValue : 0;
    }

    private int GetGoldValue(GameObject pieceObject)
    {
        Piece piece = pieceObject.GetComponent<Piece>();
        return piece != null && piece.pieceData != null ? piece.pieceData.goldValue : 0;
    }

    private string GetPieceName(GameObject pieceObject)
    {
        Piece piece = pieceObject.GetComponent<Piece>();
        return piece != null && piece.pieceData != null ? piece.pieceData.pieceName : "";
    }
}