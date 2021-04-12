using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Mirror;

public class ModifyHealth : NetworkBehaviour
{
	public Text HealthText;
	public float health;
	public float PowerupTimer;
	public float armor;
	public GameObject Door1;
	public GameObject Door2;
	public GameObject DoorTop;
	public GameObject Explosion;
	public GameObject Fire;
	public GameObject Player;
	GameObject explode;
	GameObject fire;
	public bool dead;
	//public GameObject turret;
	
	public float deathTimer;
	public float healthTimer;
	
    // Start is called before the first frame update
    void Start()
    {
        health = 100;
		armor = 1;
		PowerupTimer = 0;
		dead = false;
		healthTimer = 0;
		
    }

    // Update is called once per frame
    void Update()
    {
		
        
        if (PowerupTimer <= 0)
        {
            armor = 1;
        } else if(PowerupTimer > 0){
			PowerupTimer -= Time.deltaTime;
			Debug.Log("PowerupTimer: " + PowerupTimer.ToString());
		}
		if(healthTimer > 0){
			healthTimer -= Time.deltaTime;
		}
		if(health <= 0 && dead == false){
			DIE();
			deathTimer = 20;
			dead = true;
		} 
		if(dead == true)
		{
			deathTimer -= Time.deltaTime;
			try{
				explode.transform.position = Player.transform.position;
				fire.transform.position = Player.transform.position;
			} catch {
				
			}
			
			if(deathTimer <= 0){
				Destroy(Player);
			}
			
		}
    }
	public void ChangeHealth(float change)
	{
		if(change < 0){
			change = change * armor;
		}
		health = health + change;
		if(health >= 125){
			health = 125;
		}
		HealthText.text = "HP: " + health.ToString();
	}
	void OnCollisionEnter(Collision collision){
		
		if (collision.gameObject.tag == "ArmorPowerup" && isLocalPlayer)
		{
			PowerupTimer = 30;
			armor = .5f;
		} else if(collision.gameObject.tag == "HealthPowerup" && healthTimer <= 0 && isLocalPlayer){
			healthTimer = 1;
			ChangeHealth(10);
			
		}
		
		
	}
	void DIE(){
		Player.tag = "DeadPlayer";
		
		explode = Instantiate(Explosion,transform.position, transform.rotation);
		fire = Instantiate(Fire,transform.position , transform.rotation);
		Door1.transform.Rotate(0.0f,0.0f,-90.0f);
		Door2.transform.Rotate(0.0f,0.0f,90.0f);
		DoorTop.transform.Rotate(-90.0f,0.0f,0.0f);
		Player.GetComponent<Movement>().enabled = false;
		Player.GetComponent<FireScript>().enabled = false;
		
		//turret.GetComponent<turretRotation>().enabled = false;
		//GameObject barrelBase = turret.transform.GetChild(2).gameObject;
		//GameObject barrel = barrelBase.transform.GetChild(0).gameObject;
		//barrel.GetComponent<BarrelMovement>().enabled = false;
		
	}
}
