using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance;
    public UIELements elements;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
         
    }

    // Update is called once per frame
    public void Say(string speech, string speaker = "", bool additive = false)
    {
        StopSpeech();
        if (additive)
            speechText.text = targetSpeech;
        speaking = StartCoroutine(Speaking(speech, additive, speaker));
    }

    public void StopSpeech()
    {
        if (isSpeaking)
        {
            StopCoroutine(speaking);
        }
        speaking = null;
    }
    public bool isSpeaking { get { return speaking != null; } }
    public bool waitingForInput = false;
    string targetSpeech="";
    Coroutine speaking = null;
    IEnumerator Speaking(string speech, bool additive = false ,string speaker = "")
    {
        speechPanel.SetActive(true);
        targetSpeech = speech;
        if (!additive)
            speechText.text = "";
        else
            speech = speechText.text + targetSpeech;
        speakerNameText.text = DetermineSpeaker(speaker);
        speakerNamePanel.SetActive(speakerNameText.text != "");
        
        while(speechText.text != speech)
        {
            speechText.text += speech[speechText.text.Length];
            yield return new WaitForEndOfFrame();
        }

        waitingForInput = true;
        while(waitingForInput)
        {
            yield return new WaitForEndOfFrame();
        }
        StopSpeech();
    }

    string DetermineSpeaker(string s)
    {
        string retVal = speakerNameText.text;
        if (s != speakerNameText.text && s != "")
            retVal = (s.ToLower().Contains("narrator") ? "" : s);

        return retVal;
    }

    public void Close()
    {
        StopSpeech();
        speechPanel.SetActive(false);
    }

    [System.Serializable]
    public class UIELements
    {
        public GameObject speechPanel;
        public Text speakerNameText;
        public Text speechText;
        public GameObject speakerNamePanel;
    }

    public GameObject speechPanel { get { return elements.speechPanel;}}
    public Text speakerNameText { get { return elements.speakerNameText;}}
    public Text speechText { get { return elements.speechText;}}

    public GameObject speakerNamePanel { get { return elements.speakerNamePanel; } }

}
