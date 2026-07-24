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

    public string rarity;
    public int cost;

    // after an upgrade is purchased, disable corresponding upgrade button
    public GameObject linkedUpgradeButton;

    public void SetRarity(string newRarity)
    {
        rarity = newRarity;

        switch (rarity)
        {
            // upgrades
            case "COMMON":
                cost = 5;
                break;
            case "RARE":
                cost = 10;
                break;
            case "RAREST":
                cost = 15;
                break;

            // combos
            case "x2":
                cost = 10;
                break;
            case "x3":
                cost = 15;
                break;
            case "x4":
                cost = 20;
                break;
            case "FORBIDDEN":
                cost = 25;
                break;

            default:
                cost = 0;
                break;
        }

        TMP_Text text = GetComponentInChildren<TMP_Text>();
        if (text != null)
            text.text = rarity;
    }
}