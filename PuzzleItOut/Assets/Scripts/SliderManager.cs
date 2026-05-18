using UnityEngine;
using UnityEngine.UI;

/*
 * Class: SliderManager
 * Date: 5.18.26
 * Notes:
 *  - Handles updating the text and values for player and enemy resource (health and mana) bars
 *  - 
 */
public class SliderManager : MonoBehaviour
{
    Player player;

    public Slider playerHealthSlider;
    public Slider playerManaSlider;

    void Start()
    {
        player = Player.instance;

        player.OnHealthChanged += UpdatePlayerHealthSlider;
        player.OnManaChanged += UpdatePlayerManaSlider;

        UpdatePlayerHealthSlider(player.GetHealth(), player.maxHealth);
        UpdatePlayerManaSlider(player.GetMana(), player.maxMana);
    }

    void OnDestroy()
    {
        player.OnHealthChanged -= UpdatePlayerHealthSlider;
        player.OnManaChanged -= UpdatePlayerManaSlider;
    }

    void UpdateSlider(Slider slider, int value, int maxValue)
    {
        slider.value = value / (float)maxValue;
    }

    void UpdatePlayerHealthSlider(int value, int maxValue)
    {
        UpdateSlider(playerHealthSlider, value, maxValue);
    }

    void UpdatePlayerManaSlider(int value, int maxValue)
    {
        UpdateSlider(playerManaSlider, value, maxValue);
    }
}
