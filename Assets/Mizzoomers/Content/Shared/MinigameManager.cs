using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MinigameManager : NetworkBehaviour
{
    [SyncVar(hook = nameof(SetCurrentGameIndex))]
    public int currentGameIndex;

    public GenerateWorld generateWorld;

    public Transform minigames;
    
    public GameObject cam;

    private void Start()
    {
        // for (int i = 0; i < minigames.childCount; i++)
        // {
        //     minigames.GetChild(i).gameObject.SetActive(false);
        // }
    }

    public override void OnStopServer()
    {
        cam.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeGame(0);
            generateWorld.ServerSetSeed();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeGame(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeGame(2);
        }
    }

    private void ChangeGame(int index)
    {
        if (currentGameIndex == index + 1)
        {
            return;
        }
        
        var oldValue = currentGameIndex;
        currentGameIndex = index + 1;
        if (!isClient)
        {
            SetCurrentGameIndex(oldValue, currentGameIndex);
        }
    }

    private void SetCurrentGameIndex(int oldValue, int newValue)
    {
        cam.SetActive(false);
        
        for (int i = 0; i < minigames.childCount; i++)
        {
            minigames.GetChild(i).gameObject.SetActive(false);
        }

        minigames.GetChild(newValue - 1).gameObject.SetActive(true);

        var players = GameObject.FindObjectsOfType<MinigamePlayer>();
        for (int i = 0; i < players.Length; i++)
        {
            players[i].SetPlayerForMinigame(newValue - 1);
        }
    }
}