using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*
 * Class: ShopManager
 * Date: 4.1.25
 * Notes: 
 *  - Does not take into account gold
 *  - For some reason pieces in the deck get put in the hand?
 *  - 
 */
public class ShopManager : MonoBehaviour
{
    // shop UI game objects
    public List<GameObject> pieces;
    public List<GameObject> upgrades; // not added yet
    public List<GameObject> combos;

    // pieces/Combos that can appear in the shop
    public List<GameObject> piecePool;
    public List<ComboScriptable> comboPool;
    public List<Sprite> sprites;



    // reference to the players spellbook (singleton)
    public GameObject spellBook;

    void Start()
    {
        // assign a random piece/combo from the pools to Shop UI game objects
        AssignShopItems();

        // find the spellbook object in the scene (to add bought combos)
        spellBook = GameObject.FindWithTag("SpellBook");
    }

    /*
     * Assigns random piece/combo from the pools to each shop button. Also
     * prevents duplicates from appearing in the shop.
     */
    void AssignShopItems()
    {
        // assign random piece prefabs to piece buttons
        for (int i = 0; i < pieces.Count; i++)
        {
            ShopData data = pieces[i].GetComponent<ShopData>();
            if (data == null)
            {
                Debug.Log("Piece missing ShopData component");
                continue;
            }

            if (piecePool.Count == 0)
            {
                Debug.LogWarning("No piece prefabs assigned");
                continue;
            }

            // pick a random prefab
            int index = Random.Range(0, piecePool.Count);
            GameObject prefab = piecePool[index];
            Sprite sprite = sprites[index];

            // assign it to ShopData
            data.piecePrefab = prefab;

            // update button text if prefab has a PieceScriptable or name component
            //TMP_Text text = pieces[i].GetComponentInChildren<TMP_Text>();
            //if (text != null)
            //{
            Piece pieceComponent = prefab.GetComponent<Piece>();
            //text.text = pieceComponent.pieceData.pieceName;
            pieces[i].GetComponent<Image>().sprite = sprite;
            //}
        }

        // assign random combos to combo buttons, ignore already unlocked combos
        // temp list of combos that arent unlocked yet (should replace later maybe)
        List<ComboScriptable> lockedCombos = new List<ComboScriptable>();
        foreach (var combo in comboPool)
        {
            if (!Spellbook.instance.combosUnlocked.Contains(combo))
            {
                lockedCombos.Add(combo);
            }
        }

        // list to track which combos are already assigned in this shop
        List<ComboScriptable> assignedCombos = new List<ComboScriptable>();

        for (int i = 0; i < combos.Count; i++)
        {
            ShopData data = combos[i].GetComponent<ShopData>();
            if (data == null) continue;

            // filter out combos already assigned to other buttons
            List<ComboScriptable> availableCombos = lockedCombos.FindAll(c => !assignedCombos.Contains(c));

            // if player unlocked all combos or no new combo is available, display sold out
            if (availableCombos.Count == 0)
            {
                TMP_Text text = combos[i].GetComponentInChildren<TMP_Text>();
                if (text != null) text.text = "SOLD OUT";

                // disable button
                combos[i].GetComponent<UnityEngine.UI.Button>().interactable = false;
                // skip the rest of the loop
                continue;
            }

            // get a random combo from the available combos list
            data.combo = availableCombos[Random.Range(0, availableCombos.Count)];

            // mark this combo as assigned so it won't appear again in this shop
            assignedCombos.Add(data.combo);

            // update button text to match spell name
            TMP_Text comboText = combos[i].GetComponentInChildren<TMP_Text>();
            if (comboText != null && data.combo != null)
            {
                comboText.text = data.combo.name;
            }
        }
    }

    /*
     * Called when a piece button is clicked. 
     */
    public void BuyPiece(GameObject obj)
    {
        ShopData data = obj.GetComponent<ShopData>();

        // only continue if the button has ShopData and a valid piece assigned
        if (data != null && data.piecePrefab != null)
        {
            // add the piece to the players deck (commented out for now)
            DeckManager.instance.AddPiece(data.piecePrefab);

            // hide button when piece is bought
            obj.SetActive(false);
        }
    }

    /*
     * Called when a piece button is clicked. 
     */
    public void BuyCombo(GameObject obj)
    {
        ShopData data = obj.GetComponent<ShopData>();

        // only continue if the button has ShopData and a valid combo assigned
        if (data != null && data.combo != null)
        {
            // unlock the combo in players Spellbook
            Spellbook.instance.UnlockCombo(data.combo);

            // hide button when piece is bought
            obj.SetActive(false);
        }
    }
}