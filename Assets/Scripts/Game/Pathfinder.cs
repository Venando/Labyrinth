using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private Tilemap wallsTilemap;

    private const int MAX_NODES = 4096;             
    private readonly Node[] nodes = new Node[MAX_NODES];
    private readonly int[] openSet = new int[MAX_NODES];
    private int openCount;

    private readonly List<Vector3Int> pathBuffer = new List<Vector3Int>(256);

    private static readonly Vector3Int[] dirs = new Vector3Int[4] {
        new Vector3Int(0,1),
        new Vector3Int(1,0),
        new Vector3Int(0,-1),
        new Vector3Int(-1,0)
    };

    private struct Node
    {
        public int X, Y;
        public int G;
        public int H;
        public int F => G + H;
        public int Parent;
        public bool Used;
        public bool Closed;

        public void Set(int x, int y, int g, int h, int parent)
        {
            X = x;
            Y = y;
            G = g;
            H = h;
            Parent = parent;
            Used = true;
            Closed = false;
        }
    }

    // Tilemap check
    public bool IsWalkable(Vector3 worldPosition)
    {
        Vector3Int c = wallsTilemap.WorldToCell(worldPosition);
        return wallsTilemap.GetTile(c) == null;
    }

    public bool IsWalkableCell(int x, int y)
    {
        return wallsTilemap.GetTile(new Vector3Int(x, y, 0)) == null;
    }

    public bool TryToCalculatePath(Vector3Int start, Vector3Int end, int maxSteps, out List<Vector3Int> path)
    {
        path = null;

        // Reset nodes
        for (int i = 0; i < MAX_NODES; i++)
            nodes[i].Used = nodes[i].Closed = false;

        openCount = 0;

        // Put start node into openSet
        int startIndex = GetIndex(start.x, start.y);
        nodes[startIndex].Set(start.x, start.y, 0, Heuristic(start, end), -1);
        openSet[openCount++] = startIndex;

        int steps = 0;

        while (openCount > 0 && steps < maxSteps)
        {
            steps++;

            int bestIndex = openSet[0];
            int bestSlot = 0;
            int bestF = nodes[bestIndex].F;

            for (int i = 1; i < openCount; i++)
            {
                int idx = openSet[i];
                int f = nodes[idx].F;
                if (f < bestF)
                {
                    bestIndex = idx;
                    bestF = f;
                    bestSlot = i;
                }
            }

            // Remove chosen node from open
            openSet[bestSlot] = openSet[--openCount];
            nodes[bestIndex].Closed = true;

            // If reached target: reconstruct path
            if (bestIndex == GetIndex(end.x, end.y))
            {
                BuildPath(bestIndex, out path);
                return true;
            }

            // Expand neighbors
            Node bestNode = nodes[bestIndex];

            for (int d = 0; d < 4; d++)
            {
                int nx = bestNode.X + dirs[d].x;
                int ny = bestNode.Y + dirs[d].y;

                if (!IsWalkableCell(nx, ny))
                    continue;

                int ni = GetIndex(nx, ny);

                if (nodes[ni].Closed)
                    continue;

                int tentativeG = bestNode.G + 1;

                if (!nodes[ni].Used)
                {
                    nodes[ni].Set(nx, ny, tentativeG, Heuristic(new Vector3Int(nx, ny), end), bestIndex);
                    openSet[openCount++] = ni;
                }
                else if (tentativeG < nodes[ni].G)
                {
                    nodes[ni].G = tentativeG;
                    nodes[ni].Parent = bestIndex;
                }
            }
        }

        return false;
    }

    private void BuildPath(int endIndex, out List<Vector3Int> path)
    {
        pathBuffer.Clear();

        int i = endIndex;
        while (i != -1)
        {
            Node n = nodes[i];
            pathBuffer.Add(new Vector3Int(n.X, n.Y));
            i = n.Parent;
        }

        pathBuffer.Reverse();
        path = pathBuffer;
    }

    private static int Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static int GetIndex(int x, int y)
    {
        // Convert signed coords into 0..4095 range
        return ((x & 63) << 6) | (y & 63);
    }
}
