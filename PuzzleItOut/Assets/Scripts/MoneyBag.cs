using System.Collections.Generic;
using UnityEngine;

public class MoneyBag : MonoBehaviour
{
    public Animator animator;
    private void OnParticleCollision(GameObject other)
    {
        animator.SetTrigger("gold");
        
    }
   

}
