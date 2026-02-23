using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;
    public float damage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    public void DealDamage()
    {
        Player.instance.TakeDamage(damage);
    }
}
