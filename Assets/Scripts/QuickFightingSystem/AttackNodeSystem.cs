using UnityEngine;
using UnityEngine.Events;

using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

using XNode;

[CreateAssetMenu]
[RequireNode(typeof(AttackStart))]
public class AttackNodeSystem : NodeGraph
{
    public string Name;

    public AttackStart[] AttackStored { get {
            var result = new List<AttackStart>();

            foreach (AttackStart attack in nodes.Where(x => x is AttackStart))
                result.Add(attack);

            return result.ToArray();
        }
    }

    public void Invoke(QuickAttackController Invoker, string AttackName){
        
    }

}

[System.Serializable]
public sealed class QueueTransit {
    public QuickAttackController Origin;

    public int Index;

    public AttackStart StartNode;
}

public sealed class AttackStart : Node{
    public string Name = "Attack";

    [SerializeField] 
    [Output(ShowBackingValue.Never, ConnectionType.Override)]
    QueueTransit Enter;
}

#region Object Combine\Separate
public class CombineVector2Node : Node{
    [Input]  public float x;
    [Input]  public float y;
    [Output] public  Vector2 vector2;

    public void Combine(){
        vector2 = new(x, y);
    }
}
public class SeparateVector2Node : Node{
    [Output] public float x;
    [Output] public float y;
    [Input]  public Vector2 vector2;

    public void Separate(){
        x = vector2.x;
        y = vector2.y;
    }
}
public class CombineDamageNode : Node{
    [Output][SerializeField] public Damage damage;
}
#endregion


public class DamageTargetsInfoSplitNode : Node{
    [Input] public DamageTargetsInfo Damaged;
    
    [Output] public bool IsHit;
    [Output] public bool IsDamage;
}
public class TransmitDamageNode : Node{
    [Input] public DamageTargetsInfo DamagedInfo;
    [Input] public Damage Damage;

    public void SendDamage(){
        DamagedInfo.ForEach(a=>a.DamageReaction(Damage));
    }
}

[System.Serializable]
public sealed class DamageTargetsInfo : List<IDamageable>{
    public bool IsHit = false;
    public bool IsDamage => this.Any();

    public static explicit operator bool(DamageTargetsInfo a) => a.Any();  
}


public abstract class QueueObject : Node{

    [Input(ShowBackingValue.Never, ConnectionType.Override)] public QueueTransit entry;
    [Output(ShowBackingValue.Never, ConnectionType.Override)] public QueueTransit exit;

    public abstract void OnGUI();
}
public class CircleHitBox : QueueObject
{
    [Input] public Vector2 Position;
    [Input] public float Radius;

    [Output] public DamageTargetsInfo Damaged;

    public override void OnGUI()
    {
        throw new System.NotImplementedException();
    }
}


public class Condition : QueueObject{
    public bool Input;

    [Output(ShowBackingValue.Never, ConnectionType.Override)]
    public QueueTransit exit2;
    
    public override void OnGUI()
    { }
}