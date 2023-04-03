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
        var commands = Regex.Split(command, @"\{*(>*.*?>*)\}*[>,;]");

        foreach(var c in commands){
            Debug.LogWarning($"Completing {c}");

            CompleteAttack(c);
        }

        void CompleteAttack(string Name){
            var atk = groups.Find(a=>a.Name == Name);
            
            if(atk.Invoke(out var hits))
                foreach(var item in hits)
                    item.DamageReaction(atk.damage);
        }


        yield break;
    }

    public void Draw() 
    {
        foreach(var info in groups)
        {
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

    public List<HitBox> HitBoxes = new() { new BoxAllHitBox() };
    public UnityEvent OnAttackStarted = new();
    public UnityEvent<IDamageable> OnHit = new();
    
    [NonSerialized]public QuickAttackController Origin;
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

[CustomEditor(typeof(QuickAttackController), true)]
public class QuickAttackController_Editor : Editor{
    
    QuickAttackController t;
    
    Attack NowSerializing;
    string InvokeField;

    private void OnEnable() {
        t = (QuickAttackController)target;
    }

    public override void OnInspectorGUI() { 
        

        if(Application.isPlaying){
            InvokeField = GUILayout.TextArea(InvokeField);
            
            if(GUILayout.Button(InvokeField))
                t.Complete(InvokeField);
        }

        if(GUILayout.Button("Add")){
            t.groups.Add(new() { Name = "Attack" });   
        }
        

        List<Attack> attackList = new();
        EditorGUI.BeginChangeCheck();
        for(int i = 0; i < t.groups.Count; i++){
            var atk = t.groups[i];

            EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                    atk.Name = EditorGUILayout.TextField(atk.Name ?? string.Empty);
                EditorGUILayout.EndHorizontal();

                if(GUILayout.Button("Edit"))
                    if(NowSerializing == atk) NowSerializing = null;
                    else NowSerializing = atk;
                if(GUILayout.Button("Delete"))
                    t.groups.Remove(atk);

            EditorGUILayout.EndHorizontal();
            
            if(atk == NowSerializing){
                GUILayout.Space(50);
            }
        }
        attackList = t.groups;

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Changed Area Of Effect");
            t.groups = attackList;
        }


    }

    private void OnDrawGizmosSelected() {
        NowSerializing?.DrawEditorGUI();
    }
}

#endif