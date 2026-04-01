using UnityEngine;
using TMPro;

public class VFXManager : MonoBehaviour
{
    public static VFXManager instance;

    public GameObject numberVfx;
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


    public void SpawnNumber(float amount)
    {
        
        TextMeshProUGUI text = Instantiate(numberVfx, numberSpawnPos.position, Quaternion.identity).GetComponent<TextMeshProUGUI>();
        //text.text = amount.ToString();
    }
}
