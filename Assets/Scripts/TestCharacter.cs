using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacter : MonoBehaviour
{
    public Character Zavala;
    // Start is called before the first frame update
    void Start()
    {
        Zavala = CharacterManager.instance.GetCharacter("Zavala", enableCreatedOnStart: false) ;
    }

    public string[] q = new string[]
{
        "Hey, Guardian, you back from : Zabala", "your mission? ",
        "xqcL here is your Engram",
        "Gimme my Engram"
};
    int i = 0;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown (KeyCode.Space))
        {
            if (i < q.Length)
                Zavala.Say(q[i]);
            else
                DialogueSystem.instance.Close();
            i++;           
        }
    }
}
