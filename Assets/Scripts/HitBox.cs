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
    DownPressed,
    Grounded, NonGrounded,
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
    public AttackController(Transform origin) { this.origin = origin; }
    
    Transform origin { get; }
    [SerializeField]List<AttackInfo> allHitBoxes = new List<AttackInfo>();



    
    [Serializable]
    class AttackInfo{
        
        [SerializeReference, SubclassSelector]public HitBox[] Collider;

        [NonSerialized]public Transform Origin;

        public void Invoke(){ }
        public void OnEditorGUI(){

            foreach(var collider in Collider)
            {    
                if(collider == null) continue;

                collider.origin = Origin;

                if(collider.Invoke(Vector2.zero).Any())
                    Gizmos.color = Color.red;
                else Gizmos.color = Color.green;

                collider.OnEditorGUI(Vector2.zero);
            }
        }
    }

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(AttackController))]
public class AttackController_PropertyDrawer : PropertyDrawer{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty attackArray = property.FindPropertyRelative("allHitBoxes");


        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        {
            var amountRect = new Rect(position.x, position.y, 30, position.height);
            EditorGUI.LabelField(amountRect, "ass");
        }
        EditorGUI.EndProperty();
    }

}
#endif


public interface IDamageable{
    Rigidbody2D rb { get; }
    Vector3 position { get; set; }

    void DamageReaction(Damage type);
}

[Serializable]
public abstract class HitBox{
    [NonSerialized]public Transform origin;
    public LayerMask layer;


    public abstract IDamageable[] Invoke(Vector2 InvokePosition); 
    public abstract void OnEditorGUI(Vector2 InvokePosition);
}

public static class HitBoxExtends{
    public static Vector2 position2(this Transform a) => a.position;
}


    





