using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using Mirror;

public class PowerupMovement : NetworkBehaviour
{
	
	public float Cycle;
	public float Cyclerate;
	public float turnSpeed;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		float Change = Cycle;
        transform.position += new Vector3( 0.0f, Change * Time.deltaTime, 0.0f );
		transform.Rotate (0.0f, turnSpeed * Time.deltaTime,0.0f, Space.World);
		if(Cycle >= 1){
			Cyclerate = -Cyclerate;
		} else if (Cycle <= -1) {
			Cyclerate = -Cyclerate;
		}
		Cycle = Cycle + Cyclerate;
		
			
    }
	void OnCollisionEnter(Collision collision){
		
		if (collision.gameObject.tag == "Player")
		{
			Debug.Log("Destroy Powerup");
			Destroy(gameObject);
		}
		
		
	}
}
