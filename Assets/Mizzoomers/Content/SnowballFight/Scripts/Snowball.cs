using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Snowball : NetworkBehaviour
{
    [SyncVar] public NetworkIdentity owner;
    
    // public Rigidbody rb;
    public GameObject snowball;

    public Transform firePoint;
    public Rigidbody snowballPrefab;

    public float sbSpeed = 800f;

    bool canFire = true;

    // Update is called once per frame
    void Update()
    {
        if (!owner.isLocalPlayer)
        {
            return;
        }
        
        if (Input.GetButtonDown("Fire1") && canFire && firePoint)
        {
            canFire = false;
            CmdThrowSnowball(transform.TransformDirection(Vector3.forward * sbSpeed), firePoint.position, firePoint.rotation);
            StartCoroutine(WaitThenCanFire());
        }
    }
    
    private IEnumerator WaitThenCanFire()
    {
        yield return new WaitForSeconds(0.5f);
        canFire = true;
    }

    [Command]
    private void CmdThrowSnowball(Vector3 velocity, Vector3 startPos, Quaternion startRot)
    {
        StartCoroutine(ThrowSnowball(velocity, startPos, startRot));
    }

    IEnumerator ThrowSnowball(Vector3 velocity, Vector3 startPos, Quaternion startRot)
    {
        yield return new WaitForSeconds(0.25f);

        GameObject sb = Instantiate(snowball, startPos, startRot);
        Rigidbody sbrb = sb.GetComponent<Rigidbody>();

        sbrb.velocity = velocity;
        canFire = false;
        
        NetworkServer.Spawn(sb);

        // snowballInstance.AddForce(firePoint.forward * sbSpeed);
        // Destroy(sb, 2.5f);
        yield return new WaitForSeconds(2.5f);
        NetworkServer.Destroy(sb);
    }
}