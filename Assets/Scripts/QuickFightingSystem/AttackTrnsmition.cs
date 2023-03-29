using UnityEngine;
using UnityEngine.Events;

using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public static class HitBoxExtends{
    public static Vector2 position2(this Transform a) => a.position;
}

public interface IDamageable{
    
    Rigidbody2D rb { get; }
    Vector3 position { get; set; }

    void DamageReaction(Damage type);
}

[Serializable]
public struct Damage{
    public float Value;

    public Vector2 Repulsion;
    public DamageType Type;

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

#region --- --- HitBox 
[Serializable]
internal abstract class HitBox{
    internal protected Attack origin;

    public LayerMask layer => origin.layer;
    protected Transform transform => origin.transform;


    public abstract bool Invoke(Vector2 InvokePosition, out IDamageable[] targets); 
    public abstract void OnEditorGUI(Vector2 InvokePosition);
}

[Serializable]
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
            if(hit.collider.gameObject.TryGetComponent<IDamageable>(out var component))
                dam.Add(component);
        }
        
        targets = dam.ToArray();
        return hits.Any();
    }

    public override void OnEditorGUI(Vector2 InvokePosition)
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position2() + InvokePosition + position, Quaternion.Euler(new(0,0,angle)), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
    }
}
[Serializable]
internal class CircleAllHitBox : HitBox
{
    public Vector2 position;
    public float radius;

    public override bool Invoke(Vector2 InvokePosition, out IDamageable[] targets)
    {
        var dam = new List<IDamageable>();
        var hits = Physics2D.CircleCastAll(transform.position2() + InvokePosition + position, radius, InvokePosition + position, Mathf.Infinity, layer, 0, 1);

        foreach(var hit in hits){
            if(hit.collider.gameObject.TryGetComponent<IDamageable>(out var component))
                dam.Add(component);
        }
        
        targets = dam.ToArray();
        return hits.Any();
    }

    public override void OnEditorGUI(Vector2 InvokePosition)
    {
        Gizmos.DrawWireSphere(position, radius);
    }
}

#endregion