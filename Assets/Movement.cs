using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    Rigidbody2D m_rigidbody;

    private void Start() {
        m_rigidbody = GetComponent<Rigidbody2D>();    
    }
    private void FixedUpdate() {
        
    }
}
