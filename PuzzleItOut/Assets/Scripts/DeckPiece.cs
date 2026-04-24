using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckPiece : MonoBehaviour
{
    public int index;

    private Button button;
    private Image image;
    private TextMeshProUGUI text;

    void Start()
    {
        // cache components
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        Transform textChild = transform.Find("Text");
        if (textChild != null)
        {
            text = textChild.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogWarning("No 'Text' child found on DeckPiece");
        }

        // hook up button click automatically
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        DeckManager.instance.UpgradePiece(index);

        ShopManager.instance.SetUpgradedPanelActive(true);

        // disable the upgrade button that opened this
        ShopManager.instance.DisableCurrentUpgradeButton();

        if (button != null)
        {
            button.interactable = false;
        }

        if (image != null)
        {
            Color c = image.color;
            c.a = 0.3f;
            image.color = c;
        }
    }
}