using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public float damage;

    public Animator animator;

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

        //NumberVFX
        VFXManager.instance.SpawnNumber(VFXManager.instance.numberSpawnPos.position,damage);
        VFXManager.instance.SpawnParticle(Vector2.up, 0);
        animator.SetTrigger("hurt");

    }

    public void DealDamage()
    {
        Player.instance.TakeDamage(damage);
        VFXManager.instance.SpawnParticle(new Vector3(5.5f, 0, 0), 3);
        VFXManager.instance.SpawnNumber(new Vector3(5.5f, 0, 0), damage);


    }
}
