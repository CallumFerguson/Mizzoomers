using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        var targetPosition = target.position;
        targetPosition.y = 1f;
        transform.position = targetPosition;
    }
}