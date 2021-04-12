using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularMovement : MonoBehaviour
{
    private Transform diskZone;
    private Transform cylinderZone;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SpinDisk")
        {
            diskZone = other.transform;
        }
        if(other.tag == "Cylinder")
        {
            cylinderZone = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (diskZone == other.transform) diskZone = null;
    }

    private void FixedUpdate()
    {
        if (diskZone)
        {
            Debug.Log("On Disk!");
            var center = diskZone.position - rb.position;

            var axis = Vector3.Cross(rb.velocity, center);

            var newVel = Vector3.Cross(center, axis).normalized;

            rb.velocity = newVel * rb.velocity.magnitude;
        }

        if (cylinderZone)
        {
            Debug.Log("On Cylinder!");
            var newVel = new Vector3(1, 0, 0) * Time.deltaTime;
            rb.velocity = newVel * rb.velocity.magnitude;
        }
    }
}
