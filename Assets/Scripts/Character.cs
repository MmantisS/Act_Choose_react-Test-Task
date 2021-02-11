using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Character
{
    public string Name;
    public RectTransform root;
    DialogueSystem dialogue;
    public bool enabled { get { return root.gameObject.activeInHierarchy; } set { root.gameObject.SetActive(value); } }
    public void Say(string s, bool add = false)
    {
        if (!enabled)
            enabled = true;
        dialogue.Say(s, Name, add);
    }
    public Character(string name, bool enableOnStart = true)
    {
        if (name != "")
        {
            CharacterManager cm = CharacterManager.instance;
            GameObject prefab = Resources.Load("Characters/Character[" + name + "]") as GameObject;
            if (prefab != null)
            {
                GameObject ob = GameObject.Instantiate(prefab, cm.characterPanel);
                root = ob.GetComponent<RectTransform>();
                Name = name;

                renderers.renderer = ob.GetComponentInChildren<Image>();
                dialogue = DialogueSystem.instance;
                enabled = enableOnStart;
            }
        }
    }

    public void Hide()
    {
        root.gameObject.SetActive(false);
    }
    public void Show()
    {
        root.gameObject.SetActive(true);
    }

    [System.Serializable]
    public class Renderers
    {
        public Image renderer;
    }
    public Renderers renderers = new Renderers();
}
