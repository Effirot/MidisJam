using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Movement : MonoBehaviour
{
    public static Movement current;

    [field: SerializeField] public bool Controllable { get; set; } = true;

    [Space][Header("Weapon")]
    [SerializeReference, SubclassSelector] public Weapon CurrentWeapon;

    [Space][Header("Movement")]
    [SerializeField] private float speed;

    [Space][Header("Jump")]
    [SerializeField][Range(0, 100)] private float jumpForce;
    [SerializeField][Range(0, 100)] public float secondJumpForce;
    [SerializeField][Range(0, 10)] private int secondJumpCount;
    [SerializeField] private Collider2D groundCollider;
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
                _rigidbody.AddForce(new(targetVector.x / 2, jumpForce * 100));
                _lostJumps = 0;
            }
            else if(secondJumpCount - _lostJumps > 0)
            {
                _lostJumps++;
                _rigidbody.AddForce(new(targetVector.x / 2, secondJumpForce * 100));
            }
        }
    }

    public void Cast(CallbackContext call) {
        if(call.started && Controllable) 
            CurrentWeapon?.InvokeCast();
    }
    public void SecondCast(CallbackContext call) {
        if(call.started && Controllable) 
            CurrentWeapon?.InvokeSecondCast();
    }

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

        CurrentWeapon.player = this;

        current = this;
    }
    private void FixedUpdate() {
        if(!Controllable) return;

        _rigidbody.AddForce(new Vector2(targetVector.x, 0) * speed);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer == LayerMask.GetMask("Entity"))
            _rigidbody.AddForce(new(0,secondJumpForce));
    }
}

[Serializable]
public abstract class Weapon
{
    public abstract int? Ammo { get; }
    
    protected Coroutine currentCast;
    protected Vector2 cursorPoint => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    public Movement player;


    protected abstract IEnumerator Cast();
    public void InvokeCast(){
        
        if(currentCast == null)
            currentCast = player.StartCoroutine(Reset());
    
        IEnumerator Reset(){
            yield return Cast();
            currentCast = null;
        }
    }

    protected abstract IEnumerator SecondCast();
    public void InvokeSecondCast(){
        
        if(currentCast == null)
            currentCast = player.StartCoroutine(Reset());
    
        IEnumerator Reset(){
            yield return SecondCast();
            currentCast = null;
        }
    }

}

[Serializable]
public class SwordWeapon : Weapon{
    public override int? Ammo => null;
    [Range(0, 100)]public float attackDistance;

    public UnityEvent<Quaternion> OnSwordSlash = new();

    protected override IEnumerator Cast()
    {
        player.Controllable = false;
        yield return new WaitForSecondsRealtime(0.1f);
        player.Controllable = true;
        
        Vector2 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;
        var hit = Physics2D.Raycast(
            player.transform.position, 
            vec, 
            attackDistance,
            LayerMask.GetMask("Entity", "Ground"));
        
        OnSwordSlash.Invoke(Quaternion.AngleAxis(Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg, Vector3.forward));

        if(hit)
        if(hit.collider.gameObject.TryGetComponent<Entity>(out var component))
        {
            if(component.WillDead(100)){
                component.Hit(100);
                player.transform.position = component.transform.position;
                player._rigidbody.velocity = Vector3.zero;
                player._rigidbody.AddForce(Vector3.up * player.secondJumpForce * 90);
                
                component.StartCoroutine(AwaitDestroy(component.gameObject));
                component._collider.enabled = false;

                component._rigidbody.drag = 0;
                component._rigidbody.angularDrag = 0;
                component._rigidbody.freezeRotation = false;
                component._rigidbody.constraints = RigidbodyConstraints2D.None;
                
                component._rigidbody.velocity = Vector3.zero;
                component._rigidbody.angularVelocity = 40;

                component._rigidbody.AddForceAtPosition((vec + Vector2.up) * 250, hit.point);
                

                yield break;
            }

            component.Hit(100);
            
        }

        yield return new WaitForSecondsRealtime(3f);

        IEnumerator AwaitDestroy(UnityEngine.Object component){
            yield return new WaitForSecondsRealtime(10);
            UnityEngine.Object.Destroy(component);
        }
    }

    protected override IEnumerator SecondCast()
    {
        yield break;
    }
}