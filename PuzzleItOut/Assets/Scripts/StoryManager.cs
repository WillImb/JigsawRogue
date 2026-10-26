using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;

[System.Serializable]
public class LevelDialog
{
    public int index;
    public List<StoryManager.DialogSequence> dialogSequences = new List<StoryManager.DialogSequence>();

}

[System.Serializable]
public class Levels
{
    public List<LevelDialog> levels = new List<LevelDialog>();
}

public class StoryManager : MonoBehaviour
{
    public static StoryManager instance;


    //These fields will be updated at the start based on the players saved data
    bool isTutorialComplete;
    int level;

    //Dialog 
    string dialogPath;

    Levels dialogsList;


    [System.Serializable]
    public struct DialogSequence
    {
        public int characterIndex;
        public string speech;
    }

    public DialogCharacterScriptable[] dialogCharacters;

    //Active dialog
    GameObject dialogBox;
    Image profile;
    TextMeshProUGUI speechBox;
    TextMeshProUGUI contText;
    Button progBtn;

    string speech;
    string goalSpeech;
    public float talkSpeed;
    public bool progressSpeech;
    public bool skipSpeech;


    //Enemies
    public GameObject[] enemiesPrefab; 




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            dialogPath = Path.Combine(Application.dataPath, "dialogData.json");
            PopulateFields();
            PopulateDialog();
            
            
        }
        else
        {
            Destroy(this);
        }
    }

    void WriteDialog()
    {
        DialogSequence sq = new DialogSequence();
        sq.characterIndex = 0;
        sq.speech = "hello";

        LevelDialog ld = new LevelDialog();
        ld.index = 0;
        ld.dialogSequences.Add(sq);

        DialogSequence sq2 = new DialogSequence();
        sq2.characterIndex = 1;
        sq2.speech = "bye";

        LevelDialog ld2 = new LevelDialog();
        ld2.index = 1;
        ld2.dialogSequences.Add(sq2);

        Levels l = new Levels();        
        l.levels.Add(ld);
        l.levels.Add(ld2);

        string jsonString = JsonUtility.ToJson(l,true);
        
        File.WriteAllText(dialogPath, jsonString);
        Debug.Log("written1");
    }

    void PopulateFields()
    {
        dialogBox = StoryManRef.instance.speechBox.transform.parent.gameObject;
        profile = StoryManRef.instance.profile;
        speechBox = StoryManRef.instance.speechBox;
        contText = StoryManRef.instance.clickAnywhereText;
        progBtn = StoryManRef.instance.progButton;
        progBtn.onClick.AddListener(ProgressSpeech);

    }
    void PopulateDialog()
    {
        dialogsList = JsonUtility.FromJson<Levels>(File.ReadAllText(dialogPath));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool HasDialog(int level)
    {
        foreach(LevelDialog l in dialogsList.levels)
        {
            if(l.index == level)
                return true;
            else if(l.index > level)
            {
                return false;
            }
        }

        return false;
    }

    public IEnumerator ExchangeDialog(int level)
    {
        dialogBox.SetActive(true);

        
        List<DialogSequence> dses = new List<DialogSequence>();
        foreach (LevelDialog l in dialogsList.levels)
        {
            if (l.index == level)
            {
                dses = l.dialogSequences;
            }
        }

       

        for (int k = 0; k < dses.Count; k++) {
            goalSpeech = dses[k].speech;
            profile.sprite = dialogCharacters[dses[k].characterIndex].profile;
            speech = "";
            Debug.Log("b"+k);
            speechBox.text = speech;
            for (int i = 0; i < dses[k].speech.Length; i++)
            {
                speech = dses[k].speech.Substring(0, i+1);
                if (skipSpeech)
                {
                    break;
                }
                yield return new WaitForSeconds(talkSpeed);
                speechBox.text = speech;

            }
            skipSpeech = false;
            speech = goalSpeech;
            speechBox.text = speech;
            contText.gameObject.SetActive(true);
            yield return new WaitUntil(() => progressSpeech);
            contText.gameObject.SetActive(false);
            progressSpeech = false;
        }

        
        dialogBox.SetActive(false);
        yield return new WaitForEndOfFrame();
    }
    public void ProgressSpeech()
    {
        if(speech == goalSpeech)
        {
            progressSpeech = true;
        }
        else
        {
            skipSpeech = true;
            
        }
    }
}
