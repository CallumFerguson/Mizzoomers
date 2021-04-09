﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class FireScript : NetworkBehaviour
{
    // Start is called before the first frame update
    public GameObject FiringTank;
    public Transform EndOfBarrel;
    //public Transform EndOfBarrel2;
    public GameObject Projectile;
    public GameObject MuzzelExplosion;
    public float ProjectileSpeed;
    private bool canFire;
    private bool timerRunning;
    public float fireTime;
    private float timeLeft;
	public int Damage;
	public int NormalDamage;
	public int PowerupDamage;
	public float PowerupTimer;
	public int PlayerScore;
	public Text ScoreText;
	
    
    void Start()
    {
        canFire = true;
        timerRunning = false;
        timeLeft = 0;
		PowerupTimer = 0;
		Damage = NormalDamage;
            
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("space") && canFire == true && isLocalPlayer)
        {
			canFire = false;
            GameObject clone;
            GameObject flash;
            clone = Instantiate(Projectile, EndOfBarrel.position, EndOfBarrel.rotation);
            flash = Instantiate(MuzzelExplosion, EndOfBarrel.position, EndOfBarrel.rotation);
            ProjectileScript Projectilescript = clone.GetComponent<ProjectileScript>();
            Projectilescript.firingTank = FiringTank;
            Projectilescript.MuzzelFlash = flash;
			Projectilescript.damage = Damage;
            Rigidbody clonebody = clone.GetComponent<Rigidbody>();
            clonebody.velocity = EndOfBarrel.TransformDirection(Vector3.forward * ProjectileSpeed);
            

            timerRunning = true;
            timeLeft = fireTime;
            
        }
        if(timerRunning == true)
        {
            timeLeft -= Time.deltaTime;
            if(timeLeft<= 0)
            {
                canFire = true;
                timerRunning = false;
                
            }
        }
		if (PowerupTimer <= 0)
        {
            Damage = NormalDamage;
        } else if(PowerupTimer > 0){
			PowerupTimer -= Time.deltaTime;
			
		}
		
		ScoreText.text = "Score: " + PlayerScore.ToString();
		
    }
	void OnCollisionEnter(Collision collision){
		
		if (collision.gameObject.tag == "CannonPowerup" && isLocalPlayer)
		{
			Damage = PowerupDamage;
			PowerupTimer = 30;
		}
		
		
	}
	public void Score(int score){
		PlayerScore += score;
	}

}
