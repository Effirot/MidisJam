using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] Camera target;
    Vector2 cursorPoint => target.ScreenToWorldPoint(Input.mousePosition);
    Vector3 midPoint => new Vector3(((transform.position.x * 3) + cursorPoint.x) / 4, ((transform.position.y * 3)  + cursorPoint.y) / 4, target.gameObject.transform.position.z);

    private void LateUpdate() {
        
        target.gameObject.transform.position = Vector3.Lerp(target.gameObject.transform.position, midPoint, 0.04f);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(midPoint, 0.1f);
    }
}
