using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UiScript : NetworkBehaviour
{
	[SyncVar] public NetworkIdentity owner;
	
	public GameObject UI;
	public GameObject Player;
	public Text HealthText;
	public Text ScoreText;
	public Text RoundTimer;
	public Text EndScreen;
	
    // Start is called before the first frame update
    void Start()
    {
        UI.SetActive(false);
		if(owner.isLocalPlayer){
			UI.SetActive(true);
			Player = gameObject;
		}
		EndScreen.enabled = false;
		
		
    }

    // Update is called once per frame
    void Update()
    {
		try{
			float health = Player.GetComponent<ModifyHealth>().health;
			int score = Player.GetComponent<FireScript>().PlayerScore;
			HealthText.text = "HP: " + health.ToString();
			ScoreText.text = "Score: " + score.ToString();
			
		} catch{
			
		}
		
		
	
    }
	public void DisplayTime(float TimeLeft){
		int reducedTimer = (int)TimeLeft;
		RoundTimer.text = reducedTimer + " S";
	}
	public void DisplayEnd(GameObject[] Finalists){
		int i  = 1;
		foreach (GameObject player in Finalists){
			
			if(player.GetComponent<NetworkIdentity>().netId == gameObject.GetComponent<NetworkIdentity>().netId){
				
				EndScreen.enabled = true;
				EndScreen.text = "You Placed Number: " + i.ToString();
				
				break;
			}
			i++;
		}
		
	}
}
