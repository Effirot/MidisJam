using UnityEngine;

using System.Collections.Generic;
using System.Collections;
using System;


[RequireComponent(typeof(LineRenderer))]
public class EnemyDefault : Entity{
    LineRenderer _lines;
    Coroutine currentRoutine;

    public IEnumerator Aiming(){
        for(int i=0;i<1000;i++)
        {
            yield return new WaitForFixedUpdate();
        }
    }

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

            Debug.Log("Detected!! Attack him!!!");
        }

        IEnumerator Attack()
        {
            CurrentWalkState = EntityWalkState.Standing;
            _lines.enabled = true;
            
            for(int i=0;i<35;i++){
                yield return new WaitForFixedUpdate();
                if(Vector3.Distance(transform.position, Movement.current.transform.position) > 7)
                {
                    CurrentWalkState = EntityWalkState.ToPlayer;
                    _lines.enabled = false;
                    
                    currentRoutine = null;

                    yield break;
                }
                
                _lines.SetPosition(0, transform.position);
                _lines.SetPosition(1, Movement.current.transform.position);
                 
            }
            var playerDetect = Physics2D.Raycast(
                transform.position, 
                Movement.current.transform.position - transform.position, 
                Vector3.Distance(transform.position, Movement.current.transform.position),
                LayerMask.GetMask("Player"));


            if(playerDetect)
            if(playerDetect.collider.TryGetComponent<Movement>(out var component))
            {
                component.Death();
            }

            
            yield return new WaitForSecondsRealtime(0.3f);
            
            _lines.enabled = false;


            yield return new WaitForSecondsRealtime(0.3f);
            CurrentWalkState = EntityWalkState.ToPlayer;
            yield return new WaitForSecondsRealtime(1.5f);

            currentRoutine = null;
        }
    }        






}