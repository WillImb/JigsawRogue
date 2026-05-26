using TMPro;
using UnityEngine;

public class MoneyBag : MonoBehaviour
{
    public Animator animator;
    public TextMeshProUGUI goldText;

    private void Start()
    {
        UpdateGoldText();

        if (GoldManager.Instance != null)
            GoldManager.Instance.OnGoldChanged += UpdateGoldText;
    }

    private void OnDestroy()
    {
        if (GoldManager.Instance != null)
            GoldManager.Instance.OnGoldChanged -= UpdateGoldText;
    }

    private void OnParticleCollision(GameObject other)
    {
        // animator.SetTrigger("gold");

        GoldManager.Instance.AddGold(1);
    }

    private void UpdateGoldText(int gold)
    {
        goldText.text = gold.ToString();
    }

    private void UpdateGoldText()
    {
        goldText.text = GoldManager.Instance.Gold.ToString();
    }
}