using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public abstract class Entity : MonoBehaviour {
    public int HP { get; private set; }
    public UnityEvent OnDeadEvent = new();

    public virtual void Hit(int damage){
        HP -= damage;

        if(HP <= 0)
            OnDeadEvent.Invoke();
    }    

    public virtual bool WillDead(int damage) => HP <= 0;

    public void Destroy() {
        GameObject.Destroy(this.gameObject);
    }

    
}