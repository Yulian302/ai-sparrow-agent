using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform target;
    public float distanceFromTarget = 2f;
    public float heightAboveTarget = 3f;

    void Update()
    {
        Vector3 targetPosition = target.transform.position;
        Vector3 cameraPosition = targetPosition - distanceFromTarget * target.transform.forward;
        cameraPosition.y += heightAboveTarget;
        transform.position = cameraPosition;
        transform.LookAt(targetPosition);
    }
}