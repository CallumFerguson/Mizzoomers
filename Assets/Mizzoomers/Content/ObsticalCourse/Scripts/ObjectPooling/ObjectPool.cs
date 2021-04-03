using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool : MonoBehaviour
{
    public GameObject spherePrefab;
    public Queue<GameObject> spherePool = new Queue<GameObject>();
    public int poolStartSize = 6;

    private void Start()
    {
        for (int i = 0; i < poolStartSize; i++)
        {
            GameObject sphere = Instantiate(spherePrefab);
            spherePool.Enqueue(sphere);
            sphere.SetActive(false);
        }
    }

    public GameObject GetSphere()
    {
        if (spherePool.Count > 0)
        {
            GameObject sphere = spherePool.Dequeue();
            sphere.SetActive(true);
            return sphere;
        }
        else
        {
            GameObject sphere = Instantiate(spherePrefab);
            return sphere;
        }
    }

    public void ReturnSphere(GameObject sphere)
    {
        spherePool.Enqueue(sphere);
        sphere.SetActive(false);
    }
}

