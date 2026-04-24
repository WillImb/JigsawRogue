using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyBag : MonoBehaviour
{
    public Animator animator;

    public TextMeshProUGUI goldText;
    private void Start()
    {
        goldText.text = Player.instance.gold.ToString();   
    }
    private void OnParticleCollision(GameObject other)
    {
        // animator.SetTrigger("gold");
        Player.instance.gold += 1;
       goldText.text = Player.instance.gold.ToString();


    }


}
