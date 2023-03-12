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
        if(currentRoutine == null && Vector3.Distance(transform.position, Movement.current.transform.position) <= 7)
        {
            StopAllCoroutines();
            currentRoutine = StartCoroutine(Attack());
        }

        IEnumerator Attack()
        {
            CurrentWalkState = EntityWalkState.Standing;
            _lines.enabled = true;

            _lines.SetPosition(1, transform.position);
            
            for(int i=0;i<37;i++){
                yield return new WaitForFixedUpdate();
                if(Vector3.Distance(transform.position, Movement.current.transform.position) > AttackRange)
                {
                    CurrentWalkState = EntityWalkState.ToPlayer;
                    _lines.enabled = false;
                    
                    currentRoutine = null;

                    yield break;
                }
                
                _lines.SetPosition(0, transform.position);
                _lines.SetPosition(1, Vector3.MoveTowards(_lines.GetPosition(1), Movement.current.transform.position, 0.7f));    
            }
            yield return new WaitForSecondsRealtime(0.09f);
            var playerDetect = Physics2D.Raycast(
                _lines.GetPosition(0), 
                 _lines.GetPosition(1) - _lines.GetPosition(0), 
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