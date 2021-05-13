using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
//
public class BarrelMovement : NetworkBehaviour {
	public float turnSpeed;
	private float Temp;
	// Use this for initialization
	void Start () {
		
	}
	//z cannot be greater than 5 or less than -13
	// Update is called once per frame
	void Update () {
		// between 16 and  340

		if (Input.GetKey (KeyCode.UpArrow) && isLocalPlayer) {
			//print (transform.localEulerAngles.x);
			if (transform.localEulerAngles.x <= 18 || transform.localEulerAngles.x > 340) {
				transform.Rotate (-turnSpeed * Time.deltaTime,0.0f,0.0f);
			}
		}

				
		if (Input.GetKey (KeyCode.DownArrow) && isLocalPlayer) {
			//print (transform.localEulerAngles.x);
			if (transform.localEulerAngles.x < 16 || transform.localEulerAngles.x >= 330) {
				transform.Rotate (turnSpeed * Time.deltaTime, 0.0f, 0.0f);
			}
		}

		Temp = transform.localEulerAngles.z;
				
	}
}
