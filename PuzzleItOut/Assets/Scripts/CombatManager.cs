using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void DealDmg(float)
    {
        return;
    }

    public void TakeDmg(float)
    {
        return;
    }
}
