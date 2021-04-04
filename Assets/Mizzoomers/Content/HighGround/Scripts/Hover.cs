using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Hover : MonoBehaviour
{
    private Rigidbody _body;
    private Vector3 _startPosition;
    private float _random;
    private float _range;
    
    private void Start()
    {
        _body = gameObject.AddComponent<Rigidbody>();
        _body.isKinematic = true;

        _startPosition = transform.position;
        _random = Random.value * 100f;
        _range = Random.value * 2f + 1f;
    }

    private void FixedUpdate()
    {
        _body.MovePosition(_startPosition + new Vector3(0, ((Mathf.Sin(Time.time + _random) + 1f) / 2f) * _range, 0));
    }
}
