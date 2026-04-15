using System;
using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    public bool isPanelOpen;
    public int stage;

    public GameObject[] tutPanels;

    public Coroutine tutorialRoutine;
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (!Player.instance.completedTutorial)
        {
           // tutorialRoutine = StartCoroutine(TutorialCoroutine());
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    //The coroutine for the tutorial to manage what stage of 
    // the tutorial the player is on
    public IEnumerator TutorialCoroutine()
    {
        yield return new WaitForEndOfFrame();
    }


    //shows a specific tutorial panel
    public void ShowTutPanel(int index)
    {
        PanelManager.instance.ShowPanel(tutPanels[index]);
        isPanelOpen = true;
        PanelManager.instance.DisableInput();
    }

    //hides a specific tutorial panel
    public void HideTutPanel(int index)
    {
        PanelManager.instance.HidePanel(tutPanels[index]);
        isPanelOpen = false;
        PanelManager.instance.EnableInput();
    }

    public void ProgressStage()
    {
        stage += 1;
    }
    

}
