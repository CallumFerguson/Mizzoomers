using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mirror;
using System.Linq;


public class PlayerManager : NetworkBehaviour
{
	// Server only program
    // Start is called before the first frame update
	public List<GameObject> players = new List<GameObject>();
	public List<int> playerScores = new List<int>();
	public float RoundTimer; //in seconds
	public bool RoundInProgress;
	
	
	
    void Start()
    {
        RoundInProgress = true;
		//how long the round lasts in seconds
		RoundTimer = 180;
    }

    private void OnEnable()
    {
	    if (isServer)
	    {
		    players.Clear();
	    }
    }

    // Update is called once per frame
    void Update()
    {
	    if (!isServer)
	    {
		    return;
	    }
	    
		int i = 0;
		List<GameObject> fetchedPlayers = new List<GameObject>();
		
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")){
			fetchedPlayers.Add(player);
			if(players.Contains(player)){
				//nothing
			} else {
				players.Add(player);
				playerScores.Add(player.GetComponent<FireScript>().PlayerScore);
				
			}
			
			//playerScores[i] =GetScores(player);
			i++;
			
		}
		i = 0;
		foreach(GameObject player in players){
			playerScores[i] = player.GetComponent<FireScript>().PlayerScore;
			i++;
		}
		
		if(RoundInProgress == true){
			RoundTimer -= Time.deltaTime;
			if(RoundTimer <= 0){
				RoundTimer = 0;
			}
			//GameObject[] PlayerList = GameObject.FindGameObjectsWithTag("Player");
			
			UpdateTime(RoundTimer, players, playerScores);
		}
    }
	
	void StartTimer(int length){
		RoundTimer = length;
		RoundInProgress = true;
	}
	
	[Server]
	[ClientRpc]
	void UpdateTime(float RoundTimer, List<GameObject> PlayerList, List<int> Scores){
		if(RoundTimer <= 0)
		{
			//Sort list
			int len = PlayerList.Count;
			GameObject[] Finalists = new GameObject[len];
			int i = 0;
			foreach(GameObject x in PlayerList){
				int highestScore = 0;
				int HighIndex = 0;
				int j = 0;
				foreach(int Final in Scores){
					if (playerScores[j] > highestScore){
						highestScore = Scores[j];
						HighIndex = j;
					}
					j++;
					
				}
				Finalists[i] = PlayerList[HighIndex];
				Scores[HighIndex] = 0;
				i++;
			}
			
			//disable players and send list for end screen
			foreach (GameObject player in PlayerList){
				GameObject Affectedplayer = NetworkIdentity.spawned[player.GetComponent<NetworkIdentity>().netId].gameObject;
				Affectedplayer.GetComponent<ModifyHealth>().enabled = false;
				Affectedplayer.GetComponent<Movement>().enabled = false;
				Affectedplayer.GetComponent<FireScript>().enabled = false;
				Affectedplayer.GetComponent<UiScript>().DisplayEnd(Finalists);
				
				//disables each players movements once round is over.
			}	
		} else {
			
			foreach (GameObject player in PlayerList){
				GameObject Affectedplayer = NetworkIdentity.spawned[player.GetComponent<NetworkIdentity>().netId].gameObject;
				Affectedplayer.GetComponent<UiScript>().DisplayTime(RoundTimer);
				// sends each player how long is remaining in the round.
			}
		}
		
		
		
	}
	
	
	/*
	[ClientRpc]
	void GetScores(GameObject TankToFind){
		GameObject Tank = NetworkIdentity.spawned[TankToFind.GetComponent<NetworkIdentity>().netId].gameObject;
		int Score = Tank.GetComponent<FireScript>().PlayerScore;
		//return Score;
	}
	
	*/
	
	
}
