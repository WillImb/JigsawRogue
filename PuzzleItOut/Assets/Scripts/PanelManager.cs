using System;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public static PanelManager instance;
    public Button[] buttons;
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

    public void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
    }
    public void HidePanel(GameObject panel)
    {
        panel.SetActive(false);
    }
    public void DisableInput()
    {
        InputManager.Instance.gameObject.SetActive(false);
    }
    public void EnableInput()
    {
        InputManager.Instance.gameObject.SetActive(true);

    }

    //takes in an array of intergers and disables all the buttons in the buttons array
    public void DisableButtons(string nums)
    {
        int[] b = StringToArray(nums);

        for(int i = 0; i < b.Length; i ++)
        {
            buttons[b[i]].interactable = false;
        }
    }

    //takes in an array of intergers and enables all the buttons in the buttons array

    public void EnableButtons(string nums)
    {
        int[] b = StringToArray(nums);

        for (int i = 0; i < b.Length; i++)
        {
            buttons[b[i]].interactable = true;
        }
    }

    //Converts a string of comma separated numbers into an array
    // Must be in format "1,2,3"
    //This is done so it can be called from a buttons press
    public int[] StringToArray(string nums)
    {
        int[] indecies = Array.ConvertAll(nums.Split(","), int.Parse);

        return indecies;
    }
}
