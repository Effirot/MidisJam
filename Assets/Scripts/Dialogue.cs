using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using TMPro;

public class Dialogue : MonoBehaviour
{
    public DialogueData data;
    public int CurrentStep = 0;

    [SerializeField] Image LDialogueCharacter;
    [SerializeField] Image RDialogueCharacter;
    [SerializeField] TextMeshProUGUI Text; 
    
    [SerializeField] UnityEvent OnDialogueEnd;

    public void NextStep() {
        if(CurrentStep >= data.Length - 1) {
            OnDialogueEnd.Invoke();
            return;
        }
        
        CurrentStep++;


        LDialogueCharacter.sprite = data[CurrentStep].LCharacter.Sprite;
        RDialogueCharacter.sprite = data[CurrentStep].RCharacter.Sprite;
        
        Text.text = data[CurrentStep].text;

        switch(data[CurrentStep].Speaker)
        {
            case DialogueData.SpeakerLR.Left: LDialogueCharacter.color = Color.gray; RDialogueCharacter.color = Color.white; break;
            case DialogueData.SpeakerLR.Right: LDialogueCharacter.color = Color.white; RDialogueCharacter.color = Color.gray; break;
            case DialogueData.SpeakerLR.Nobody: LDialogueCharacter.color = Color.gray; RDialogueCharacter.color = Color.gray; break;
        }

        
    }
    public void NextStep(int StepIndex) {
        if(CurrentStep >= data.Length) {
            OnDialogueEnd.Invoke();
            return;
        }

        LDialogueCharacter.sprite = data[StepIndex].LCharacter.Sprite;
        RDialogueCharacter.sprite = data[StepIndex].RCharacter.Sprite;
        
        Text.text = data[StepIndex].text;

        switch(data[StepIndex].Speaker)
        {
            case DialogueData.SpeakerLR.Left: LDialogueCharacter.color = Color.gray; RDialogueCharacter.color = Color.white; break;
            case DialogueData.SpeakerLR.Right: LDialogueCharacter.color = Color.white; RDialogueCharacter.color = Color.gray; break;
            case DialogueData.SpeakerLR.Nobody: LDialogueCharacter.color = Color.gray; RDialogueCharacter.color = Color.gray; break;
        }
    }

    private void OnEnable() => NextStep(0);
    

}




