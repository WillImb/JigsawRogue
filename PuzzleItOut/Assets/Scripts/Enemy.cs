using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int health;
    public int maxHealth;
    public int damage;

    public Animator animator;

    public Slider enemyHealthSlider;

    public Sprite[] sprites;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
        enemyHealthSlider.value = health / (float)maxHealth;
               
        
        GetComponentInChildren<Image>().sprite = sprites[0];

        
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
    public void TakeDamage(int damage)
    {
        health -= damage;
        enemyHealthSlider.value = health / (float)maxHealth;

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
        Player.instance.TakeDamage(damage);
        VFXManager.instance.SpawnParticle(new Vector3(5.5f, 0, 0), 3);
        VFXManager.instance.SpawnNumber(new Vector3(5.5f, 0, 0), damage);


    }
}
