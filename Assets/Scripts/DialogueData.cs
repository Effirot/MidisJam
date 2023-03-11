using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using TMPro;


[CreateAssetMenu(fileName = "DialogueData", menuName = "DialogueSystem/Data", order = 1)]
public sealed class DialogueData : ScriptableObject
{
    public Phrase this[int index] { get => AllPhrases[index]; }
    public int Length => AllPhrases.Count;

    [SerializeField]List<Phrase> AllPhrases;

    [System.Serializable]
    public struct Phrase{
        public DialogueCharacter LCharacter;
        public DialogueCharacter RCharacter;
        
        public SpeakerLR Speaker;
        
        [TextAreaAttribute(5, 100)]
        public string text;        
    }

    public enum SpeakerLR{
        Left, Right, Nobody
    }
} 