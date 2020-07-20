using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid: MonoBehaviour
{
    Node[,] grid;

    public LayerMask unwalkableLayerMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;

    float nodeDiameter;
    int gridSizeX, gridSizeY; // the number of nodes along the X,Y axis of the grid

    [Header("Visualization")]
    public Transform player;

    private void Start()
    { 
        CreateGrid();
    }

    private void CreateGrid()
    {
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
                grid[x, y] = new Node(walkable, worldPoint);
            }
        }

    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            Node playerNode = NodeFromWorldPoint(player.position);
            foreach (var n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                if (playerNode == n)
                {
                    Gizmos.color = Color.cyan;
                }

                Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter - 0.1f));
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

        return grid[x,y];
    }
}
