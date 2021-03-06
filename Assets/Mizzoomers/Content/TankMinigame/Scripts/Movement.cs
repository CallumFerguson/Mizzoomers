﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Movement : NetworkBehaviour
{

	[SyncVar] public NetworkIdentity owner;
	
	// Use this for initialization
	public float moveSpeed;
	public float turnSpeed;
	public float TurretturnSpeed;
	public float BarrelturnSpeed;
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
	public GameObject Turret;
	public GameObject Barrel;
	public Transform Cam1;
	public Transform Cam2;
	public Camera Playercamera;
	bool camPosition;

	public override void OnStartClient()
	{
		startTime = 0f;
		reset = false;
		enabled = true;
		PowerupTimer = 0;
		camPosition = true;
		Playercamera.transform.position = Cam1.transform.position;
		Playercamera.transform.rotation = Cam1.transform.rotation;
		Playercamera.fieldOfView = 60.0f;

		Playercamera.gameObject.SetActive(false);
		if(owner.isLocalPlayer){
			Playercamera.gameObject.SetActive(true);
		}
		
		if (!owner.isLocalPlayer)
		{
			return;
		}

		StartCoroutine(LookForStartPosition());
	}
	
	private IEnumerator LookForStartPosition()
	{
		Transform startPosition = null;
		do
		{
			if (NetworkManagerGame.singleton)
			{
				startPosition = NetworkManagerGame.singleton.GetStartPosition();
			}
			yield return null;
		} while (startPosition == null);
		transform.position = startPosition.position;
		transform.rotation = startPosition.rotation;
	}

	// Update is called once per frame
	void Update () {
		
		if(!(transform.localEulerAngles.z >= 90 && transform.localEulerAngles.z <= 270) && MoveEnabled == true && owner.isLocalPlayer)
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
		if(Input.GetKeyDown(KeyCode.O)&& owner.isLocalPlayer)
		{
			startTime = Time.time;
			reset = true;
		}
		if(Input.GetKeyUp(KeyCode.O) && owner.isLocalPlayer)
		{
			reset = false;
		}
		if (Input.GetKey (KeyCode.LeftArrow )&& owner.isLocalPlayer)
			Turret.transform.Rotate (0.0f,0.0f,-TurretturnSpeed * Time.deltaTime);
		if (Input.GetKey (KeyCode.RightArrow )&& owner.isLocalPlayer)
			Turret.transform.Rotate (0.0f, 0.0f,TurretturnSpeed * Time.deltaTime);
		if (Input.GetKey (KeyCode.UpArrow) && owner.isLocalPlayer) {
			//print (transform.localEulerAngles.x);
			if (Barrel.transform.localEulerAngles.x <= 18 || Barrel.transform.localEulerAngles.x > 340) {
				Barrel.transform.Rotate (-BarrelturnSpeed * Time.deltaTime,0.0f,0.0f);
			}
		}

				
		if (Input.GetKey (KeyCode.DownArrow) && owner.isLocalPlayer) {
			//print (transform.localEulerAngles.x);
			if (Barrel.transform.localEulerAngles.x < 16 || Barrel.transform.localEulerAngles.x >= 330) {
				Barrel.transform.Rotate (BarrelturnSpeed * Time.deltaTime, 0.0f, 0.0f);
			}
		}
		
		if(Input.GetKey(KeyCode.O) && owner.isLocalPlayer)
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
		if(MoveEnabled == false && owner.isLocalPlayer)
		{
			if(disableTimer + disableTime <= Time.time)
			{
				MoveEnabled = true;
			}
		}
		if (PowerupTimer <= 0 && owner.isLocalPlayer)
        {
            moveSpeed = NormalMoveSpeed;
			turnSpeed = NormalTurnSpeed;
        } else if(PowerupTimer > 0 && owner.isLocalPlayer){
			PowerupTimer -= Time.deltaTime;
			
		}
		if (Input.GetKeyDown (KeyCode.Z) && owner.isLocalPlayer) {
			if (camPosition == true) {
				Playercamera.transform.position = Cam2.transform.position;
				Playercamera.transform.rotation = Cam2.transform.rotation;
				Playercamera.fieldOfView = 40.0f;
				camPosition = false;
			} else {
				Playercamera.transform.position = Cam1.transform.position;
				Playercamera.transform.rotation = Cam1.transform.rotation;
				Playercamera.fieldOfView = 60.0f;
				camPosition = true;
			}
		}
		if (camPosition == false) {
			Playercamera.transform.position = Cam2.transform.position;
			Playercamera.transform.rotation = Cam2.transform.rotation;
		}
		
	}
	
	void OnCollisionEnter(Collision collision){
		
		if (collision.gameObject.tag == "SpeedPowerup" && owner.isLocalPlayer)
		{
			moveSpeed = PowerupMoveSpeed;
			turnSpeed = PowerupTurnSpeed;
			PowerupTimer = 30;
		}
		
		
	}
	public bool ISPlayer(){
		return owner.isLocalPlayer;
	}
}
