using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using TMPro;

public class Dialogue : MonoBehaviour
{
    [field: SerializeField]public DialogueData data { get; set; }
    public int CurrentStep = 0;

    [SerializeField] Image LDialogueCharacter;
    [SerializeField] Image RDialogueCharacter;
    [SerializeField] TextMeshProUGUI Text; 
    
    [SerializeField] UnityEvent OnDialogueEnd;

    public void NextStep() {
        if(CurrentStep >= data.Length - 1) {
            OnDialogueEnd.Invoke();
            Time.timeScale = 1;
            return;
        }
        
        CurrentStep++;


        LDialogueCharacter.sprite = data[CurrentStep].LCharacter.Sprite[data[CurrentStep].LSpriteIndex];
        RDialogueCharacter.sprite = data[CurrentStep].RCharacter.Sprite[data[CurrentStep].RSpriteIndex];
        
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
            Time.timeScale = 1;

            return;
        }

        LDialogueCharacter.sprite = data[StepIndex].LCharacter.Sprite[data[StepIndex].LSpriteIndex];
        RDialogueCharacter.sprite = data[StepIndex].RCharacter.Sprite[data[StepIndex].RSpriteIndex];
        
        Text.text = data[StepIndex].text;

        switch(data[StepIndex].Speaker)
        {
            case DialogueData.SpeakerLR.Left: LDialogueCharacter.color = Color.gray; RDialogueCharacter.color = Color.white; break;
            case DialogueData.SpeakerLR.Right: LDialogueCharacter.color = Color.white; RDialogueCharacter.color = Color.gray; break;
            case DialogueData.SpeakerLR.Nobody: LDialogueCharacter.color = Color.gray; RDialogueCharacter.color = Color.gray; break;
        }
    }

    public void resetCurrentStep()
    {
        CurrentStep = 0;
    }

    private void OnEnable() { 
        NextStep(0);
    
        Time.timeScale = 0;
    }
    

}




