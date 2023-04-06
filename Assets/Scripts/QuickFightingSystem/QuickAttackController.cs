using UnityEngine;
using UnityEngine.Events;

using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;


#if UNITY_EDITOR
    using UnityEditor;
#endif


public class QuickAttackController : MonoBehaviour{
    
    public AttackNodeSystem AttackSystem;

    /// <summary>
    /// w=2300>Light>w=30>{w=10>Line}x20>Line?[]:[]:[]
    /// \{*(>*.*?>*)\}*[>,;]
    /// </summary>

    public void Complete(string command){
        StartCoroutine(_Complete(command));
    }
    
    IEnumerator<YieldInstruction> _Complete(string command){
        var commands = Regex.Split(command, @"\{*(>*.*?>*)\}*[>,;]");

        foreach(var c in commands){
            Debug.LogWarning($"Completing {c}");

            CompleteAttack(c);
        }

        void CompleteAttack(string Name){

        }


        yield break;
    }

    public void Draw() 
    {

    }
}
