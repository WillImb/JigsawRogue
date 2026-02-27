using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public float damage;

    public Slider enemyHealthSlider;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
        enemyHealthSlider.value = health / maxHealth;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
    public void TakeDamage(float damage)
    {
        health -= damage;
        enemyHealthSlider.value = health / maxHealth;

    }

    public void DealDamage()
    {
        Player.instance.TakeDamage(damage);
    }
}
