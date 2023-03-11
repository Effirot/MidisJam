using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    

    public void NextStep() {

    }

}


[CreateAssetMenu(fileName = "DialogueCharacter", menuName = "DialogueSystem/Character", order = 1)]
public class DialogueCharacter : ScriptableObject {
    public string Name;
    public Sprite Sprite;
}

[CreateAssetMenu(fileName = "DialogueData", menuName = "DialogueSystem/Data", order = 1)]
public sealed class DialogueData : ScriptableObject
{
    Phrase this[int index] { get => AllPhrases[index]; }

    [SerializeField]List<Phrase> AllPhrases;

    public struct Phrase{
        DialogueCharacter character;
        string text;        
    }
} 

