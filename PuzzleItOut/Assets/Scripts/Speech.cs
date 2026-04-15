using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Speech : MonoBehaviour
{
    public string[] speech;
    public int line;
    public string message;
    public TextMeshProUGUI textBox;
    public float speed;

    Coroutine currentLine;

    public GameObject progressBtn;
    public GameObject endBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        line = 0;
        progressBtn.SetActive(true);
        endBtn.SetActive(false);
        message = speech[line];
        currentLine = StartCoroutine(SpeechCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator SpeechCoroutine()
    {
        textBox.text = "";
        int index = 0;
        while (index < message.Length)
        {
            textBox.text += message.Substring(index, 1);
            index++;
            yield return new WaitForSeconds(speed);
        }
    }

    public void ProgressSpeech()
    {
        //if still talking finish their message
        if(textBox.text != message)
        {
            StopCoroutine(currentLine);
            textBox.text = message;
        }
        //if done talking
        else
        {
            line++;
            //if this is their last line appear the button that ends their speech -- if they have one
            if (line == speech.Length-1 && progressBtn && endBtn)
            {
                progressBtn.SetActive(false);
                endBtn.SetActive(true);
            }            
                message = speech[line];
                currentLine = StartCoroutine(SpeechCoroutine());

            
        }
    }

    


}
