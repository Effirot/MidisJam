using UnityEngine;

using System.Collections.Generic;
using System.Collections;
using System;

[RequireComponent(typeof(LineRenderer))]
public class EnemyArmored : Entity{
    LineRenderer _lines;
    Coroutine currentRoutine = null;

    public float AttackRange = 11;


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
        if(currentRoutine == null && Vector3.Distance(transform.position, Movement.current.transform.position) <= AttackRange - 4)
        {
            StopAllCoroutines();
            currentRoutine = StartCoroutine(Attack());
        }

        IEnumerator Attack()
        {
            CurrentWalkState = EntityWalkState.Standing;
            _lines.enabled = true;
            _lines.positionCount = 0;

            
            for(int i=0;i<120;i++){
                yield return new WaitForFixedUpdate();
                if(Vector3.Distance(transform.position, Movement.current.transform.position) > AttackRange)
                    break;
                
                if(i % 4 == 0){
                    _lines.positionCount += 2;
                    _lines.SetPosition(_lines.positionCount - 2, transform.position);
                    _lines.SetPosition(_lines.positionCount - 1, Vector3.MoveTowards(_lines.GetPosition(1), Movement.current.transform.position, 0.7f));
                }    
            }
            yield return new WaitForSecondsRealtime(0.15f);


            for(int i = (_lines.positionCount / 2) - 1; i >= 0; i--){
                yield return new WaitForSecondsRealtime(0.04f);
                
                var playerDetect = Physics2D.Raycast(
                    _lines.GetPosition(i), 
                    _lines.GetPosition(i-1) - _lines.GetPosition(i), 
                    Vector3.Distance(_lines.GetPosition(i), _lines.GetPosition(i-1)),
                    LayerMask.GetMask("Player"));


                if(playerDetect)
                if(playerDetect.collider.TryGetComponent<Movement>(out var component))
                {
                    component.Death();
                }

                _lines.positionCount -= 2;
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