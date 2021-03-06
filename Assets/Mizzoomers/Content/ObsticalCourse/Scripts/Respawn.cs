﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private Transform cam;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other);
        if(other.tag == "Player")
        {
            Debug.Log("Respawning!");
            //player.transform.Rotate(180, 0, 0, Space.Self); //Why no work
            player.transform.position = respawnPoint.transform.position;
            cam.transform.position = new Vector3(0, 11, -42);
        }
    }
}
