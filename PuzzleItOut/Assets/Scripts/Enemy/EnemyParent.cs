
using UnityEngine;

public abstract class EnemyParent : MonoBehaviour
{
    public int maxHealth;
    public int health;

    public int Health
    {
        get {  return health; }
        set { health = value; GameManager.instance.sliderManager.UpdateEnemyHealthSlider(health, maxHealth); }
    }


    public int combatPower;

    public Animator animator;

    private void Start()
    {
        health = maxHealth;
        
    }

    public virtual void Attack()
    {
        Player.instance.TakeDamage(combatPower);
    }
    public virtual void TakeDamage(int damage)
    {
        Health = Mathf.Clamp(health - damage, 0, maxHealth);
    }

}
