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

    public override void OnStartServer()
    {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int cur_time = (int) (System.DateTime.UtcNow - epochStart).TotalSeconds;
        _seed = cur_time;

        if (isServer && !isClient)
        {
            CreateWorld();
        }
    }

    public override void OnStartClient()
    {
        CreateWorld();
    }

    private void CreateWorld()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        Random.InitState(_seed);

        var nodes = new List<Node>();

        for (int i = 0; i < 15; i++)
        {
            const float size = 35f;
            const float space = 7.5f;


            Vector2 pos = Vector2.zero;
            int attempts = 10;
            do
            {
                attempts--;
                if (attempts == -1)
                {
                    break;
                }

                var x = Random.value * size;
                var z = Random.value * size;
                pos = new Vector2(x, z);
            } while (PosNearAnother(pos, nodes, space));

            if (attempts >= 0)
            {
                Node node;
                node.pos = new Vector3(pos.x, Random.value * 5, pos.y);
                node.rotation = Quaternion.identity;
                node.scale = new Vector2(Random.value * 5 + 1, Random.value * 5 + 1);
                nodes.Add(node);
            }
        }

        print($"Created {nodes.Count} nodes.");

        var edges = new HashSet<(int, int)>();

        for (int i = 0; i < 100; i++)
        {
            var i1 = Random.Range(0, nodes.Count);
            var i2 = Random.Range(0, nodes.Count);
            if (i1 != i2 && !edges.Contains((i1, i2)))
            {
                if (!IntersectAny((i1, i2), edges, nodes))
                {
                    edges.Add((i1, i2));
                }
            }
        }

        print($"Created {edges.Count} edges.");

        for (int i = 0; i < nodes.Count; i++)
        {
            CreatePlatform(nodes[i]);
        }


        foreach (var edge in edges)
        {
            var n1 = nodes[edge.Item1];
            var n2 = nodes[edge.Item2];
            var p1 = n1.pos + new Vector3(0, 5 - 0.251f, 0);
            var p2 = n2.pos + new Vector3(0, 5 - 0.251f, 0);
            CreateBridge(p1, p2);
        }
    }

    private bool PosNearAnother(Vector2 pos, List<Node> nodes, float space)
    {
        for (int n = 0; n < nodes.Count; n++)
        {
            var nodePos = new Vector2(nodes[n].pos.x, nodes[n].pos.z);
            if (Vector2.Distance(pos, nodePos) < space)
            {
                return true;
            }
        }

        return false;
    }

    //from: https://stackoverflow.com/questions/3838329/how-can-i-check-if-two-segments-intersect
    private bool ccw(Vector2 A, Vector2 B, Vector2 C)
    {
        return (C.y - A.y) * (B.x - A.x) > (B.y - A.y) * (C.x - A.x);
    }

    //Return true if line segments AB and CD intersect
    private bool Intersect(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
    {
        return ccw(A, C, D) != ccw(B, C, D) && ccw(A, B, C) != ccw(A, B, D);
    }

    private bool Intersect((int, int) edge1, (int, int) edge2, List<Node> nodes)
    {
        var A = new Vector2(nodes[edge1.Item1].pos.x, nodes[edge1.Item1].pos.z);
        var B = new Vector2(nodes[edge1.Item2].pos.x, nodes[edge1.Item2].pos.z);
        var C = new Vector2(nodes[edge2.Item1].pos.x, nodes[edge2.Item1].pos.z);
        var D = new Vector2(nodes[edge2.Item2].pos.x, nodes[edge2.Item2].pos.z);
        return Intersect(A, B, C, D);
    }

    private bool IntersectAny((int, int) edge, HashSet<(int, int)> edges, List<Node> nodes)
    {
        foreach (var testEdge in edges)
        {
            if (Intersect(edge, testEdge, nodes))
            {
                return true;
            }
        }

        return false;
    }

    private void CreatePlatform(Node node)
    {
        float height = node.pos.y + 10 + 5;

        var block = Instantiate(blockPrefab, transform);

        block.transform.position = new Vector3(node.pos.x, height / 2f - 10, node.pos.z);
        block.transform.rotation = Quaternion.AngleAxis(Random.value * 360f, Vector3.up);
        block.transform.localScale = new Vector3(node.scale.x, height, node.scale.y);
    }

    private void CreateBridge(Vector3 p1, Vector3 p2)
    {
        var block = Instantiate(blockPrefab, transform);
        block.transform.position = (p1 + p2) / 2f;
        block.transform.LookAt(p2);
        block.transform.localScale = new Vector3(1.5f, 0.5f, Vector3.Distance(p1, p2));
    }

    private struct Node
    {
        public Vector3 pos;
        public Quaternion rotation;
        public Vector2 scale;
    }
}