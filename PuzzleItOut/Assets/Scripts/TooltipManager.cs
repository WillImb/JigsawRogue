using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;

    public Canvas canvas;
    public GameObject tooltipPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI typeText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI healingText;
    public TextMeshProUGUI combatText;
    public TextMeshProUGUI goldText;

    public Vector2 offset = new Vector2(20f, 0f);
    public float showDelay = 0.25f;

    private float hoverTimer;
    private bool pendingShow;

    void Awake()
    {
        instance = this;
        tooltipPanel.SetActive(false);
    }

    void Update()
    {
        if (pendingShow)
        {
            hoverTimer += Time.deltaTime;
            if (hoverTimer >= showDelay)
            {
                tooltipPanel.SetActive(true);
                pendingShow = false;
            }
        }

        if (tooltipPanel.activeSelf)
        {
            Vector2 screenPos = InputManager.Instance.Gameplay.Point.ReadValue<Vector2>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                screenPos + offset,
                canvas.worldCamera,
                out Vector2 localPoint
            );
            ((RectTransform)tooltipPanel.transform).anchoredPosition = localPoint;
        }
    }

    public void ShowTooltip(PieceScriptable data)
    {
        nameText.text    = data.pieceName.ToUpper();
        typeText.text    = data.cardType.ToString().ToUpper();
        damageText.text  = $"DMG :{data.baseDamange}";
        healingText.text = $"HEAL :{data.healingValue:F0}";
        combatText.text  = $"CMB :{data.combatValue:F0}";
        goldText.text    = $"GOLD :{data.goldValue:F0}";

        pendingShow = true;
        hoverTimer  = 0f;
    }

    public void HideTooltip()
    {
        pendingShow = false;
        hoverTimer  = 0f;
        tooltipPanel.SetActive(false);
    }
}