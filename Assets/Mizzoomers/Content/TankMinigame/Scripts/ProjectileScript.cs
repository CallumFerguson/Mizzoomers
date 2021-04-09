using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject firingTank;
    public GameObject Explosion;
    public GameObject MuzzelFlash;
	public GameObject Projectile;
	public Collider ProjectileCollider;
	public int damage;
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
			ProjectileCollider.enabled = false;
            GameObject explode;
            explode = Instantiate(Explosion,transform.position, transform.rotation);
			float healthBeforeHit = collision.gameObject.GetComponent<ModifyHealth>().health;
			collision.gameObject.GetComponent<ModifyHealth>().ChangeHealth(-damage);
			if(healthBeforeHit > 0){
				firingTank.GetComponent<FireScript>().Score(damage);
				if(healthBeforeHit <= damage){
					firingTank.GetComponent<FireScript>().Score(damage*3);
				}
			}
            
			
            Destroy(Projectile);
        }
        

    }
}
