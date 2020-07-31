using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class PathFinding : MonoBehaviour
{
    #region Pseudo Code
    /* 1. create two data sets:
     *      1) OPEN: the nodes that are to be evaluated
     *               require to add, remove, contain, search
     *      2) CLOSED: the nodes that have been evaluated
     *               require to add, contain
     * 2. add start node to Open
     * 3. loop
     *      1) current node = node in OPEN with the lowest f cost
     *      2) remove current from OPEN
     *      3) add current to CLOSED
     *      
     *      4) RETURN current if current is the target node
     *
     *      5) foreach neighbor node of current
     *          i. if neighbor is not walkable or neighbor is in CLOSED
     *                  skip to the next neighbor
     *
     *          ii. if neighbor is not in OPEN or new g cost is smaller (new shorter path to neighbor)
     *                  set new f cost to the node
     *                  set current as the parent of neighbor
     *                  if neighbor is not in OPEN
     *                      add neighbor to OPEN
     */
    #endregion

    Grid grid;

    public Transform seeker;
    public Transform target;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            grid.path = FindPath(seeker.position, target.position);
        }
    }

    List<Node> FindPath(Vector3 startPos, Vector3 targetPos) {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        var startNode = grid.NodeFromWorldPoint(startPos);
        var targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        //Heap<Node> openSet = new Heap<Node>(grid.maxHeapSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0) {
            // find the lowest fCost - later will be optimized with heap
            var currentNode = openSet[0];
            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            //Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                var path = RetracePath(startNode, currentNode);
                sw.Stop();
                Debug.Log("Path found in " + sw.ElapsedMilliseconds + " ms.");
                return path;
            }

            foreach (var neighbor in grid.getNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        Debug.LogWarning("No path found!");
        return null;
    }

    List<Node> RetracePath(Node start, Node end) {
        List<Node> path = new List<Node>();
        Node current = end;

        while (current != start)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    int GetDistance(Node a, Node b) {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);

        return 14 * Mathf.Min(dstX, dstY) + 10 * Mathf.Abs(dstX - dstY);
    }

}
