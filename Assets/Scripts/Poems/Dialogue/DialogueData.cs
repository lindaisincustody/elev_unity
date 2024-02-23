using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Custom/DialogueData")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class CharacterData
    {
        public bool isYourText;
        public string dialogueLineText;
    }

    public string otherCharacterName;
    public CharacterData[] textList;
    public Sprite mainCharacterImage;
    public Sprite otherCharacterImage;
    public bool activateFight;
}
