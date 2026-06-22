using UnityEngine;
using TMPro;

/*
 * Author(s): Anthony L
 * Date: 6.22.26
 * Notes:
 *  - Added rarity stuff
 */
public class ShopData : MonoBehaviour
{
    public GameObject piecePrefab;
    public UpgradeData upgrade;
    public ComboScriptable combo;

    public int cost;

    // after an upgrade is purchased, disable corresponding upgrade button
    public GameObject linkedUpgradeButton;

    void Awake()
    {
        TMP_Text text = GetComponentInChildren<TMP_Text>();
        if (text == null) return;

        string rarity = text.text;

        if (rarity == "COMMON")
        {
            cost = 5;
        }
        else if (rarity == "UNCOMMON")
        {
            cost = 10;
        }
        else if (rarity == "RARE")
        {
            cost = 15;
        }
    }
}