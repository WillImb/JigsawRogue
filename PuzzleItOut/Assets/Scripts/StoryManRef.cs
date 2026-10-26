using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryManRef : MonoBehaviour
{
    public static StoryManRef instance;
    public Image profile;
    public TextMeshProUGUI speechBox;
    public Button progButton;
    public TextMeshProUGUI clickAnywhereText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if(instance == null)
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
