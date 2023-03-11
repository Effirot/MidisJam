using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Trigger : MonoBehaviour
{
    public UnityEvent<Collider2D> OnEnter = new();    
    public UnityEvent<Collider2D> OnExit = new();  


    private void OnValidate() {
        foreach (var a in GetComponents<Collider2D>()) {
            a.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        OnEnter.Invoke(other);   
    }

    private void OnTriggerExit2D(Collider2D other) {
        OnExit.Invoke(other);   
    }

    public void DestroySelf() => GameObject.Destroy(this.gameObject); 
}
