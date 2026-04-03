using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class VFXManager : MonoBehaviour
{
    public static VFXManager instance;

    public GameObject numberVfx;

    public List<ParticleSystem> particles;


    public Transform numberSpawnPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SpawnNumber(Vector3 pos,float amount)
    {
        
        TextMeshProUGUI text = Instantiate(numberVfx, pos, Quaternion.identity).GetComponentInChildren<TextMeshProUGUI>();
        text.text = amount.ToString();
        
    }

    public void SpawnParticle(Vector3 pos, int index)
    {
        Instantiate(particles[index], pos, Quaternion.identity);
    }
}
