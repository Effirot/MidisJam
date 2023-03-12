using UnityEngine;

using System.Collections.Generic;
using System.Collections;
using System;

[RequireComponent(typeof(LineRenderer))]
public class EnemyDefault : Entity{
    LineRenderer _lines;
    Coroutine currentRoutine = null;

    public float AttackRange = 8;

    protected override void Start()
    {
        base.Start();

        _lines = GetComponent<LineRenderer>();
    }

    protected override void FixedUpdate(){
        CastAttack();
        base.FixedUpdate();
    }

    public void CastAttack()
    {
        if( currentRoutine == null && 
            Vector3.Distance(transform.position, Movement.current.transform.position) <= AttackRange && 
            Physics2D.Raycast(transform.position, Movement.current.transform.position - transform.position, AttackRange, LayerMask.GetMask("Player", "Ground")).collider.gameObject == Movement.current.gameObject)
        {
            StopAllCoroutines();
            currentRoutine = StartCoroutine(Attack());
        }

        IEnumerator Attack()
        {
            CurrentWalkState = EntityWalkState.Standing;
            _lines.enabled = true;

            _lines.SetPosition(1, Vector3.zero);
            
            for(int i=0;i<37;i++){
                yield return new WaitForFixedUpdate();
                if(Vector3.Distance(transform.position, Movement.current.transform.position) > AttackRange ||
                    Physics2D.Raycast(transform.position, Movement.current.transform.position - transform.position, AttackRange, LayerMask.GetMask("Player", "Ground")).collider.gameObject != Movement.current.gameObject)
                {
                    CurrentWalkState = EntityWalkState.ToPlayer;
                    _lines.enabled = false;
                    
                    currentRoutine = null;

                    yield break;
                }
                
                _lines.SetPosition(0, Vector3.zero);
                _lines.SetPosition(1, Vector3.MoveTowards(_lines.GetPosition(1), Movement.current.transform.position - transform.position, 0.4f));    
            }
            yield return new WaitForSecondsRealtime(0.9f);
            var playerDetect = Physics2D.Raycast(
                transform.position, 
                _lines.GetPosition(1), 
                Vector3.Distance(_lines.GetPosition(0), _lines.GetPosition(1)),
                LayerMask.GetMask("Player"));


            if(playerDetect)
            if(playerDetect.collider.TryGetComponent<Movement>(out var component))
            {
                component.Death();
                

            }

            
            yield return new WaitForSecondsRealtime(0.3f);
            
            _lines.enabled = false;


            yield return new WaitForSecondsRealtime(0.4f);
            CurrentWalkState = EntityWalkState.ToPlayer;
            yield return new WaitForSecondsRealtime(1.2f);

            currentRoutine = null;
        }
    }        
}