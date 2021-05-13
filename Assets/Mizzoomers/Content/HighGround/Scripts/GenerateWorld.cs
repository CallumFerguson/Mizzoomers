using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerateWorld : NetworkBehaviour
{
    public GameObject blockPrefab;

    [SyncVar(hook = nameof(SetSeed))] private int _seed;

    private void SetSeed(int oldSeed, int newSeed)
    {
        CreateWorld();
    }

    [Server]
    public void ServerSetSeed(int seed)
    {
        var oldSeed = _seed;
        _seed = seed;
        if (!isClient)
        {
            SetSeed(oldSeed, _seed);
        }
    }
    
    [Server]
    public void ServerSetSeed()
    {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int cur_time = (int) (System.DateTime.UtcNow - epochStart).TotalSeconds;
        ServerSetSeed(cur_time);
    }

    public override void OnStopClient()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public override void OnStopServer()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void CreateWorld()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        Random.InitState(_seed);
        print("Generating world with seed: " + _seed);

        const float size = 45f;
        const float space = 7f;
        const float distanceSlopeRatio = 0.15f;

        var nodes = new List<Node>();
        var height = new Vector2(size, size).magnitude * distanceSlopeRatio + 1.75f;
        // var height = 0;

        //start platform
        nodes.Add(new Node() {pos = new Vector3(-6, 0, -6), rotation = Quaternion.identity, scale = new Vector2(8, 8)});

        //end platform
        nodes.Add(new Node() {pos = new Vector3(size, height, size), rotation = Quaternion.identity, scale = new Vector2(2, 2)});

        for (int i = 0; i < 100; i++)
        {
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
                var nodeHeight = pos.magnitude * distanceSlopeRatio + (Random.value * 1f);
                // var nodeHeight = 0f;
                node.pos = new Vector3(pos.x, nodeHeight, pos.y);
                node.rotation = Quaternion.AngleAxis(Random.value * 360f, Vector3.up);
                node.scale = new Vector2(Random.value * 5 + 1, Random.value * 5 + 1);

                var rotatedPos = node.pos - new Vector3(size / 2f, 0, size / 2f);
                rotatedPos = Quaternion.AngleAxis(-45f, Vector3.up) * rotatedPos;
                if (Mathf.Abs(rotatedPos.x) < size / 3f)
                {
                    nodes.Add(node);
                }
            }
        }

        // print($"Created {nodes.Count} nodes.");

        var everyEdgeSorted = new SortedDictionary<float, (int, int)>();
        for (int i = 0; i < nodes.Count; i++)
        {
            for (int n = i + 1; n < nodes.Count; n++)
            {
                var distance = Vector3.Distance(nodes[i].pos, nodes[n].pos);
                while (everyEdgeSorted.ContainsKey(distance))
                {
                    distance += 0.001f;
                }

                everyEdgeSorted.Add(distance, (i, n));
            }
        }

        // print($"Every edge lengh: {everyEdgeSorted.Count}");

        var edges = new HashSet<(int, int)>();

        foreach (var pair in everyEdgeSorted)
        {
            var edge = pair.Value;
            var fill = FloodFillGraph(edge.Item1, edges);
            if (fill.Count == nodes.Count)
            {
                break;
            }

            if (!fill.Contains(edge.Item2))
            {
                edges.Add(pair.Value);
            }
        }

        var lowPriorityEdges = new HashSet<(int, int)>();
        int edgesToAdd = Mathf.FloorToInt(nodes.Count / 1.0f);
        int maxEdgeToTry = nodes.Count * 4;
        foreach (var edge in everyEdgeSorted)
        {
            maxEdgeToTry--;
            if (maxEdgeToTry == -1)
            {
                break;
            }

            if (Random.value < 0.5f)
            {
                if (!IntersectAny(edge.Value, edges, nodes))
                {
                    // edges.Add(edge.Value);
                    lowPriorityEdges.Add(edge.Value);
                    edgesToAdd--;
                    if (edgesToAdd == 0)
                    {
                        break;
                    }
                }
            }
        }

        // print($"Created {edges.Count} edges.");

        var closestNodes = new SortedDictionary<float, int>();
        
        for (int i = 1; i < nodes.Count; i++)
        {
            CreatePlatform(nodes[i], i.ToString(), i > 1);
            if (i > 1)
            {
                var distance = Vector3.Distance(nodes[i].pos, nodes[0].pos);
                while (closestNodes.ContainsKey(distance))
                {
                    distance += 0.001f;
                }

                closestNodes.Add(distance, i);
            }
        }

        var numToAdd = 5;
        foreach (var pair in closestNodes)
        {
            numToAdd--;
            if (numToAdd == -1)
            {
                break;
            }

            if (!lowPriorityEdges.Contains((0, pair.Value)))
            {
                lowPriorityEdges.Add((0, pair.Value));
            }
        }

        // CreateBridge(new Vector3(-2, 5 - 0.25f, -2), nodes[closestIndex].pos + new Vector3(0, 5, 0));

        foreach (var edge in edges)
        {
            CreateEdge(edge, nodes, false);
        }

        foreach (var edge in lowPriorityEdges)
        {
            CreateEdge(edge, nodes, true);
        }
    }

    private void CreateEdge((int, int) edge, List<Node> nodes, bool checkHitBox)
    {
        var n1 = nodes[edge.Item1];
        var n2 = nodes[edge.Item2];
        var n1Sides = NodeSides(n1);
        var n2Sides = NodeSides(n2);
        var closestDistance = float.PositiveInfinity;
        var closestPair = (-1, -1);
        for (int i = 0; i < n1Sides.Length; i++)
        {
            for (int n = 0; n < n2Sides.Length; n++)
            {
                var distance = Vector3.Distance(n1Sides[i], n2Sides[n]);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPair = (i, n);
                }
            }
        }

        CreateBridge(n1Sides[closestPair.Item1], n2Sides[closestPair.Item2], $"{edge.Item1} to {edge.Item2}", checkHitBox);
    }

    private Vector3[] NodeSides(Node node)
    {
        var points = new Vector3[4];
        points[0] = node.pos + new Vector3(0, 5, 0) + node.rotation * Vector3.forward * node.scale.y / 2f;
        points[1] = node.pos + new Vector3(0, 5, 0) + node.rotation * Vector3.back * node.scale.y / 2f;
        points[2] = node.pos + new Vector3(0, 5, 0) + node.rotation * Vector3.right * node.scale.x / 2f;
        points[3] = node.pos + new Vector3(0, 5, 0) + node.rotation * Vector3.left * node.scale.x / 2f;
        return points;
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

    private HashSet<int> FloodFillGraph(int from, HashSet<(int, int)> edges)
    {
        var found = new HashSet<int>();
        var queued = new HashSet<int>();
        var queue = new Queue<int>();
        queue.Enqueue(from);
        queued.Add(from);
        while (queue.Count > 0)
        {
            int next = queue.Dequeue();
            found.Add(next);
            foreach (var edge in edges)
            {
                if (edge.Item1 == next || edge.Item2 == next)
                {
                    if (edge.Item1 != next && !found.Contains(edge.Item1) && !queued.Contains(edge.Item1))
                    {
                        queue.Enqueue(edge.Item1);
                        queued.Add(edge.Item1);
                    }

                    if (edge.Item2 != next && !found.Contains(edge.Item2) && !queued.Contains(edge.Item2))
                    {
                        queue.Enqueue(edge.Item2);
                        queued.Add(edge.Item2);
                    }
                }
            }
        }

        return found;
    }

    //from: https://stackoverflow.com/questions/3838329/how-can-i-check-if-two-segments-intersect
    private bool ccw(Vector2 A, Vector2 B, Vector2 C)
    {
        return (C.y - A.y) * (B.x - A.x) > (B.y - A.y) * (C.x - A.x);
    }

    //Return true if line segments AB and CD intersect
    private bool Intersect(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
    {
        A = Vector2.MoveTowards(A, B, 0.1f);
        B = Vector2.MoveTowards(B, A, 0.1f);
        C = Vector2.MoveTowards(C, D, 0.1f);
        D = Vector2.MoveTowards(D, C, 0.1f);
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

    private void CreatePlatform(Node node, string nodeName, bool canHover)
    {
        float height = node.pos.y + 10 + 5;

        var block = Instantiate(blockPrefab, transform);
        block.name = nodeName;

        block.transform.position = new Vector3(node.pos.x, height / 2f - 10, node.pos.z);
        block.transform.rotation = node.rotation;
        block.transform.localScale = new Vector3(node.scale.x, height, node.scale.y);

        block.AddComponent<BoxCollider>();
        if (Random.value < 0.15f && canHover)
        {
            block.AddComponent<Hover>();
        }
    }

    private void CreateBridge(Vector3 p1, Vector3 p2, string bridgeName, bool checkHitBox)
    {
        var flatDistance = Vector2.Distance(new Vector2(p1.x, p1.z), new Vector2(p2.x, p2.z));
        var heightDistance = Mathf.Abs(p1.y - p2.y);

        const float maxPlatformSpacingFlat = 3f;
        const float maxPlatformSpacingHeight = 0.8f;

        if (flatDistance < maxPlatformSpacingFlat && heightDistance < maxPlatformSpacingHeight)
        {
            return;
        }

        int numPlatformsFlat = Mathf.FloorToInt(flatDistance / maxPlatformSpacingFlat);
        int numPlatformsHeight = Mathf.FloorToInt(heightDistance / maxPlatformSpacingHeight);

        int numPlatforms = Mathf.Max(numPlatformsFlat, numPlatformsHeight);
        numPlatforms = Mathf.Max(1, numPlatforms);

        for (int i = 0; i < numPlatforms; i++)
        {
            var size = Random.value + 1f;
            var rotation = Quaternion.AngleAxis(Random.value * 360f, Vector3.up);
            var t = (i + 1f) / (numPlatforms + 1f);
            var pos = Vector3.Lerp(p1, p2, t);
            var scale = new Vector3(size, 10f + pos.y, size);
            var position = new Vector3(pos.x, -scale.y / 2f + pos.y, pos.z);
            position += new Vector3(0, Random.value * 0.01f, 0);

            var hit = checkHitBox && Physics.CheckBox(position, scale / 2f, rotation);

            if (!hit)
            {
                var block = Instantiate(blockPrefab, position, rotation, transform);
                block.name = bridgeName;
                block.transform.localScale = scale;
                block.AddComponent<BoxCollider>();
                if (Random.value < 0.15f)
                {
                    block.AddComponent<Hover>();
                }
            }
        }
    }

    private struct Node
    {
        public Vector3 pos;
        public Quaternion rotation;
        public Vector2 scale;
    }
}