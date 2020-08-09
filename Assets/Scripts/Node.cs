using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node:IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPos;
    public int gridX;
    public int gridY;
    public int heapIndex;
    public int movementPenalty;

    public Node parent;

    public int gCost;
    public int hCost;

    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    public int HeapIndex {
        get { return heapIndex; }
        set { heapIndex = value; }
    }

    public Node(bool _walkable, Vector3 _worldPos, int _x, int _y, int _penalty)
    {
        walkable = _walkable;
        worldPos = _worldPos;
        gridX = _x;
        gridY = _y;
        movementPenalty = _penalty;
    }

    public int CompareTo(Node obj)
    {
        int result = obj.fCost.CompareTo(fCost);
        if (result == 0)
        {
            result = obj.hCost.CompareTo(hCost);
        }
        return result;
    }
}
