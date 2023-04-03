using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XNode;

[CreateAssetMenu]
[RequireNode(
    typeof(AttackStart),
    typeof(AttackEnd))]
public class AttackNodeSystem : NodeGraph
{
}

[System.Serializable]
public sealed class AttackTransit {}

public sealed class AttackStart : Node{
    public string Name;

    [Output][SerializeField] AttackTransit Enter;
}
public sealed class AttackEnd : Node{
    [Input][SerializeField] AttackTransit Exit;
}


public class Vector2Node : Node{
    [Input]public float x;
    [Input]public float y;
    [Output] [SerializeField] Vector2 vector2;
}



public abstract class AttackActionNode : Node{

    [Input] public AttackTransit entry;
    [Output] public AttackTransit exit;

    public abstract void OnGUI();
    public abstract bool Invoke(out List<IDamageable> hits);
}

public class CircleHitBox : AttackActionNode
{
    [Input] public Vector2 Position;
    [Input] public float Radius;


    public override bool Invoke(out List<IDamageable> hits)
    {
        throw new System.NotImplementedException();
    }

    public override void OnGUI()
    {
        throw new System.NotImplementedException();
    }
}
