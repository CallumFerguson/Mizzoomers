using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    // Start is called before the first frame update
	public GameObject Armor;
	public GameObject Speed;
	public GameObject Cannon;
	public GameObject Health;
	
	public float timer;
	public int collisions;
	public bool hasPowerup;
    void Start()
    {
        
		timer = Random.Range(5.0f,45.0f);
		collisions = 0;
		hasPowerup = false;
    }

    // Update is called once per frame
    void Update()
    {
		if(timer > 0 && collisions == 0){
			timer -= Time.deltaTime;
		} else if(timer <= 0 && collisions == 0){
			Spawn();
		} else {
			
		}
        
    }
	
	void Spawn(){
		float choice = Random.Range(0.0f, 4.0f);
		Debug.Log(choice);
		GameObject clone;
		if(choice <= 1 && choice >= 0){
			clone = Instantiate(Armor, transform.position + new Vector3(0,0,0),Quaternion.identity);
		} else if(choice <= 2 && choice >1 ){
			clone = Instantiate(Speed, transform.position + new Vector3(0,0,0),Quaternion.identity);
		} else if(choice <= 3 && choice > 2){
			clone = Instantiate(Cannon, transform.position + new Vector3(0,0,0),Quaternion.identity);
		} else {
			clone = Instantiate(Health, transform.position + new Vector3(0,0,0),Quaternion.identity);
		}
		hasPowerup = true;
		collisions++;
		
	}
	void OnTriggerEnter(Collider collision)
	{
		if(collision.gameObject.tag != "Projectile")
		{
			collisions++;
			if(hasPowerup == true){
				hasPowerup = false;
				collisions--;
			}
		} 
		
		
	}

	void OnTriggerExit(Collider collision)
	{
		if(collision.gameObject.tag != "Projectile")
		{
			collisions--;
			if(timer <= 20){
				timer = Random.Range(20.0f,45.0f);
			}
		}
		
	}
}
