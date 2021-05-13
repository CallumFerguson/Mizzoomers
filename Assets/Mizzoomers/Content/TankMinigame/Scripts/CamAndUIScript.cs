using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;



public class CamAndUIScript : NetworkBehaviour
{
    // Start is called before the first frame update
	public Camera camera;
	public Canvas canvas;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		
        if(isLocalPlayer){
			camera.enabled = true;
			canvas.enabled = true;
		} else {
			camera.enabled = false;
			canvas.enabled = false;
		}
    }
}
