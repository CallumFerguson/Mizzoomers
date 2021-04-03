using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerateWorld : NetworkBehaviour
{
    public GameObject blockPrefab;

    [SyncVar] private int _seed;

    private bool _worldCreated = false;

    public override void OnStartServer()
    {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int cur_time = (int) (System.DateTime.UtcNow - epochStart).TotalSeconds;
        _seed = cur_time;

        CreateWorld();
    }

    public override void OnStartClient()
    {
        CreateWorld();
    }

    private void CreateWorld()
    {
        if (_worldCreated)
        {
            return;
        }

        _worldCreated = true;

        Random.InitState(_seed);

        for (int i = 0; i < 500; i++)
        {
            CreatePlatform(new Vector3(Random.value * 100, Random.value, Random.value * 100), new Vector2(Random.value * 5 + 1, Random.value * 5 + 1));
        }
    }

    private void CreatePlatform(Vector3 position, Vector2 size)
    {
        float height = position.y + 10 + 5;

        var block = Instantiate(blockPrefab, transform);

        block.transform.position = new Vector3(position.x, height / 2f - 10, position.z);
        block.transform.rotation = Quaternion.identity;
        block.transform.localScale = new Vector3(size.x, height, size.y);
    }
}