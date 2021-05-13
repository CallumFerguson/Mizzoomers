using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mirror;

public class ProjectileScript : NetworkBehaviour
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
	//[Command]
	void SpawnExplosion(){
		GameObject explode;
        explode = NetworkManager.Instantiate(Explosion,transform.position, transform.rotation);
		NetworkServer.Spawn(explode);
	}
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject != firingTank && collision.gameObject != MuzzelFlash && collision.gameObject.tag == "Player")
        {
			ProjectileCollider.enabled = false;
			try{
				SpawnExplosion();
			} catch{
				
			}
            
			
			
			ScoreAndDamage(collision.gameObject, firingTank);
			/*
			float healthBeforeHit = 100;
			try{
				healthBeforeHit = collision.gameObject.GetComponent<ModifyHealth>().health;
			} catch {
				
			}
			try{
				collision.gameObject.GetComponent<ModifyHealth>().ChangeHealth(-damage);
				if(healthBeforeHit > 0){
					firingTank.GetComponent<FireScript>().Score(damage);
					if(healthBeforeHit <= damage){
						firingTank.GetComponent<FireScript>().Score(damage*3);
					}
				}
			} catch {
				
				
			}
			*/
			
            
			Destroy(Projectile);
            
        }else if(collision.gameObject != firingTank && collision.gameObject != MuzzelFlash){
			SpawnExplosion();
			Destroy(Projectile);
		}
		
        

    }
	[ClientRpc]
	void ScoreAndDamage(GameObject HitTank, GameObject ScoreTank){
		float healthBeforeHit = 100;
		GameObject DamagedTank = NetworkIdentity.spawned[HitTank.GetComponent<NetworkIdentity>().netId].gameObject;
		GameObject ScoringTank = NetworkIdentity.spawned[ScoreTank.GetComponent<NetworkIdentity>().netId].gameObject;
		damage = ScoringTank.GetComponent<FireScript>().Damage;
		//try{
			healthBeforeHit = DamagedTank.GetComponent<ModifyHealth>().health;
		//} catch {
				
		//}
		//try{
			DamagedTank.GetComponent<ModifyHealth>().ChangeHealth(-damage);
			if(healthBeforeHit > 0){
				ScoringTank.GetComponent<FireScript>().Score(damage);
				if(healthBeforeHit <= damage){
					ScoringTank.GetComponent<FireScript>().Score(damage*3);
				}
			}
		//} catch {
				
				
		//}
		
	}
	
}
