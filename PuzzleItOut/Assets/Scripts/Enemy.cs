using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public float damage;

    public Animator animator;

    public Slider enemyHealthSlider;

    [Header("Enemy Sprites")]
    public Sprite[] enemySprites; 
    private static int lastEnemyIndex = -1; 

    private Image enemyImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get enemy image child
        Transform imageChild = transform.Find("Image");
        if (imageChild != null)
        {
            enemyImage = imageChild.GetComponent<Image>();
            if (enemySprites != null && enemySprites.Length > 0 && enemyImage != null)
            {
                enemyImage.sprite = GetRandomEnemySprite();
            }
        }
        else
        {
            Debug.LogWarning("Enemy child 'Image' not found!");
        }

        health = maxHealth;
        enemyHealthSlider.value = health / maxHealth;
    }

    Sprite GetRandomEnemySprite()
    {
        int index;
        do
        {
            index = Random.Range(0, enemySprites.Length);
        } while (index == lastEnemyIndex && enemySprites.Length > 1);

        lastEnemyIndex = index;
        return enemySprites[index];
    }


    public void TakeDamage(float damage)
    {
        health -= damage;
        enemyHealthSlider.value = health / maxHealth;

        //NumberVFX
        VFXManager.instance.SpawnNumber(VFXManager.instance.numberSpawnPos.position, damage);
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