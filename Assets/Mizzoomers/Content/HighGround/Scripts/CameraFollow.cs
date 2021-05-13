using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;

    void Update()
    {
        if (target)
        {
            var targetPosition = target.position;
            transform.position = targetPosition;
        }
        else if (HighGroundPlayerController.localPlayer)
        {
            target = HighGroundPlayerController.localPlayer.transform;
        }
    }
}