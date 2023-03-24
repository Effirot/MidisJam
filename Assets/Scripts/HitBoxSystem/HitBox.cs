using UnityEngine;

using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[Flags]
public enum InputTargets{    
    MoveLeft, MoveRight,
    DownPressed, UpPressed,

    Run, Air, 

    Inverted
}

public static class HitBoxExtends{
    public static Vector2 position2(this Transform a) => a.position;
}

public interface IDamageable{
    Rigidbody2D rb { get; }
    Vector3 position { get; set; }

    void DamageReaction(Damage type);
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
    [SerializeField] List<AttackGroup> groups = new() { 
        new("Light"), 
        new("Ground SLight"), 
        new("Ground WLight") };

    public void DrawEditorGUI(){
        foreach(var info in groups)
            info?.DrawEditorGUI();
    }
}

[Serializable]
internal class AttackGroup : List<Attack>{
    public string Name;

    public AttackGroup() { }
    public AttackGroup(string name) {
        Name = name;
    }

    public void DrawEditorGUI(){
        foreach(var obj in this)
            obj?.DrawEditorGUI();
    }
}

[Serializable]
internal class Attack : List<HitBox>{
    [NonSerialized]public AttackController Origin;

    public string Name;

    public Transform transform => Origin.transform;

    public bool Invoke(out IDamageable[] hits){  
        List<IDamageable> damaged = new();
        bool result = false;

        foreach(var info in this){
            result = info.Invoke(Vector3.zero, out var damages);
            damaged.AddRange(damages);
        }

        hits = damaged.ToArray();
        return result;
    }
    public void DrawEditorGUI(){

        foreach(var collider in this)
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


#region --- --- HitBox 
[Serializable]
internal abstract class HitBox{
    internal protected Attack origin;
    public LayerMask layer;

    protected Transform transform => origin.transform;


    public abstract bool Invoke(Vector2 InvokePosition, out IDamageable[] targets); 
    public abstract void OnEditorGUI(Vector2 InvokePosition);
}

internal class BoxAllHitBox : HitBox
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
#endregion