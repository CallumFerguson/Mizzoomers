using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowball : MonoBehaviour
{

    // public Rigidbody rb;
    public GameObject snowball;

    public Transform firePoint;
    public Rigidbody snowballPrefab;

    public float sbSpeed = 100f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            throwSnowball();
        }

        

    }

    public void throwSnowball()
    {
        GameObject sb = Instantiate(snowball, firePoint.position, firePoint.rotation);
        Rigidbody sbrb = sb.GetComponent<Rigidbody>();

        sbrb.velocity = transform.TransformDirection(Vector3.forward * sbSpeed);

        // snowballInstance.AddForce(firePoint.forward * sbSpeed);
        Destroy(sb, 3f);

    }

    
            //Rigidbody snowball;
           /* GameObject sb = Instantiate(snowball, transform.position, transform.rotation);
    Rigidbody sbrb = sb.GetComponent<Rigidbody>();

    sbrb.velocity = transform.TransformDirection(Vector3.forward* sbSpeed);
            // sbrb.velocity = transform.forward * sbSpeed;
            //sbrb.AddForce(Vector3.forward * sbSpeed);
            Destroy(sb, 3f);
           */
}

