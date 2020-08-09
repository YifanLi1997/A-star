using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [System.Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }

    Node[,] grid;

    public bool displayGrid = false;
    public LayerMask unwalkableLayerMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public TerrainType[] walkableRegions;

    LayerMask walkableMask;
    Dictionary<int, int> walkableRegionsDict = new Dictionary<int, int>();
    float nodeDiameter;
    int gridSizeX, gridSizeY; // the number of nodes along the X,Y axis of the grid

    public int maxHeapSize{
        get { return gridSizeX * gridSizeY; }
    }

    [Header("Visualization")]
    public Transform player;

    private void Awake()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        foreach (TerrainType region in walkableRegions)
        {
            walkableMask.value |= region.terrainMask.value;
            walkableRegionsDict.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
        }

        nodeDiameter = 2 * nodeRadius;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft =
            transform.position -
            Vector3.right * gridWorldSize.x / 2 -
            Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint =
                    worldBottomLeft +
                    Vector3.right * (x * nodeDiameter + nodeRadius) +
                    Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableLayerMask);

                int penalty = 0;
                // raycast
                if (walkable) {
                    Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100, walkableMask)) {
                        walkableRegionsDict.TryGetValue(hit.collider.gameObject.layer, out penalty);
                    }
                }

                grid[x, y] = new Node(walkable, worldPoint, x, y, penalty);
            }
        }

    }

    public List<Node> getNeighbors(Node node) {
        List<Node> neighbors = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // do not add itself
                if (x == 0 && y == 0) continue;

                // mind the boundry of the grid
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }



    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null && displayGrid)
        {
            Node playerNode = NodeFromWorldPoint(player.position);
            foreach (var n in grid) {
                //Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(0, 10, n.movementPenalty));
                //Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                if (playerNode == n) Gizmos.color = Color.cyan;
                Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter));
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        // percent ranges in [0, 1]
        // worldPos ranges in [-gridWorldSize.x / 2, gridWorldSize.x / 2]
        // thus we add (gridWorldSize.x / 2) to remap
        float percentX = Mathf.Clamp01((worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPos.z + gridWorldSize.y / 2) / gridWorldSize.y); // be careful with y and z

        // x ranges in [0, gridSizeX - 1]
        // (percentX * gridSizeX) ranges in [0, gridSizeX]
        int x = Mathf.FloorToInt(Mathf.Min(percentX * gridSizeX, gridSizeX - 1));
        int y = Mathf.FloorToInt(Mathf.Min(percentY * gridSizeY, gridSizeY - 1));

        return grid[x, y];
    }
}
