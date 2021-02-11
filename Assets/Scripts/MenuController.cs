using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static MenuController instance;
    public GameObject MenuPanel;
    public GameObject ChapterChoicePanel;
    public GameObject PanelUIRoot;
    private void Awake()
    {
        instance = this;
    }

    public void StartGame()
    {
        NovelController nv = NovelController.instance;
        nv.LoadChapterFile("beginning");      
    }

    public void EnableMenu()
    {
        MenuPanel.SetActive(true);
    }
    public void EnableChapterChoice()
    {
        ChapterChoicePanel.SetActive(true);
    }
    public void EnablePanelUIRoot()
    {
        PanelUIRoot.SetActive(true);
    }
    public void DisableMenu()
    {
        MenuPanel.SetActive(false);
    }
    public void DisableChapterChoice()
    {
        ChapterChoicePanel.SetActive(false);
    }
    public void DisablePanelUIRoot()
    {
        PanelUIRoot.SetActive(false);
    }


}
