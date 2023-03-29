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

public class AttackController : MonoBehaviour{
    
    internal List<Attack> groups = new() { 
        new(), 
        new(), 
        new() 
    };


    /// <summary>
    /// w=2300>Light>w=30>{w=10>Line}x20>Line?[]:[]:[]
    /// \{*(>*.*?>*)\}*[>,;]
    /// </summary>

    public void Complete(string command){
        StartCoroutine(_Complete(command));
    }
    
    IEnumerator<YieldInstruction> _Complete(string command){
        var commands = Regex.Matches(command, @"\{*(>*.*?>*)\}*[>,;]");

        foreach(var c in commands){
            
        }

        yield break;
    }

    public void Draw() {
        foreach(var info in groups){
            if(info == null) continue;

            info.Origin = this;
            info.DrawEditorGUI();
        }
    }
}

[Serializable]
internal class Attack{

    public string Name;
    public LayerMask layer;
    public Damage damage;

    [SerializeReference, SubclassSelector]public List<HitBox> HitBoxes = new() { new BoxAllHitBox() };
    public UnityEvent OnAttackStarted = new();
    public UnityEvent<IDamageable> OnHit = new();
    
    [NonSerialized]public AttackController Origin;
    public Transform transform => Origin.transform;


    public bool Invoke(out IDamageable[] hits){  
        List<IDamageable> damaged = new();
        bool result = false;

        foreach(var info in HitBoxes){
            result = info.Invoke(Vector3.zero, out var damages);
            
            foreach(var obj in damages)
                if(!damaged.Contains(obj))
                    damaged.Add(obj);
        }

        hits = damaged.ToArray();
        return result;
    }
    public void DrawEditorGUI(){

        if(HitBoxes.Count > 0)
        foreach(var collider in HitBoxes)
        {    
            if(collider == null) continue;

            collider.origin = this;

            Gizmos.matrix = Matrix4x4.zero;
            if(collider.Invoke(Vector2.zero, out var comps))
            {
                if(comps.Any())
                    Gizmos.color = Color.red;
                else Gizmos.color = Color.yellow;
            }
            else Gizmos.color = Color.green;


            collider.OnEditorGUI(Vector2.zero);
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(AttackController), true)]
public class AttackController_Editor : Editor{
    
    AttackController Target;
    
    Attack NowSerializing;
    string InvokeField;

    private void OnEnable() {
        Target = (AttackController)target;
    }

    public override void OnInspectorGUI() { 
        if(Application.isPlaying){
            InvokeField = GUILayout.TextArea(InvokeField);
            
            if(GUILayout.Button(InvokeField))
                Target.Complete(InvokeField);
        }

        if(GUILayout.Button("Add")){
            Target.groups.Add(new() { Name = "Attack" });   
        }


        foreach(var atk in Target.groups){
            EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                    atk.Name = EditorGUILayout.TextArea(atk.Name ?? string.Empty);
                EditorGUILayout.EndHorizontal();

                if(GUILayout.Button("Edit"))
                    if(NowSerializing == atk) NowSerializing = null;
                    else NowSerializing = atk;
                                   
                

            EditorGUILayout.EndHorizontal();
            
            if(atk == NowSerializing){
                GUILayout.Space(50);
            }
        }
    }

    private void OnDrawGizmosSelected() {
        NowSerializing?.DrawEditorGUI();
    }
}

#endif