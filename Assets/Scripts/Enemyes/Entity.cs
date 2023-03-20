using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour, IDamageable {
    public int HP { get; private set; }
    public UnityEvent OnDeadEvent = new();

    [SerializeField] protected EntityWalkState CurrentWalkState;
    [SerializeField] public float speed;
    
    [Space][Header("Raycast origins")]
    [SerializeField] public Transform RRaycastOrigin;
    bool CheckR => Physics2D.Raycast(RRaycastOrigin.position, Vector3.down, 1, LayerMask.GetMask("Ground", "TransparentGround"));
    [SerializeField] public Transform LRaycastOrigin;
    bool CheckL => Physics2D.Raycast(LRaycastOrigin.position, Vector3.down, 1, LayerMask.GetMask("Ground", "TransparentGround"));

    public Rigidbody2D rb => _rigidbody;
    public Vector3 position { get => transform.position; set => transform.position = value; }

    protected bool IsPlayerFounded = false;
    public Rigidbody2D _rigidbody;
    public Collider2D _collider;

    public virtual void Hit(int damage){
        HP -= damage;

        if(HP <= 0)
            OnDeadEvent.Invoke();

        SessionManager.main.KillStreak++;
            
    }    

    public virtual bool WillDead(int damage) => HP <= 0;

    public void Destroy() {
        GameObject.Destroy(this.gameObject);
    }

    public void InvertMove(){
        switch(CurrentWalkState){
            case EntityWalkState.ToLeft: 
            CurrentWalkState = EntityWalkState.ToRight;
            return;
        
            case EntityWalkState.ToRight:
            CurrentWalkState = EntityWalkState.ToLeft;
            return;
        }
    }

    public virtual void StartAiming(){

    }

    public virtual void DefaultWalking(){
        switch(CurrentWalkState){
            case EntityWalkState.ToLeft: 
                _rigidbody.AddForce(Vector3.left * speed);
                if(!CheckL)
                {
                    StopAllCoroutines();
                    CurrentWalkState = EntityWalkState.Standing;
                    
                    StartCoroutine(RoutineAction(5, ()=>{
                        CurrentWalkState = EntityWalkState.ToRight; 
                        StartCoroutine(RoutineAction(10, InvertMove));
                    }));
                }                    
            return;
            case EntityWalkState.ToRight: 
                _rigidbody.AddForce(Vector3.right * speed);
                if(!CheckR)
                {
                    StopAllCoroutines();
                    CurrentWalkState = EntityWalkState.Standing;
                    
                    StartCoroutine(RoutineAction(5, ()=>{
                        CurrentWalkState = EntityWalkState.ToLeft; 
                        StartCoroutine(RoutineAction(10, InvertMove));
                    }));
                }          
            return;
            case EntityWalkState.ToPlayer: 
                
                Vector3 LTarget = new Vector3(Math.Clamp((PlayerController.current.transform.position - transform.position).x*10, -1, 0),0,0);
                Vector3 RTarget = new Vector3(Math.Clamp((PlayerController.current.transform.position - transform.position).x*10, 0, 1),0,0);
                
                if(CheckR)
                    _rigidbody.AddForce(RTarget * speed);
                if(CheckL)
                    _rigidbody.AddForce(LTarget * speed);

            return;
        }
    }

    protected virtual void OnDrawGizmos() {

        var rayR = new Ray(RRaycastOrigin?.position ?? transform.position, Vector3.down);
        if(CheckR)
            Debug.DrawRay(rayR.origin, rayR.direction, Color.red);
        else
            Debug.DrawRay(rayR.origin, rayR.direction, Color.green);

            
        var rayL = new Ray(LRaycastOrigin?.position ?? transform.position, Vector3.down);
        if(CheckL)
            Debug.DrawRay(rayL.origin, rayL.direction, Color.red);
        else
            Debug.DrawRay(rayL.origin, rayL.direction, Color.green);
    
    }

    protected virtual void FixedUpdate() {
        DefaultWalking();
    }

    protected virtual void Start(){
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

        CurrentWalkState = (EntityWalkState)UnityEngine.Random.Range(1, 2);
    }

    protected IEnumerator RoutineAction(float timer, Action action){
        yield return new WaitForSecondsRealtime(timer);
        action.Invoke();
        
    }

    public void DamageReaction(Damage type)
    {
        throw new NotImplementedException();
    }
}

public enum EntityWalkState{
    Standing,
    
    ToLeft = 1,
    ToRight = 2,
    
    ToPlayer,
}