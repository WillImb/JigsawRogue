using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*
 * Author(s): Anthony L
 * Date: 5.26.26
 * Notes:
 *  - SetAllCombosSoldOut() is currently deprecated since upgrade buttons just disappear
 *    when they're bought
 *  - I plan on separating upgrade panel and deck panel into two different things. This
 *    will be added as a task or subtask on the Trello - AL
 *  - 
 */
public class ShopManager : MonoBehaviour
{
    // instance for this class
    public static ShopManager instance;

    // shop ui game objects
    public List<GameObject> pieces;
    public List<GameObject> upgrades;
    public List<GameObject> combos;

    // pools
    public List<GameObject> piecePool;
    public List<ComboScriptable> comboPool;
    public List<Sprite> sprites;
    // public List<UpgradeData> upgradePool;

    // reference variables
    public GameObject deckPanel;   // reference to deck panel 
    public GameObject upgradedPanel;

    // temp upgrade cost var for testing
    [SerializeField] private int upgradeCost = 1;

    // is the deck panel open because of upgrading
    [SerializeField] public bool isUpgrading;

    private GameObject currentUpgradeButton;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple ShopManager instances found");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // assign a random piece/combo from the pools to Shop UI game objects
        AssignShopItems();
    }

    /// <summary>
    /// Assigns random pieces and combos to all shop slots
    /// </summary>
    void AssignShopItems()
    {
        AssignPieces();
        AssignCombos();
    }

    /// <summary>
    /// Assigns random pieces to each piece slot and links their corresponding upgrade buttons
    /// </summary>
    void AssignPieces()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            ShopData pieceData = pieces[i].GetComponent<ShopData>();
            ShopData upgradeData = upgrades[i].GetComponent<ShopData>();

            int index = Random.Range(0, piecePool.Count);

            // assign piece
            AssignPieceToSlot(pieces[i], pieceData, index);

            // link the corresponding upgrade button
            pieceData.linkedUpgradeButton = upgrades[i];
        }
    }

    /// <summary>
    /// Assigns a piece prefab and sprite to a specific shot slot
    /// </summary>
    void AssignPieceToSlot(GameObject slot, ShopData data, int index)
    {
        GameObject prefab = piecePool[index];
        Sprite sprite = sprites[index];

        data.piecePrefab = prefab;

        Image img = slot.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = sprite;
        }
    }

    /// <summary>
    /// Assigns available combos to combo shop slots while preventing duplicates
    /// </summary>
    void AssignCombos()
    {
        List<ComboScriptable> lockedCombos = GetLockedCombos();
        List<ComboScriptable> assignedCombos = new List<ComboScriptable>();

        // if player unlocked all combos
        if (lockedCombos.Count == 0)
        {
            SetAllCombosSoldOut();
            return;
        }

        for (int i = 0; i < combos.Count; i++)
        {
            ShopData data = combos[i].GetComponent<ShopData>();
            if (data == null) continue;

            // refresh available pool each slot
            List<ComboScriptable> availableCombos = GetAvailableCombos(lockedCombos, assignedCombos);

            // if we still have combos left assign one
            if (availableCombos.Count > 0)
            {
                ComboScriptable selected = GetRandomCombo(availableCombos);
                AssignComboToSlot(combos[i], data, selected);
                assignedCombos.Add(selected);
            }
            else
            {
                // only happens if we exhausted all combos in THIS shop roll
                SetComboSoldOut(combos[i]);
            }
        }
    }

    /// <summary>
    /// Marks all combo slots as sold out when no locked combos remain
    /// </summary>
    void SetAllCombosSoldOut()
    {
        foreach (GameObject slot in combos)
        {
            SetComboSoldOut(slot);
        }
    }

    /// <summary>
    /// Returns a list of combos the player has not unlocked yet
    /// </summary>
    List<ComboScriptable> GetLockedCombos()
    {
        List<ComboScriptable> locked = new List<ComboScriptable>();

        foreach (var combo in comboPool)
        {
            if (!Spellbook.instance.combosUnlocked.Contains(combo))
            {
                locked.Add(combo);
            }
        }

        return locked;
    }

    /// <summary>
    /// Returns combos that are locked and not already assigned in the current shop roll
    /// </summary>
    List<ComboScriptable> GetAvailableCombos(List<ComboScriptable> locked, List<ComboScriptable> assigned)
    {
        return locked.FindAll(c => !assigned.Contains(c));
    }

    /// <summary>
    /// Returns a random combo from the given combo list
    /// </summary>
    ComboScriptable GetRandomCombo(List<ComboScriptable> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Assigns combo data and display text to a combo shop slot
    /// </summary>
    void AssignComboToSlot(GameObject slot, ShopData data, ComboScriptable combo)
    {
        data.combo = combo;

        TMP_Text text = slot.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = combo.name;
        }
    }

    /// <summary>
    /// Disables a combo slot and displays sold out text
    /// </summary>
    /// <param name="slot"></param>
    void SetComboSoldOut(GameObject slot)
    {
        TMP_Text text = slot.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = "SOLD OUT";
        }

        Button button = slot.GetComponent<Button>();
        if (button != null)
        {
            button.interactable = false;
        }
    }

    /// <summary>
    /// Buys a piece, adds it to the player's deck, and hides the purchased piece
    /// </summary>
    public void BuyPiece(GameObject piece)
    {
        ShopData data = piece.GetComponent<ShopData>();

        if (data == null || data.piecePrefab == null)
            return;

        // check gold first
        if (!GoldManager.Instance.CanAfford(1))
            return;

        DeckManager.instance.AddPiece(data.piecePrefab);

        GoldManager.Instance.SpendGold(1);

        piece.SetActive(false);
    }

    /// <summary>
    /// Buys a combo, unlocks it in the spellbook, and hides the purchased combo
    /// </summary>
    public void BuyCombo(GameObject combo)
    {
        ShopData data = combo.GetComponent<ShopData>();

        if (data == null || data.combo == null)
            return;

        // check gold first
        if (!GoldManager.Instance.CanAfford(1))
            return;

        Spellbook.instance.UnlockCombo(data.combo);

        GoldManager.Instance.SpendGold(1);

        combo.SetActive(false);
    }

    /// <summary>
    /// Upgrades a piece if the Player can afford it
    /// </summary>
    public bool BuyUpgrade(int deckIndex)
    {
        if (!GoldManager.Instance.CanAfford(upgradeCost))
        {
            Debug.Log("Not enough gold to upgrade");
            return false;
        }

        GoldManager.Instance.SpendGold(upgradeCost);

        DeckManager.instance.UpgradePiece(deckIndex);

        SetUpgradedPanelActive(true);
        DisableCurrentUpgradeButton();

        return true;
    }

    /// <summary>
    /// Sets whether the player is currently upgrading a piece
    /// </summary>
    public void SetUpgrading(bool upgrading)
    {
        isUpgrading = upgrading;
    }

    /// <summary>
    /// Toggles deck panel visibility
    /// </summary>
    /// <param name="active"></param>
    public void SetDeckPanelActive(bool active)
    {
        deckPanel.SetActive(active);

        if (active && isUpgrading)
        {
            foreach (GameObject piece in DeckManager.instance.deck)
            {
                if (piece != null)
                {

                }
            }
        }
    }

    /// <summary>
    /// Opens the upgrade flow and stores the selected upgrade button reference
    /// </summary>
    public void OpenUpgradePanel(GameObject upgradeButton)
    {
        isUpgrading = true;
        currentUpgradeButton = upgradeButton;

        SetDeckPanelActive(true);
    }

    /// <summary>
    /// Disables currently used upgrade button after upgrading
    /// </summary>
    public void DisableCurrentUpgradeButton()
    {
        if (currentUpgradeButton != null)
        {
            currentUpgradeButton.SetActive(false);
            currentUpgradeButton = null;
        }
    }

    /// <summary>
    /// Toggles the upgraded panel visibility and updates upgrade feedback UI
    /// </summary>
    public void SetUpgradedPanelActive(bool active)
    {
        upgradedPanel.SetActive(active);
        // also set text of upgradedPanel to reflect the type of piece that got upgraded
    }
}