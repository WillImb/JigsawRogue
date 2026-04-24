using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*
 * Class: ShopManager
 * Date: 4.1.25
 * Notes: 
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
    public GameObject spellBook;   // reference to the players spellbook (singleton)
    public GameObject deckPanel;   // reference to deck panel 
    private DeckManager deckManager;
    public GameObject upgradedPanel;

    // is the deck panel open because of upgrading
    [SerializeField] bool isUpgrading;

    private GameObject currentUpgradeButton;

    void Start()
    {
        // find the spellbook object in the scene (to add bought combos)
        spellBook = GameObject.FindWithTag("SpellBook");

        // grab deck manager instance and current deck
        deckManager = DeckManager.instance;

        // assign a random piece/combo from the pools to Shop UI game objects
        AssignShopItems();
    }

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

    /*
     * Assigns random piece/combo from the pools to each shop button. Also
     * prevents duplicates from appearing in the shop.
     */
    void AssignShopItems()
    {
        AssignPieces();
        AssignCombos();
    }

    // piece assignment
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

    // combo assignment
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

    // if all combos are sold out
    void SetAllCombosSoldOut()
    {
        foreach (GameObject slot in combos)
        {
            SetComboSoldOut(slot);
        }
    }

    // returns a list of locked combos
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

    // returns a list of available combos
    List<ComboScriptable> GetAvailableCombos(List<ComboScriptable> locked, List<ComboScriptable> assigned)
    {
        return locked.FindAll(c => !assigned.Contains(c));
    }

    // gets a random combo from a given list of combos
    ComboScriptable GetRandomCombo(List<ComboScriptable> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    // assigns a combo to a slot in the shop
    void AssignComboToSlot(GameObject slot, ShopData data, ComboScriptable combo)
    {
        data.combo = combo;

        TMP_Text text = slot.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = combo.name;
        }
    }

    // 
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

    // purchase methods

    /*
     * Called when a piece button is clicked. 
     */
    public void BuyPiece(GameObject obj)
    {
        ShopData data = obj.GetComponent<ShopData>();

        // only continue if the button has ShopData and a valid piece assigned
        if (data != null && data.piecePrefab != null)
        {
            // add the piece to the players deck
            DeckManager.instance.AddPiece(data.piecePrefab);

            // hide button when piece is bought
            obj.SetActive(false);
        }
    }

    /*
     * Called when a combo button is clicked. 
     */
    public void BuyCombo(GameObject obj)
    {
        ShopData data = obj.GetComponent<ShopData>();

        // only continue if the button has ShopData and a valid combo assigned
        if (data != null && data.combo != null)
        {
            // unlock the combo in players Spellbook
            Spellbook.instance.UnlockCombo(data.combo);

            // hide button when combo is bought
            obj.SetActive(false);
        }
    }

    // state/ui control

    /*
     * 
     */
    public void SetUpgrading(bool upgrading)
    {
        isUpgrading = upgrading;
    }

    public void SetDeckPanelActive(bool active)
    {
        deckPanel.SetActive(active);

        if (active && isUpgrading)
        {
            foreach (GameObject piece in deckManager.deck)
            {
                if (piece != null)
                {

                }
            }
        }
    }

    public void OpenUpgradePanel(GameObject upgradeButton)
    {
        isUpgrading = true;
        currentUpgradeButton = upgradeButton;

        SetDeckPanelActive(true);
    }

    public void DisableCurrentUpgradeButton()
    {
        if (currentUpgradeButton != null)
        {
            currentUpgradeButton.SetActive(false);
            currentUpgradeButton = null;
        }
    }

    public void SetUpgradedPanelActive(bool active)
    {
        upgradedPanel.SetActive(active);
        // also set text of upgradedPanel to reflect the type of piece that got upgraded
    }
}