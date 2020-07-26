using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPos;
    public int gridX;
    public int gridY;

    public Node parent;

    public int gCost;
    public int hCost;

    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    public Node(bool _walkable, Vector3 _worldPos, int _x, int _y)
    {
        walkable = _walkable;
        worldPos = _worldPos;
        gridX = _x;
        gridY = _y;
    }
}
