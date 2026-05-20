using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    Enemy enemy;

    public Slider playerHealthSlider;
    public Slider playerManaSlider;

    public Slider enemyHealthSlider;

    [SerializeField] TextMeshProUGUI playerHealthText;
    [SerializeField] TextMeshProUGUI playerManaText;
    [SerializeField] TextMeshProUGUI enemyHealthText;

    void Start()
    {
        // assign local refernces
        player = Player.instance;
        enemy = Enemy.instance;

        // subscribe to events
        player.OnHealthChanged += UpdatePlayerHealthSlider;
        player.OnManaChanged += UpdatePlayerManaSlider;
        enemy.OnHealthChanged += UpdateEnemyHealthSlider;

        // update GUI for game start
        UpdatePlayerHealthSlider(player.GetHealth(), player.maxHealth);
        UpdatePlayerManaSlider(player.GetMana(), player.maxMana);
        UpdateEnemyHealthSlider(enemy.health, enemy.maxHealth);
    }

    void OnDestroy()
    {
        player.OnHealthChanged -= UpdatePlayerHealthSlider;
        player.OnManaChanged -= UpdatePlayerManaSlider;
        enemy.OnHealthChanged -= UpdateEnemyHealthSlider;
    }

    /// <summary>
    /// Generic slider update method
    /// </summary>
    void UpdateSlider(Slider slider, int value, int maxValue)
    {
        slider.value = value / (float)maxValue;
    }

    void UpdatePlayerHealthSlider(int value, int maxValue)
    {
        // refresh slider ui
        UpdateSlider(playerHealthSlider, value, maxValue);
        // update text
        playerHealthText.text = player.health + "/" + player.maxHealth;
    }

    void UpdatePlayerManaSlider(int value, int maxValue)
    {
        UpdateSlider(playerManaSlider, value, maxValue);
        playerManaText.text = player.mana + "/" + player.maxMana;
    }

    void UpdateEnemyHealthSlider(int value, int maxValue)
    {
        // refresh slider ui
        UpdateSlider(enemyHealthSlider, value, maxValue);
        // update text
        enemyHealthText.text = enemy.health + "/" + enemy.maxHealth;
    }
}
