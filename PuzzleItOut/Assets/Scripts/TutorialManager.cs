using System;
using System.Collections;
using System.Collections.Generic;
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
    


    public void TutorialCast()
    {
        //game manager do turn        

        List<PieceScriptable> currentPieces = BoardManager.instance.GetBoardPieces();

        int comboIndex = CombatManager.Instance.FindCombo(currentPieces);
        ComboScriptable combo = comboIndex >= 0 ? Spellbook.instance.combosUnlocked[comboIndex] : null;

        if (combo != null)
        {
            Debug.Log("yoooo");
            StartCoroutine(CastTutCoroutine());
            
        }
        else
        {
            //if player casted a incorrect spell
            HideTutPanel(8);
            ShowTutPanel(9);
        }

    }
    public IEnumerator CastTutCoroutine()
    {
        //called if player casts a successful spell
        float curHealth = Player.instance.GetHealth();

        HideTutPanel(8);

        GameManager.instance.DoTurn();
        while(curHealth <= Player.instance.GetHealth())
        {
           
            yield return new WaitForEndOfFrame();
        }

        ShowTutPanel(10);
    }


}
