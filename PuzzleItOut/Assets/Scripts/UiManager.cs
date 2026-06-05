using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    public TextMeshProUGUI deckCount;
    public TextMeshProUGUI discardCount;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        deckCount.text = DeckManager.instance.physicalDeck.Count.ToString();
        discardCount.text = DeckManager.instance.discard.Count.ToString();
    }
}
