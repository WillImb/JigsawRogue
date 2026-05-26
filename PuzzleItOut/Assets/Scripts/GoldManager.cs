using System;
using UnityEngine;

/*
 * Author(s): Anthony L, 
 * Date: 5.26.26
 * Notes:
 *  - Keeps track of gold value between scenes
 */
public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;

    [SerializeField] private int gold;

    public event Action<int> OnGoldChanged;

    public int Gold
    {
        get { return gold; }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SpendGold(int amount)
    {
        gold -= amount;

        if (gold < 0)
            gold = 0;

        OnGoldChanged?.Invoke(gold);
    }

    public void AddGold(int amount)
    {
        gold += amount;
        OnGoldChanged?.Invoke(gold);
    }

    public bool CanAfford(int amount)
    {
        return gold >= amount;
    }
}