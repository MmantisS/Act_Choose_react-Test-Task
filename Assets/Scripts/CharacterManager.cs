using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;

    public RectTransform characterPanel;

    public List<Character> characters = new List<Character>();

    public Dictionary<string, int> characterDIctionary = new Dictionary<string, int>();

    private void Awake()
    {
        instance = this;
    }

    public Character GetCharacter(string characterName, bool createCharacterIfDoesntExist = true, bool enableCreatedOnStart = true)
    {
        int index = -1;
        if (characterDIctionary.TryGetValue(characterName, out index))
        {
            return characters[index];
        }
        else
        {
            return CreateCharacter(characterName, enableCreatedOnStart);
        }
    }

    public Character CreateCharacter(string characterName, bool enableOnStart = true)
    {
        Character newChar = new Character(characterName, enableOnStart);

        characterDIctionary.Add(characterName, characters.Count);
        characters.Add(newChar);

        return newChar;
    }
}
