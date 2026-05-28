using UnityEngine;

public class ShopData : MonoBehaviour
{
    public GameObject piecePrefab;
    public UpgradeData upgrade;
    public ComboScriptable combo;

    public int goldValue;

    // after an upgrade is purchased, disable corresponding upgrade button
    public GameObject linkedUpgradeButton;
}
