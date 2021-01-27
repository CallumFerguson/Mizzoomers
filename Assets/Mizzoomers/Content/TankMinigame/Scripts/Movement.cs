using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

	// Use this for initialization
	public float moveSpeed;
	public float turnSpeed;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.A))
			transform.Rotate (0.0f,-turnSpeed * Time.deltaTime,0.0f);
		if (Input.GetKey (KeyCode.D))
			transform.Rotate (0.0f, turnSpeed * Time.deltaTime,0.0f);
		if (Input.GetKey (KeyCode.W))
			transform.Translate ( 0.0f, 0.0f, moveSpeed * Time.deltaTime);
		if (Input.GetKey (KeyCode.S))
			transform.Translate ( 0.0f, 0.0f, -moveSpeed * Time.deltaTime);
		
	}
}
