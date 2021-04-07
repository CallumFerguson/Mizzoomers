using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowball : MonoBehaviour
{

    // public Rigidbody rb;
    public GameObject snowball;

    public Transform firePoint;
    public Rigidbody snowballPrefab;

    public float sbSpeed = 800f;

    bool canFire = true;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && canFire)
        {
            StartCoroutine(ThrowSnowball());
        }

        

    }

    IEnumerator ThrowSnowball()
    {
        yield return new WaitForSeconds(0.4f);
        
        GameObject sb = Instantiate(snowball, firePoint.position, firePoint.rotation);
        Rigidbody sbrb = sb.GetComponent<Rigidbody>();

        sbrb.velocity = transform.TransformDirection(Vector3.forward * sbSpeed);
        canFire = false;

        

        // snowballInstance.AddForce(firePoint.forward * sbSpeed);
        Destroy(sb, 1.5f);

        yield return new WaitForSeconds(0.5f);
        canFire = true;

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

