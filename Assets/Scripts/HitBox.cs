using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.IO;

#if UNITY_EDITOR 
    using UnityEditor;
#endif


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
public class Attack{
    
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

// --- --- --- Figure
[Serializable]
public class CircleHitBox : HitBox
{
    public Vector2 position;
    public float radius;

    public override IDamageable[] Invoke(Vector2 InvokePosition)
    {
        var hitBoxes = new List<IDamageable>();
        
        foreach(var hit in Physics2D.CircleCastAll(origin.position2() - (InvokePosition + position), radius, position, float.PositiveInfinity, layer))
            if(hit.collider.TryGetComponent<IDamageable>(out var component))
                hitBoxes.Add(component);

        return hitBoxes.ToArray();
    }

    public override void OnEditorGUI(Vector2 InvokePosition)
    {
        Gizmos.DrawWireSphere(origin.position2() - (InvokePosition + position), radius);
    }
}
[Serializable]
public class CubeHitBox : HitBox
{
    public Vector2 position;
    public Vector2 scale;
    public float angle;

    public override IDamageable[] Invoke(Vector2 InvokePosition)
    {
        var hitBoxes = new List<IDamageable>();
        
        foreach(var hit in Physics2D.BoxCastAll(InvokePosition + (position * origin.right), scale, angle, position, Mathf.Infinity, layer))
            if(hit.collider.gameObject.TryGetComponent<IDamageable>(out var component))
                hitBoxes.Add(component);

        return hitBoxes.ToArray();
    }

    public override void OnEditorGUI(Vector2 InvokePosition)
    {
        Gizmos.DrawWireCube(InvokePosition + (position * origin.right), scale);
    }
}

// --- --- --- Line
[Serializable]
public class LineHitBox : HitBox
{
    public Vector2 position;
    public Vector2 position2;

    public override IDamageable[] Invoke(Vector2 InvokePosition)
    {
        var hitBoxes = new List<IDamageable>();

        var hit = Physics2D.Linecast(InvokePosition + (position * origin.right), position2, layer);
        
        if(hit)
        if(hit.collider.gameObject.TryGetComponent<IDamageable>(out var component))
            hitBoxes.Add(component);

        return hitBoxes.ToArray();
    }

    public override void OnEditorGUI(Vector2 InvokePosition)
    {
        Gizmos.DrawLine(InvokePosition + (position * origin.right), InvokePosition + (position2 * origin.right));
    }
}
[Serializable]
public class LineAllHitBox : HitBox
{
    public Vector2 position;
    public Vector2 position2;

    public override IDamageable[] Invoke(Vector2 InvokePosition)
    {
        var hitBoxes = new List<IDamageable>();
        
        foreach(var hit in Physics2D.LinecastAll(InvokePosition + (position * origin.right), position2, layer))
            if(hit.collider.gameObject.TryGetComponent<IDamageable>(out var component))
                hitBoxes.Add(component);

        return hitBoxes.ToArray();
    }

    public override void OnEditorGUI(Vector2 InvokePosition)
    {
        Gizmos.DrawLine(InvokePosition + (position * origin.right), InvokePosition + (position2 * origin.right));
    }
}

// --- --- --- RaycastHit 
[Serializable]
public class RaycastHitBox : HitBox
{
    public Vector2 position;
    public Vector2 Vector;
    public float MaxDistance;

    public override IDamageable[] Invoke(Vector2 InvokePosition)
    {
        var hitBoxes = new List<IDamageable>();
        var hit = Physics2D.Raycast(InvokePosition + (position * origin.right), Vector, MaxDistance, layer);
        
        if(hit)
        if(hit.collider.gameObject.TryGetComponent<IDamageable>(out var component))
            hitBoxes.Add(component);

        return hitBoxes.ToArray();
    }

    public override void OnEditorGUI(Vector2 InvokePosition)
    {
        Gizmos.DrawLine(InvokePosition + (position * origin.right), InvokePosition + (Vector * origin.right));
    }
}
[Serializable]
public class RaycastAllHitBox : HitBox
{
    public Vector2 position;
    public Vector2 Vector;
    public float MaxDistance;

    public override IDamageable[] Invoke(Vector2 InvokePosition)
    {
        var hitBoxes = new List<IDamageable>();
        
        foreach(var hit in Physics2D.RaycastAll(InvokePosition + (position * origin.right), Vector, MaxDistance, layer))
            if(hit.collider.gameObject.TryGetComponent<IDamageable>(out var component))
                hitBoxes.Add(component);

        return hitBoxes.ToArray();
    }

    public override void OnEditorGUI(Vector2 InvokePosition)
    {
        Gizmos.DrawLine(InvokePosition + (position * origin.right), InvokePosition + (Vector * origin.right));
    }
}

// --- --- --- Target placer
// [Serializable]
// public class LineHitPositionerHitBox : HitBox
// {
//     public Vector2 position;
//     public Vector2 position2;
    
//     [SerializeReference, SubclassSelector]public HitBox subHitBox;


//     public override IDamageable[] Invoke(Vector2 InvokePosition)
//     {
//         var hitBoxes = new List<IDamageable>();

//         var hit = Physics2D.Linecast(InvokePosition + (position * origin.right), InvokePosition + (position2 * origin.right), layer);
        
//         if(hit){
//             if(hit.collider.gameObject.TryGetComponent<IDamageable>(out var component))
//                 hitBoxes.Add(component);
        
//             if(subHitBox != null){
//                 subHitBox.origin = origin;
//                 subHitBox.layer = layer;

//                 hitBoxes.AddRange(subHitBox.Invoke(hit.point));
                
//             }
//         }
//         else{
//             if(subHitBox != null){
//                 subHitBox.origin = origin;
//                 subHitBox.layer = layer;

//                 hitBoxes.AddRange(subHitBox.Invoke(position2));
//             }
//         }

//         return hitBoxes.ToArray();
//     }

//     public override void OnEditorGUI(Vector2 InvokePosition)
//     {
//         Gizmos.color = Color.cyan;
//         var hit = Physics2D.Linecast(InvokePosition + (position * origin.right), InvokePosition + (position2 * origin.right), layer);
//         if(hit)
//             Gizmos.DrawLine(InvokePosition + (position * origin.right), hit.point);
//         else
//             Gizmos.DrawLine(InvokePosition + (position * origin.right), InvokePosition + (position2 * origin.right));

//         subHitBox?.OnEditorGUI(InvokePosition - position);
//     }
// }