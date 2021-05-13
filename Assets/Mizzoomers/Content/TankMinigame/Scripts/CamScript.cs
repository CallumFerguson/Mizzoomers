using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour {
	
	bool camPosition;
	public Transform Cam1;
	public Transform Cam2;
	public Camera cam;
	// Use this for initialization
	void Start () {
		camPosition = true;
		this.transform.position = Cam1.transform.position;
		this.transform.rotation = Cam1.transform.rotation;
		cam.fieldOfView = 60.0f;


	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Z)) {
			if (camPosition == true) {
				this.transform.position = Cam2.transform.position;
				this.transform.rotation = Cam2.transform.rotation;
				cam.fieldOfView = 40.0f;
				camPosition = false;
			} else {
				this.transform.position = Cam1.transform.position;
				this.transform.rotation = Cam1.transform.rotation;
				cam.fieldOfView = 60.0f;
				camPosition = true;
			}
		}
		if (camPosition == false) {
			this.transform.position = Cam2.transform.position;
			this.transform.rotation = Cam2.transform.rotation;
		}
	}
}
