using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.IO;

#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;
#endif

[Flags]
public enum InputTargets{    
    MoveLeft, MoveRight,
    DownPressed, UpPressed,

    Run, Air, 

    Inverted
}


public readonly struct Damage{
    public float Value { get; }

    public Vector2 Repulsion { get; }
    public DamageType Type { get; }

    public Damage(float value, DamageType type, Vector2 repulsion){
        Value = value;
        Type = type;
        Repulsion = repulsion;
    }
    public Damage(float value, DamageType type = DamageType.Default){
        Value = value;
        Type = type;
        Repulsion = Vector2.zero;
    }
    public Damage(float value, Vector2 repulsion, DamageType type = DamageType.Default){
        Value = value;
        Type = type;
        Repulsion = repulsion;
    }
}
public enum DamageType{
    Default = 0,
    
    Penetrating = 1,
    Finisher = 2,
    PenetratingFinisher = 3,
    
    Distant = 4,
}


[Serializable]
public class AttackController{
    
    public Transform transform { get; set; }
    [SerializeField]List<AttackInfo> allHitBoxes = new List<AttackInfo>() { new(), new(), new(), new(), new(), new(), new(), new() };


    public void AttackGizmos(){
        foreach(AttackInfo info in allHitBoxes)
            info?.OnEditorGUI();
    }
}

[Serializable]
public sealed class AttackInfo{
    [SerializeReference, SubclassSelector]public HitBox[] Collider;
    [NonSerialized]public AttackController Origin;

    public string Name;

    public Transform transform => Origin.transform;


    public bool Invoke(out IDamageable[] hits){  
        bool result = false;
        List<IDamageable> damaged = new();

        foreach(var info in Collider){
            result = info.Invoke(Vector3.zero, out var damages);
            damaged.AddRange(damages);
        }

        hits = damaged.ToArray();
        return result;
    }
    public void OnEditorGUI(){

        foreach(var collider in Collider)
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
[CustomPropertyDrawer(typeof(AttackController))]
public class AttackController_PropertyDrawer : PropertyDrawer{

    int NowEditing = -1;
    int LinesCount = 0;

    float SpaceSize => EditorGUIUtility.standardVerticalSpacing;
    float LineSize => EditorGUIUtility.singleLineHeight;

    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (LineSize + SpaceSize) * LinesCount - 1;
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty attackArray = property.FindPropertyRelative("allHitBoxes");

        var attackArr = property.FindPropertyRelative("allHitBoxes"); 


        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        {
            int lines = 0;
            property.isExpanded = true; 

            var tableRect = new Rect(position);
            tableRect.height = LineSize;
            tableRect.x -= tableRect.width / 2;
            tableRect.width *= 1.5f;
            lines += attackArr.arraySize;

            for(int i = 0; i < attackArr.arraySize; i++){

                tableRect.y += LineSize + SpaceSize;


                #region Edit button
                    var rectEditBtn = new Rect(tableRect);
                    rectEditBtn.width = 45;
                    
                    if(GUI.Button(rectEditBtn, "Edit"))
                        if(NowEditing != i)
                            NowEditing = i;
                        else NowEditing = -1;
                #endregion
                #region Name field
                    var rectLabel = new Rect(tableRect);
                    rectLabel.width = tableRect.width - rectEditBtn.width - SpaceSize - LineSize;
                    rectLabel.x += rectEditBtn.width + SpaceSize;

                    var name = attackArr.GetArrayElementAtIndex(i).FindPropertyRelative("Name");
                    name.stringValue = EditorGUI.TextField(rectLabel, name.stringValue);
                #endregion
                #region Delete button
                    var rectDeleteBtn = new Rect(rectLabel);
                    rectDeleteBtn.width = LineSize;
                    rectDeleteBtn.x += SpaceSize + rectLabel.width;

                    if(GUI.Button(rectDeleteBtn, "X")){
                        attackArr.DeleteArrayElementAtIndex(i);
                        NowEditing = -1;
                    }
                #endregion

                if(i == NowEditing)
                {
                    tableRect.y += (LineSize + SpaceSize) * 3;
                    lines += 3;
                }
            
            }


            LinesCount = lines;
        }
        EditorGUI.EndProperty();

    }
    

}

[CustomPropertyDrawer(typeof(Damage))]
public class Damage_PropertyDrawer : PropertyDrawer{

    float SpaceSize => EditorGUIUtility.standardVerticalSpacing;
    float LineSize => EditorGUIUtility.singleLineHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
        return (SpaceSize + LineSize) * 2;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var value = property.FindPropertyRelative("Value");
        var type = property.FindPropertyRelative("Type");
        var repulsion = property.FindPropertyRelative("Repulsion");

        EditorGUI.BeginProperty(position, label, property);

        var damageRect = new Rect(position);
        damageRect.height = SpaceSize;
        damageRect.width /= 2;

        value.floatValue = EditorGUI.FloatField(damageRect, value.floatValue);


        damageRect.height += damageRect.width;

        //value.enumValueFlag = EditorGUI.EnumPopup(damageRect, value.enumValueFlag);

        var repulsionRect = new Rect(position);
        repulsionRect.height /= 2;
        repulsionRect.x += repulsionRect.height;
        

        EditorGUI.EndProperty();
    }
}
#endif


public interface IDamageable{
    Rigidbody2D rb { get; }
    Vector3 position { get; set; }

    void DamageReaction(Damage type);
}

public static class HitBoxExtends{
    public static Vector2 position2(this Transform a) => a.position;
}

[Serializable]
public abstract class HitBox{
    internal protected AttackInfo origin;
    public LayerMask layer;

    protected Transform transform => origin.transform;


    public abstract bool Invoke(Vector2 InvokePosition, out IDamageable[] targets); 
    public abstract void OnEditorGUI(Vector2 InvokePosition);
}

public class BoxAllHitBox : HitBox
{
    public Vector2 position;
    public Vector2 size;
    public float angle;

    public override bool Invoke(Vector2 InvokePosition, out IDamageable[] targets)
    {
        var dam = new List<IDamageable>();
        var hits = Physics2D.BoxCastAll(transform.position2() + InvokePosition + position, size, angle, InvokePosition + position, Mathf.Infinity, layer, 0, 1);

        foreach(var hit in hits){
            
        }

        
        targets = dam.ToArray();
        return hits.Any();
    }

    public override void OnEditorGUI(Vector2 InvokePosition)
    {

    }
}



    





