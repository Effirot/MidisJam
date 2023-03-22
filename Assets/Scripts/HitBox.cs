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
    
    public Transform transform { get; set; }
    [SerializeField]List<AttackInfo> allHitBoxes = new List<AttackInfo>();

    public void AttackGizmos(){
        foreach(AttackInfo info in allHitBoxes)
            info?.OnEditorGUI();
    }
}

[Serializable]
public class AttackInfo{
    [SerializeReference, SubclassSelector]public HitBox[] Collider;
    [NonSerialized]public AttackController Origin;

    public Transform transform => Origin.transform;


    public void Invoke(){ }
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
[CustomPropertyDrawer(typeof(AttackController), true)]
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



    





