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

    public Sprite[] sprites;

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
        health = maxHealth;
        OnHealthChanged?.Invoke(health, maxHealth);

        GetComponentInChildren<Image>().sprite = sprites[0];
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        // prevents value from going below 0
        health = Mathf.Clamp(health, 0, maxHealth);

        OnHealthChanged?.Invoke(health, maxHealth);

        //NumberVFX
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
            VFXManager.instance.SpawnNumber(new Vector3(5.5f, 0, 0), damage);
            GameManager.instance.enemyRebound = false;
        }
        else if(GameManager.instance.enemyDamageReduced)
        {
            Player.instance.TakeDamage((int)(damage*0.5f));
            VFXManager.instance.SpawnNumber(new Vector3(5.5f, 0, 0), (int)(damage*0.5f));
            GameManager.instance.enemyDamageReduced = false;
        }
        else
        {
            Player.instance.TakeDamage(damage);
            VFXManager.instance.SpawnNumber(new Vector3(5.5f, 0, 0), damage);
        }
        VFXManager.instance.SpawnParticle(new Vector3(5.5f, 0, 0), 3);
    }
}
