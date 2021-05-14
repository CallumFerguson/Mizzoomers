using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MinigamePlayer : NetworkBehaviour
{
    public GameObject[] playerPrefabs;
    private SyncList<NetworkIdentity> ownedPlayers = new SyncList<NetworkIdentity>();

    public override void OnStartServer()
    {
        StartCoroutine(WaitThenSpawn());
    }

    private IEnumerator WaitThenSpawn()
    {
        yield return new WaitForSeconds(0.5f);
        
        var manager = GameObject.FindObjectOfType<MinigameManager>();
        if (manager && manager.currentGameIndex > 0)
        {
            SetPlayerForMinigame(GameObject.FindObjectOfType<MinigameManager>().currentGameIndex - 1);
        }
    }

    [Server]
    public void SetPlayerForMinigame(int index)
    {
        for (int i = ownedPlayers.Count - 1; i >= 0; i--)
        {
            NetworkServer.Destroy(ownedPlayers[i].gameObject);
            ownedPlayers.Remove(ownedPlayers[i]);
        }
        
        var player = Instantiate(playerPrefabs[index]);
        player.transform.position = transform.position;
        player.transform.rotation = transform.rotation;

        switch (index)
        {
            case 0:
                player.GetComponent<HighGroundPlayerController>().owner = netIdentity;
                break;
            case 1:
                player.GetComponent<Movement>().owner = netIdentity;
                player.GetComponent<FireScript>().owner = netIdentity;
                player.GetComponent<ModifyHealth>().owner = netIdentity;
                player.GetComponent<UiScript>().owner = netIdentity;
                break;
            case 2:
                player.GetComponent<PlayerController>().owner = netIdentity;
                break;
            case 3:
                player.GetComponent<ThirdPersonMovement>().owner = netIdentity;
                player.GetComponent<Snowball>().owner = netIdentity;
                break;
            default:
                Debug.LogError("Uknown index: " + index);
                break;
        }

        ownedPlayers.Add(player.GetComponent<NetworkIdentity>());

        NetworkServer.Spawn(player, connectionToClient);
    }
}