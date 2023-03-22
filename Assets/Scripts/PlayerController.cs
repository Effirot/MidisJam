using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using static UnityEngine.InputSystem.InputAction;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController current;

    [field: SerializeField] public bool Controllable { get; set; } = true;

    [Space][Header("Weapon")]
    [SerializeField] public int CooldownFrames = 0;
    [SerializeField]public AttackController CurrentWeapon;

    [Space][Header("Movement")]
    [SerializeField] private float Speed;
    [SerializeField] private float RunSpeed;
    [SerializeField] private float StartRunTimer;

    [Space][Header("Jump")]
    [SerializeField][Range(0, 100)] private float JumpForce;
    [SerializeField][Range(0, 100)] public float SecondJumpForce;
    [SerializeField][Range(0, 10)] private int SecondJumpsCount;
    
    [SerializeField] private Collider2D groundCollider;

    [Space][Header("Events")]
    [SerializeField] private UnityEvent OnStartMoving = new();
    [SerializeField] private UnityEvent OnStartRunning = new();
    [SerializeField] private UnityEvent OnStopMoving = new();

    [SerializeField] private UnityEvent OnAttack = new();
    [SerializeField] private UnityEvent OnAttackReset = new();

    [SerializeField] private UnityEvent OnStart = new();
    [SerializeField] private UnityEvent OnDeath = new();

    private bool _isGrounded => groundCollider?.IsTouchingLayers(LayerMask.GetMask("Ground", "TransparentGround")) ?? false;
    
    int _lostJumps = 0;
    public Rigidbody2D _rigidbody;
    public Collider2D _collider;


    Vector2 targetVector = Vector2.zero;

    public void Move(CallbackContext call){
        if(!Controllable) return;

        targetVector = call.ReadValue<Vector2>();
    }
    public void Jump(CallbackContext call){
        if(!Controllable) return;

        if(call.started){
            if(_isGrounded){
                _rigidbody.AddForce(new(targetVector.x / 2, JumpForce * 100));
                _lostJumps = 0;
            }
            else if(SecondJumpsCount - _lostJumps > 0)
                {
                    _lostJumps++;
                    _rigidbody.velocity /= 2;
                    _rigidbody.AddForce(new(0, SecondJumpForce * 100));
                }
        }
    }
    public void SecondJump(){
        _rigidbody.AddForce(new(0, SecondJumpForce * 100));
    }

    public void SetCooldown(int frames) => CooldownFrames = frames;

    public void Death(){
        Controllable = false;
        OnDeath.Invoke();

        _collider.enabled = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.drag = 0;
        _rigidbody.angularDrag = 0;
        _rigidbody.constraints = RigidbodyConstraints2D.None;
        _rigidbody.freezeRotation = false;
        _rigidbody.AddForce((Vector3.up) * 250);
        GetComponent<CameraTarget>().target = null;
    }


    private void Start() {
        _rigidbody = GetComponent<Rigidbody2D>();   
        _collider = GetComponent<Collider2D>();

        current = this;
    }
    private void FixedUpdate() {
        if(!Controllable) return;

        _rigidbody.AddForce(new Vector2(targetVector.x, 0) * RunSpeed);

        if(CooldownFrames > 0)
            CooldownFrames--;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer == LayerMask.GetMask("Entity"))
            _rigidbody.AddForce(new(0,SecondJumpForce));
    }

    private void OnDrawGizmosSelected() {
        if(CurrentWeapon == null)return;
        CurrentWeapon.transform = transform;
        CurrentWeapon.AttackGizmos();
    }
}


// [Serializable]
// public class KatanaWeapon : AttackController{

// }