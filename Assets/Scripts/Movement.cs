using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public bool Controllable = true;

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


    private void Start() {
        m_rigidbody = GetComponent<Rigidbody2D>();    
    }
    private void FixedUpdate() {
        if(!Controllable) return;

        m_rigidbody.AddForce(new Vector2(targetVector.x, 0) * speed);
    }

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

    public void Death(){
        Controllable = false;
    }
}
