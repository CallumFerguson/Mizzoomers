using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
	public float speed = 2f;
    public float direction = 1;


    // Update is called once per frame
    void Update()
    {
		transform.Rotate(0f, direction * speed * Time.deltaTime,0f, Space.Self);
	}
}
