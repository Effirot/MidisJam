using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public static Movement current;

    public bool Controllable = true;

    [Space][Header("Weapon")]
    [SerializeReference, SubclassSelector] public Weapon CurrentWeapon;

    [Space][Header("Movement")]
    [SerializeField] private float speed;

    [Space][Header("Jump")]
    [SerializeField][Range(0, 100)] private float jumpForce;
    [SerializeField][Range(0, 100)] private float secondJumpForce;
    [SerializeField][Range(0, 10)] private int secondJumpCount;
    [SerializeField] private Collider2D groundCollider;

    private bool _isGrounded => groundCollider?.IsTouchingLayers(LayerMask.GetMask("Ground", "TransparentGround")) ?? false;
    
    int _lostJumps = 0;
    Rigidbody2D m_rigidbody;
    Vector2 targetVector = Vector2.zero;

    public void Move(CallbackContext call){
        if(!Controllable) return;

        targetVector = call.ReadValue<Vector2>();
    }
    public void Jump(CallbackContext call){
        if(!Controllable) return;

        if(call.started){
            if(_isGrounded){
                m_rigidbody.AddForce(new(targetVector.x / 2, jumpForce * 100));
                _lostJumps = 0;
            }
            else if(secondJumpCount - _lostJumps > 0)
            {
                _lostJumps++;
                m_rigidbody.AddForce(new(targetVector.x / 2, secondJumpForce * 100));
            }
        }
    }

    public void Cast(CallbackContext call) {
        if(call.started) 
            CurrentWeapon?.InvokeCast();
    }

    public void Death(){
        Controllable = false;
    }

    private void Start() {
        m_rigidbody = GetComponent<Rigidbody2D>();   
        CurrentWeapon.player = this;

        current = this;
    }
    private void FixedUpdate() {
        if(!Controllable) return;

        m_rigidbody.AddForce(new Vector2(targetVector.x, 0) * speed);
    }
}

[Serializable]
public abstract class Weapon
{
    public abstract int? Ammo { get; }
    
    protected Coroutine currentCast;
    protected Vector2 cursorPoint => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    public Movement player;


    public float LastCastTimer { get; set; }
    protected abstract IEnumerator Cast();
    public void InvokeCast(){
        Debug.Log("CastInvoked");
        
        if(currentCast == null)
            currentCast = player.StartCoroutine(Cast());
    }

    public float SecondCastPower = 0;
    public TimeSpan SecondLastCastTimer { get; set; }
    protected abstract IEnumerator SecondCast();
}

[Serializable]
public class SwordWeapon : Weapon{
    public override int? Ammo => null;
    [Range(0, 100)]public float attackDistance;

    public UnityEvent<Quaternion> OnSwordSlash = new();

    protected override IEnumerator Cast()
    {
        
        yield return new WaitForSecondsRealtime(0.1f);
        
        Vector2 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;
        var hit = Physics2D.Raycast(
            player.transform.position, 
            vec, 
            attackDistance,
            (1<<8));
        
        OnSwordSlash.Invoke(Quaternion.AngleAxis(Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg, Vector3.forward));

        Debug.Log(hit.transform?.name);
        if(hit)
        if(hit.collider.gameObject.TryGetComponent<Entity>(out var component)){
            if(component.WillDead(100))
                player.transform.position = component.transform.position;
            
            component.Hit(100);
        }

        LastCastTimer = Time.time;

        yield return new WaitForSecondsRealtime(0.7f);

        currentCast = null;
    }

    protected override IEnumerator SecondCast()
    {
        yield break;
    }
}