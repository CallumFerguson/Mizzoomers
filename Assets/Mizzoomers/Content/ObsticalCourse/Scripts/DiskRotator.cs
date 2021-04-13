using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskRotator : MonoBehaviour
{
    public float direction = 1;
    private Rigidbody rb;
    public Vector3 eulerVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Quaternion deltaRotation = Quaternion.Euler(eulerVelocity* direction * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }
}
