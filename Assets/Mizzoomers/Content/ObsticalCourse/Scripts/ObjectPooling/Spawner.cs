using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private float timeToSpawn = 3f; //How frequently the object spawns
    private float timeSinceSpawn; //How long since the last object was spawned
    private ObjectPool objectPool; //Reference to object pool

    void Start()
    {
        objectPool = FindObjectOfType<ObjectPool>();
    }

    void Update()
    {
        timeSinceSpawn += Time.deltaTime;
        if(timeSinceSpawn >= timeToSpawn)
        {
            GameObject newSphere = objectPool.GetSphere();
            newSphere.transform.position = this.transform.position; //Initialize object, move to correct position
            timeSinceSpawn = 0f; //Without this the spawner would create a new sphere every frame
        }
    }

}
