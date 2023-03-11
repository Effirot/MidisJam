using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using TMPro;


[CreateAssetMenu(fileName = "DialogueCharacter", menuName = "DialogueSystem/Character", order = 1)]
public class DialogueCharacter : ScriptableObject {
    public string Name;
    public List<Sprite> Sprite;
}
