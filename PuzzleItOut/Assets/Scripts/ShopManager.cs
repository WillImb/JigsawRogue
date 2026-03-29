using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public List<GameObject> pieces;
    public List<GameObject> upgrades;
    public List<GameObject> combos;

    public GameObject spellBook;

    void Start()
    {
        RandomizeListUnique(pieces);
        RandomizeListUnique(upgrades);
        RandomizeListUnique(combos);

        spellBook = GameObject.FindWithTag("SpellBook");
    }

    void RandomizeListUnique(List<GameObject> list)
    {
        // create a pool of numbers 1–5
        List<int> numberPool = new List<int> { 1, 2, 3, 4, 5 };

        // shuffle the pool
        for (int i = 0; i < numberPool.Count; i++)
        {
            int randomIndex = Random.Range(i, numberPool.Count);
            int temp = numberPool[i];
            numberPool[i] = numberPool[randomIndex];
            numberPool[randomIndex] = temp;
        }

        // assign first N unique values (N = list count, e.g., 3)
        for (int i = 0; i < list.Count; i++)
        {
            TMP_Text text = list[i].GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                text.text = numberPool[i].ToString();
            }
        }
    }
}