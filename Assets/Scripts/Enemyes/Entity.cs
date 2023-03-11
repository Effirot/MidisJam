using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour {
    public int HP { get; private set; }
    public UnityEvent OnDeadEvent = new();

    [SerializeField] public float speed;

    protected EntityWalkState CurrentWalkState;
    protected bool IsPlayerFounded = false;

    public virtual void Hit(int damage){
        HP -= damage;

        if(HP <= 0)
            OnDeadEvent.Invoke();
    }    

    public virtual bool WillDead(int damage) => HP <= 0;

    public void Destroy() {
        GameObject.Destroy(this.gameObject);
    }


    
    private void FixedUpdate() {
        
    }

    public virtual void Patrolling(){

    }

    protected IEnumerable RoutineAction(float timer, Action action){
        yield return new WaitForSecondsRealtime(timer);
        action.Invoke();
    }
}

public enum EntityWalkState{
    Standing,
    
    ToLeft,
    ToRight,
    
    ToPlayer,
}