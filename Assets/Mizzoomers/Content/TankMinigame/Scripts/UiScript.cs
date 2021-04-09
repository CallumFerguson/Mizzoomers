using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UiScript : NetworkBehaviour
{
	public GameObject UI;
	public GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        if(!Player.GetComponent<Movement>().ISPlayer()){
			UI.SetActive(false);
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
