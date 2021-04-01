using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

	// Use this for initialization
	public float moveSpeed;
	public float turnSpeed;
	private float startTime;
	public float holdTime;
	public float resetHeight;
	private bool reset;
	private bool MoveEnabled;
	public float disableTime;
	private float disableTimer;
	public float ArmorPowerupTime;
	public bool ArmorPowerup;
	public float NormalMoveSpeed;
	public float NormalTurnSpeed;
	public float PowerupTurnSpeed;
	public float PowerupMoveSpeed;
	public float PowerupTimer;
	
	void Start () {
		startTime = 0f;
		reset = false;
		enabled = true;
		PowerupTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(!(transform.localEulerAngles.z >= 90 && transform.localEulerAngles.z <= 270) && MoveEnabled == true)
		{
			if (Input.GetKey (KeyCode.A))
				transform.Rotate (0.0f,-turnSpeed * Time.deltaTime,0.0f);
			if (Input.GetKey (KeyCode.D))
				transform.Rotate (0.0f, turnSpeed * Time.deltaTime,0.0f);
			if (Input.GetKey (KeyCode.W))
				transform.Translate ( 0.0f, 0.0f, moveSpeed * Time.deltaTime);
			if (Input.GetKey (KeyCode.S))
				transform.Translate ( 0.0f, 0.0f, -moveSpeed * Time.deltaTime);
		}
		if(Input.GetKeyDown(KeyCode.O))
		{
			startTime = Time.time;
			reset = true;
		}
		if(Input.GetKeyUp(KeyCode.O))
		{
			reset = false;
		}
		
		if(Input.GetKey(KeyCode.O))
		{
			if(startTime + holdTime <= Time.time && reset == true)
			{
				transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0.0f);
				transform.Translate(0.0f,resetHeight,0.0f);
				reset = false;
				MoveEnabled = false;
				disableTimer = Time.time;
			}
		}
		if(MoveEnabled == false)
		{
			if(disableTimer + disableTime <= Time.time)
			{
				MoveEnabled = true;
			}
		}
		if (PowerupTimer <= 0)
        {
            moveSpeed = NormalMoveSpeed;
			turnSpeed = NormalTurnSpeed;
        } else if(PowerupTimer > 0){
			PowerupTimer -= Time.deltaTime;
			
		}
		
	}
	
	void OnCollisionEnter(Collision collision){
		
		if (collision.gameObject.tag == "SpeedPowerup")
		{
			moveSpeed = PowerupMoveSpeed;
			turnSpeed = PowerupTurnSpeed;
			PowerupTimer = 30;
		}
		
		
	}
}
