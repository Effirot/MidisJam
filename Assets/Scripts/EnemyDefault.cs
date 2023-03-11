using UnityEngine;

using System.Collections.Generic;
using System.Collections;
using System;

public class EnemyDefault : Entity{


    public IEnumerator Aiming(){
        for(int i=0;i<1000;i++)
        {
            yield return new WaitForFixedUpdate();
        }
    }







}