using System;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public static Enemy instance;

    public int health;
    public int maxHealth;
    public int damage;

    public Animator animator;

    [Header("Enemy Sprites")]
    public Sprite[] sprites;          // standard enemy sprites
    public Sprite[] bossSprites;      // boss enemy sprites

    [Header("Background Settings")]
    public Image backgroundImage;      // reference to the UI Image displaying the background
    public Sprite[] normalBackgrounds; // normal background sprites
    public Sprite[] bossBackgrounds;   // boss background sprites

    [Header("State")]
    public bool isBoss;               // identifies if the current enemy is a boss

    public event Action<int, int> OnHealthChanged;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        SetupEnemy();
    }

    /// <summary>
    /// Configures health, enemy sprite, and background sprite based on whether the current round is a Boss round
    /// </summary>
    public void SetupEnemy()
    {
        health = maxHealth;
        OnHealthChanged?.Invoke(health, maxHealth);

        // check with GameManager if the current round is a boss round
        if (GameManager.instance != null && GameManager.instance.IsBossRound())
        {
            isBoss = true;
            ApplyBossSprite();
            ApplyBossBackground();
        }
        else
        {
            isBoss = false;
            ApplyNormalSprite();
            ApplyNormalBackground();
        }
    }

    private void ApplyBossSprite()
    {
        Image enemyImage = GetComponentInChildren<Image>();
        if (enemyImage != null && bossSprites != null && bossSprites.Length > 0)
        {
            enemyImage.sprite = bossSprites[UnityEngine.Random.Range(0, bossSprites.Length)];
        }
        else
        {
            ApplyNormalSprite();
        }
    }

    private void ApplyNormalSprite()
    {
        Image enemyImage = GetComponentInChildren<Image>();
        if (enemyImage != null && sprites != null && sprites.Length > 0)
        {
            enemyImage.sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];
        }
    }

    private void ApplyBossBackground()
    {
        if (backgroundImage != null && bossBackgrounds != null && bossBackgrounds.Length > 0)
        {
            backgroundImage.sprite = bossBackgrounds[UnityEngine.Random.Range(0, bossBackgrounds.Length)];
        }
    }

    private void ApplyNormalBackground()
    {
        if (backgroundImage != null && normalBackgrounds != null && normalBackgrounds.Length > 0)
        {
            backgroundImage.sprite = normalBackgrounds[UnityEngine.Random.Range(0, normalBackgrounds.Length)];
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        OnHealthChanged?.Invoke(health, maxHealth);

        // numberVFX
        if (damage > 0)
        {
            VFXManager.instance.SpawnNumber(VFXManager.instance.numberSpawnPos.position, damage);
            VFXManager.instance.SpawnParticle(Vector2.up, 0);
            animator.SetTrigger("hurt");
        }
    }

    public void DealDamage()
    {
        if (GameManager.instance.enemyRebound)
        {
            TakeDamage(damage);
            GameManager.instance.enemyRebound = false;
        }
        else if (GameManager.instance.acidRainDamageReduced)
        {
            Player.instance.TakeDamage((int)(damage * 0.5f));
            GameManager.instance.acidRainDamageReduced = false;
        }
        else if (GameManager.instance.ashfallDamageReduction)
        {
            Player.instance.TakeDamage((int)(damage * 0.5f));
            GameManager.instance.ashfallDamageReduction = false;
        }
        else
        {
            Player.instance.TakeDamage(damage);
        }
        VFXManager.instance.SpawnParticle(new Vector3(5.5f, 0, 0), 3);
    }
}
