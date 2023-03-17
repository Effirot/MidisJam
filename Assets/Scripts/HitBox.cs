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
    
    [SerializeField]LayerMask layer;
    [SerializeReference, SubclassSelector]public HitBox[] Collider;

    public void Invoke(Vector2 origin, LayerMask layer){ }
    public void OnEditorGUI(Vector2 origin){
        foreach(var collider in Collider)
            collider?.OnEditorGUI(origin, layer);
    }
}

public interface IDamageable{
    Rigidbody2D rb { get; }
    Vector3 position { get; set; }

    void DamageReaction(Damage type);
}

[Serializable]
public abstract class HitBox{
    public Vector2 position;

    public abstract IDamageable[] Invoke(Vector2 origin, LayerMask layer); 
    public abstract void OnEditorGUI(Vector2 origin, LayerMask layer);
}


[Serializable]
public class CircleHitBox : HitBox
{
    public float radius;

    public override IDamageable[] Invoke(Vector2 origin, LayerMask layer)
    {
        var hitBoxes = new List<IDamageable>();
        
        foreach(var hit in Physics2D.CircleCastAll(origin + position, radius, position))
            if(hit.collider.gameObject.TryGetComponent<IDamageable>(out var component))
                hitBoxes.Add(component);

        return hitBoxes.ToArray();
    }

    public override void OnEditorGUI(Vector2 origin, LayerMask layer)
    {
        if(Invoke(origin, layer).Any())
            Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(origin + position, radius);
    }
}
[Serializable]
public class CubeHitBox : HitBox
{
    public Vector2 scale;
    public float angle;

    public override IDamageable[] Invoke(Vector2 origin, LayerMask layer)
    {
        var hitBoxes = new List<IDamageable>();
        
        foreach(var hit in Physics2D.BoxCastAll(origin + position, scale, angle, position))
            if(hit.collider.gameObject.TryGetComponent<IDamageable>(out var component))
                hitBoxes.Add(component);

        return hitBoxes.ToArray();
    }

    public override void OnEditorGUI(Vector2 origin, LayerMask layer)
    {
        if(Invoke(origin, layer).Any())
            Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin + position, scale);
    }
}


[Serializable]
public class LineHitBox : HitBox
{
    public Vector2 position2;

    public override IDamageable[] Invoke(Vector2 origin, LayerMask layer)
    {
        var hitBoxes = new List<IDamageable>();

        var hit = Physics2D.Linecast(origin + position, position2);
        
        if(hit)
        if(hit.collider.gameObject.TryGetComponent<IDamageable>(out var component))
            hitBoxes.Add(component);

        return hitBoxes.ToArray();
    }

    public override void OnEditorGUI(Vector2 origin, LayerMask layer)
    {
        if(Invoke(origin, layer).Any())
            Gizmos.color = Color.red;

        Gizmos.DrawLine(origin + position, origin + position2);
    }
}
[Serializable]
public class LineAllHitBox : HitBox
{
    public Vector2 position2;

    public override IDamageable[] Invoke(Vector2 origin, LayerMask layer)
    {
        var hitBoxes = new List<IDamageable>();
        
        foreach(var hit in Physics2D.LinecastAll(origin + position, position2))
            if(hit.collider.gameObject.TryGetComponent<IDamageable>(out var component))
                hitBoxes.Add(component);

        return hitBoxes.ToArray();
    }

    public override void OnEditorGUI(Vector2 origin, LayerMask layer)
    {
        if(Invoke(origin, layer).Any())
            Gizmos.color = Color.red;

        Gizmos.DrawLine(origin + position, origin + position2);
    }
}


[Serializable]
public class RaycastHitBox : HitBox
{
    public Vector2 Vector;
    public float MaxDistance;

    public override IDamageable[] Invoke(Vector2 origin, LayerMask layer)
    {
        var hitBoxes = new List<IDamageable>();
        var hit = Physics2D.Raycast(origin + position, Vector, MaxDistance, layer);
        
        if(hit)
        if(hit.collider.gameObject.TryGetComponent<IDamageable>(out var component))
            hitBoxes.Add(component);

        return hitBoxes.ToArray();
    }

    public override void OnEditorGUI(Vector2 origin, LayerMask layer)
    {
        if(Invoke(origin, layer).Any())
            Gizmos.color = Color.red;

        Gizmos.DrawLine(origin + position, origin + Vector);
    }
}
[Serializable]
public class RaycastAllHitBox : HitBox
{
    public Vector2 Vector;
    public float MaxDistance;

    public override IDamageable[] Invoke(Vector2 origin, LayerMask layer)
    {
        var hitBoxes = new List<IDamageable>();
        
        foreach(var hit in Physics2D.RaycastAll(origin + position, Vector, MaxDistance, layer))
            if(hit.collider.gameObject.TryGetComponent<IDamageable>(out var component))
                hitBoxes.Add(component);

        return hitBoxes.ToArray();
    }

    public override void OnEditorGUI(Vector2 origin, LayerMask layer)
    {
        if(Invoke(origin, layer).Any())
            Gizmos.color = Color.red;

        Gizmos.DrawLine(origin + position, origin + Vector);
    }
}
