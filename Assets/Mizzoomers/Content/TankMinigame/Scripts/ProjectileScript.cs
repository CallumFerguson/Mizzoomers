using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject firingTank;
    public GameObject Explosion;
    public GameObject MuzzelFlash;
    private float Timeout;
    
    void Start()
    {
        Timeout = 10;
    }

    // Update is called once per frame
    void Update()
    {
        Timeout -= Time.deltaTime;
        if (Timeout <= 0)
        {
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject != firingTank && collision.gameObject != MuzzelFlash)
        {
            GameObject explode;
            explode = Instantiate(Explosion,transform.position, transform.rotation);
            //damage tank
            Destroy(gameObject);
        }
        

    }
}
