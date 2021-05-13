using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReturn : MonoBehaviour
{

    private ObjectPool objectPool;
    private bool enteredTrigger = false;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="respawner") enteredTrigger = true;
    }

    // Start is called before the first frame update
    private void Start()
    {
        objectPool = FindObjectOfType<ObjectPool>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (enteredTrigger)
        {
            if (objectPool != null)
            {
                objectPool.ReturnSphere(this.gameObject);
                enteredTrigger = false;
            }
        }
    }
}
