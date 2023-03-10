using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlatform : MonoBehaviour
{
    private PlatformEffector2D effector2D;
    public float waitTime = 0.3f;

    private void Start()
    {
        effector2D = GetComponent<PlatformEffector2D>();
    }
    private void Update()
    {

        if (Input.GetKeyUp(KeyCode.S))
        {
            waitTime = 0.3f;
            effector2D.rotationalOffset = 0;
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (waitTime<=0)
            {
                effector2D.rotationalOffset = 180;
                waitTime = 0.3f;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
        if (Input.GetButtonUp("Jump") || Input.GetKeyUp(KeyCode.Space))
        {
            effector2D.rotationalOffset = 0;
        }
    }
}
