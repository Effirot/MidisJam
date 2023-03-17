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
        if( currentRoutine == null && 
            Vector3.Distance(transform.position, PlayerController.current.transform.position) <= AttackRange - 4 &&
            Physics2D.Raycast(transform.position, PlayerController.current.transform.position - transform.position, AttackRange - 4, LayerMask.GetMask("Player", "Ground")).collider.gameObject == PlayerController.current.gameObject)
        {
            StopAllCoroutines();
            currentRoutine = StartCoroutine(Attack());
        }

        IEnumerator Attack()
        {
            CurrentWalkState = EntityWalkState.Standing;
            _lines.enabled = true;
            _lines.positionCount = 0;

            
            for(int i=0;i<60;i++){
                yield return new WaitForFixedUpdate();
                if( Vector3.Distance(transform.position, PlayerController.current.transform.position) > AttackRange || 
                    Physics2D.Raycast(transform.position, PlayerController.current.transform.position - transform.position, AttackRange, LayerMask.GetMask("Player", "Ground")).collider.gameObject != PlayerController.current.gameObject)
                    break;
                
                if(i % 2 == 0){
                    Vector2 Vec = PlayerController.current.transform.position - transform.position;
                    Vector2 Vec_0 = Vec / Mathf.Sqrt(Mathf.Abs(Vec.x) + Mathf.Abs(Vec.y));

                    _lines.positionCount += 2;
                    _lines.SetPosition(_lines.positionCount - 2, Vector3.zero);
                    
                    var ray = Physics2D.Raycast(
                        transform.position, 
                        PlayerController.current.transform.position - transform.position, AttackRange, 
                        LayerMask.GetMask("Ground"));
                    if(ray)
                        _lines.SetPosition(_lines.positionCount - 1, ray.point);
                    else
                        _lines.SetPosition(_lines.positionCount - 1, Vec_0 * AttackRange);
                }    
            }
            yield return new WaitForSecondsRealtime(0.15f);


            for(int i = (_lines.positionCount / 2); i > 0; i--){
                yield return new WaitForSecondsRealtime(0.04f);

                var playerDetect = Physics2D.Raycast(
                    transform.position, 
                    _lines.GetPosition(i), 
                    Vector3.Distance(transform.position - _lines.GetPosition(i), transform.position),
                    LayerMask.GetMask("Player"));


                if(playerDetect)
                if(playerDetect.collider.TryGetComponent<PlayerController>(out var component))
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