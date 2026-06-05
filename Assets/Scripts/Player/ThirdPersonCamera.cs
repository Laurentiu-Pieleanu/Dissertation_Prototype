using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;

    public float distance = 6f;
    public float height = 3f;
    public float lookHeight = 1.5f;
    public float rotationSmoothSpeed = 6f;
    
    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if(target == null)
            return;

        Vector3 desiredPosition = target.position - target.forward * distance + Vector3.up * height;
        
        transform.position = Vector3.Lerp(transform.position, desiredPosition, rotationSmoothSpeed * Time.deltaTime);

        Vector3 lookTarget = target.position + Vector3.up * lookHeight;
        transform.LookAt(lookTarget);
    }
}
