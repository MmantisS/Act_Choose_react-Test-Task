using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NovelController : MonoBehaviour
{
    List<string> data = new List<string>();
    public static NovelController instance;
    int lightLevel = 0;
    int maxLightLevel = 40;

    private void Awake()
    {
        instance = this;
        chapters.Add("beginning", ChapterHolder.beginning);
        chapters.Add("chapter_a", ChapterHolder.chapter_a);
        chapters.Add("chapter_b", ChapterHolder.chapter_b);
        chapters.Add("chapter_c", ChapterHolder.chapter_c);
        chapters.Add("chapter_d", ChapterHolder.chapter_d);
        chapters.Add("chapter_e", ChapterHolder.chapter_e);
        chapters.Add("chapter_f", ChapterHolder.chapter_f);
        chapters.Add("chapter_g", ChapterHolder.chapter_g);
        chapters.Add("chapter_j", ChapterHolder.chapter_j);
        chapters.Add("chapter_k", ChapterHolder.chapter_k);
        chapters.Add("chapter_l", ChapterHolder.chapter_l);
        chapters.Add("deathQuria", ChapterHolder.deathQuria);
    }

    int progress = 0;
    public string cachedLastSpeaker = "";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Next();
        }
        if (timerIsRunning)
            CountdownTime();
        lightLevelBar.fillAmount = (float)lightLevel / (float)maxLightLevel;
    }
    public GameObject Statistics;
    public Text StatsText;
    public Image lightLevelBar;
    public Image timerBar;
    public float maxTime = 5f;
    float timeLeft;
    bool timerIsRunning;
    public static bool timeElapsed;
    string forcedElapsedAction;

    public void LaunchTimer()
    {
        timerIsRunning = true;
        timerBar.gameObject.SetActive(true);
        timeLeft = maxTime;
        timeElapsed = false;
    }

    public void StopTimer()
    {
        timerIsRunning = false;
        timerBar.gameObject.SetActive(false);
        timeElapsed = false;
        timeLeft = maxTime;
    }
    public void CountdownTime()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerBar.fillAmount = timeLeft / maxTime;
        }
        else
        {
            timeElapsed = true;
            timerBar.gameObject.SetActive(false);
        }
    }
    Dictionary<string, bool> visitedChapters = new Dictionary<string, bool>();
    Dictionary<string, List<string>> chapters = new Dictionary<string, List<string>>();
    string currentChapter;
    public void LoadChapterFile(string filename)
    {
        chapterProgress = 0;
        data = chapters[filename];
        currentChapter = filename;
        CharacterManager cm = CharacterManager.instance;
        MenuController mc = MenuController.instance;
        Statistics.SetActive(false);
        mc.EnablePanelUIRoot();
        StopTimer();
        mc.DisableMenu();
        mc.DisableChapterChoice();
        foreach (Character character in cm.characters)
        {
            character.Hide();
        }
        cachedLastSpeaker = "";
        if (handlingChapterFile != null)
            StopCoroutine(handlingChapterFile);
        handlingChapterFile = StartCoroutine(HandlingChapterFile());
        Next();
    }

    bool _next = false;
    public void Next()
    {
        _next = true;
        lightLevelBar.fillAmount = lightLevel / maxLightLevel;
    }

    void HandleLine(string line)
    {

        string[] dialogueandActions = line.Split('|');
        if (dialogueandActions.Length == 3)
        {
            HandleDialogue(dialogueandActions[0], dialogueandActions[1]);
            HandleEventsFromLine(dialogueandActions[2]);
        }
        else
        {
            HandleEventsFromLine(dialogueandActions[0]);
        }
    }
   void HandleDialogue(string dialogueDetails, string dialogue)
    {
        string speaker = cachedLastSpeaker;
        bool additive = dialogueDetails.Contains("+");

        if (additive)
            dialogueDetails = dialogueDetails.Remove(dialogueDetails.Length - 1);

        if (dialogueDetails.Length > 0)
        {
            if (dialogueDetails[dialogueDetails.Length - 1] == ' ')
                dialogueDetails = dialogueDetails.Remove(dialogueDetails.Length - 1);

            speaker = dialogueDetails;
            cachedLastSpeaker = speaker;
        }
        if(speaker != "narrator")
        {
            Character character = CharacterManager.instance.GetCharacter(speaker);
            character.Say(dialogue, additive);
        }
        else
        {
            DialogueSystem.instance.Say(dialogue, speaker, additive);
        }
    }
    void HandleEventsFromLine(string events)
    {
        string[] actions = events.Split(' ');
        foreach(string action in actions)
        {
            HandleAction(action);
        }

    }
    public bool isHandlingChapterFile { get { return handlingChapterFile != null; } }
    Coroutine handlingChapterFile = null;
    [HideInInspector] public int chapterProgress = 0;
    IEnumerator HandlingChapterFile()
    {
        //the progress through the lines in this chapter.
        chapterProgress = 0;
        while (chapterProgress < data.Count)
        {
            //we need a way of knowing when the player wants to advance. We need a "next" trigger. Not just a keypress. But something that can be triggerd
            //by a click or a keypress
            if (_next)
            {
                _next = false;
                string line = data[chapterProgress];//this is the line loaded in its pure format. No injections have taken place yet.
                //make sure the line has the proper data injected in it where it needs it.

                //this is a choice
                if (line.StartsWith("choice"))
                {
                    yield return HandlingChoiceLine(line);
                    chapterProgress++;
                }
                //this is a normal line of dialogue and actions.
                else
                {
                    HandleLine(line);
                    chapterProgress++;
                }
            }
            yield return new WaitForEndOfFrame();
        }
        handlingChapterFile = null;
    }
    IEnumerator HandlingChoiceLine(string line)
    {
        string title = line.Split('|')[1];
        List<string> choices = new List<string>();
        List<string> actions = new List<string>();

        bool gatheringChoices = true;
        while (gatheringChoices)
        {
            chapterProgress++;
            line = data[chapterProgress];

            if (line == "{")
                continue;

            line = line.Replace("    ", "");//remove the tabs that have become quad spaces.

            if (line != "}")
            {
                choices.Add(line.Split('|')[1]);
                actions.Add(data[chapterProgress + 1].Trim());
                chapterProgress++;
            }
            else
            {
                gatheringChoices = false;
            }
        }

        //display choices
        if (choices.Count > 0)
        {
            string action;
            ChoiceScreen.Show(title, choices.ToArray()); yield return new WaitForEndOfFrame();
            while (ChoiceScreen.isWaitingForChoiceToBeMade)
            {
                if (timeElapsed)
                {
                    ChoiceScreen.overrideByTimer = false;
                }
                yield return new WaitForEndOfFrame();
            }
            //choice is made. execute the paired action.
            if (timeElapsed)
            {
                action = forcedElapsedAction;
            }
            else
            {
                StopTimer();
                action = actions[ChoiceScreen.lastChoiceMade.index];
            }
            HandleLine(action);
        }
        else
        {
            Debug.LogError("Invalid choice operation. No choices were found.");
        }
    }

    public void GetStats()
    {
        if (Statistics.activeSelf)
            Statistics.SetActive(false);
        else
            Statistics.SetActive(true);
        StatsText.text = "";
        string allStats = "";
        allStats += lightLevel.ToString() + " " + maxLightLevel.ToString() + "\n";
        allStats += "Chapter C visited " + visitedChapters.ContainsKey("chapter_c") + "\n";
        allStats += "Chapter J visited " + visitedChapters.ContainsKey("chapter_j") + "\n";
        allStats += "Chapter E visited " + visitedChapters.ContainsKey("chapter_e") + "\n";
        allStats += "Chapter F visited " + visitedChapters.ContainsKey("chapter_f") + "\n";

        StatsText.text += allStats;
    }

    void HandleAction(string action)
    {
        string[] data = action.Split('(', ')');
        switch(data[0])
        {
            case "setBackground":
                Command_SetLayerImage(data[1], BackgroundController.instance.background);
                Next();
                break;
            case "LoadChapter":
                NovelController.instance.LoadChapterFile(data[1]);
                break;
            case "HideChar":
                Command_HideCharacter(data[1]);
                Next();
                break;
            case "ShowChar":
                Command_ShowCharacter(data[1]);
                Next();
                break;
            case "IncreaseLL":
                Command_IncreaseLightLevel(data[1]);
                Next();
                break;
            case "TimedChoice":
                Command_TimedChoiceForce(data[1]);
                Next();
                break;
            case "Visited":
                Command_ChapterVisited(currentChapter);
                Next();
                break;
        }
    }

    void Command_SetLayerImage(string data, BackgroundController.Layer layer)
    {
        string texName = data.Contains(",") ? data.Split(',')[0] : data;
        Texture tex = Resources.Load("Backgrounds/" + texName) as Texture;
        float spd = 2f;
        bool smooth = false;

        if (data.Contains(","))
        {
            string[] parameters = data.Split(',');
            foreach(string p in parameters)
            {
                float fval = 0;
                bool bval = false;
                if (float.TryParse(p, out fval))
                    spd = fval; continue;
                if (bool.TryParse(p, out smooth))
                    smooth = bval; continue;
            }
        }

        layer.TransitionToTexture(tex, spd, smooth);
    }

    void Command_IncreaseLightLevel(string value)
    {
        var incrementValue = int.Parse(value);
        if (!visitedChapters.ContainsKey(currentChapter))
            lightLevel += incrementValue;
        lightLevelBar.fillAmount = (float)lightLevel / (float)maxLightLevel;
    }

    void Command_HideCharacter(string name)
    {
        CharacterManager cm = CharacterManager.instance;
        Character character = cm.GetCharacter(name);
        character.Hide();
    }
    void Command_ShowCharacter(string name)
    {
        CharacterManager cm = CharacterManager.instance;
        Character character = cm.GetCharacter(name);
        character.Show();
    }
    string timedForcedAction;
    void Command_TimedChoiceForce(string time)
    {
        maxTime = float.Parse(time);
        chapterProgress++;
        forcedElapsedAction = data[chapterProgress];
        LaunchTimer();
    }
    void Command_ChapterVisited(string chapterName)
    {
        if (!visitedChapters.ContainsKey(chapterName))
            visitedChapters.Add(chapterName, true);
        
    }
}
