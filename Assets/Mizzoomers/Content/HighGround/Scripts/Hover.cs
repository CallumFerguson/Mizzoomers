using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class Hover : MonoBehaviour
{
    private Rigidbody _body;
    private Vector3 _startPosition;
    private float _random;
    private float _range;

    private float frac(float value)
    {
        return (float)(value - Math.Truncate(value));
    }

    private Vector3 frac(Vector3 value)
    {
        return new Vector3(frac(value.x), frac(value.y), frac(value.z));
    }
    
    private float Hash(Vector3 p)
    {
        p = frac(p * 0.3183099f + new Vector3(0.1f, 0.1f, 0.1f));
        p *= 17.0f;
        return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
    }
    
    private void Start()
    {
        _body = gameObject.AddComponent<Rigidbody>();
        _body.isKinematic = true;

        _startPosition = transform.position;
        _random = Hash(_startPosition) * 100f;
        _range = Hash(_startPosition) * 2f + 1f;
    }

    private void FixedUpdate()
    {
        if (!NetworkClient.isConnected && NetworkServer.active)
        {
            return;
        }
        
        _body.MovePosition(_startPosition + new Vector3(0, ((Mathf.Sin((float)NetworkTime.time + _random) + 1f) / 2f) * _range, 0));
    }
}
