using UnityEngine;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour {

    private Grid grid;

    private Node startNode;
    private Node targetNode;
    private BinaryHeap<Node> openSet;
    private Node[] finalPath;
    private HashSet<Node> closedSet;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    public Node[] FindPath(Vector3 start, Vector3 target, IPathfindingListener pfListener = null)
    {
        bool isPFListened = pfListener != null;
        bool pathSuccess = false;

        startNode = grid.NodeFromWorldPoint(start);
        targetNode = grid.NodeFromWorldPoint(target);

        if (startNode == null || !startNode.isPassable)
        {
            print("Invalid start node");
            return null;
        }

        if (targetNode == null || !targetNode.isPassable)
        {
            print("Invalid target node");
            return null;
        }

        if (isPFListened)
        {
            pfListener.OnStart(startNode, targetNode);
        }

        openSet = new BinaryHeap<Node>(grid.MaxNodes);
        closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
     
            Node currentNode = openSet.First();
            closedSet.Add(currentNode);
            

            if (currentNode == targetNode)
            {
                pathSuccess = true;
                break;
            }

            HashSet<Node> newOpenNodes = null; //This is used for PathfindingListener to gather step data
            if (isPFListened)
            {
                newOpenNodes = new HashSet<Node>();
            }

            List<Node> neighbours = grid.GetNeighbours(currentNode);
            for (int i = 0; i < neighbours.Count; ++i)
            {
                if (!neighbours[i].isPassable || closedSet.Contains(neighbours[i]))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbours[i]);

                if (newMovementCostToNeighbour < neighbours[i].gCost || !openSet.Contains(neighbours[i]))
                {
                    neighbours[i].gCost = newMovementCostToNeighbour;
                    neighbours[i].hCost = GetDistance(neighbours[i], targetNode) * 1;//GetDistance(neighbours[i], targetNode);
                    neighbours[i].parent = currentNode;

                    if (!openSet.Contains(neighbours[i]))
                    {
                        openSet.Add(neighbours[i]);
                    } else
                    {
                        openSet.UpdateItem(neighbours[i]);
                    }

                    if (isPFListened)
                    {
                        newOpenNodes.Add(neighbours[i]);
                    }
                }
            }

            if (isPFListened)
            {
                pfListener.OnCurrentNodeChange(currentNode, newOpenNodes);
            }

        }
        Node[] path = null;
        if (pathSuccess)
        {
            path = RetracePath(startNode, targetNode);
        }

        if (isPFListened)
        {
            pfListener.OnPathFound(path);
        }

        return path;
    }

    Node[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();

        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Node[] wayPoints = path.ToArray();
        Array.Reverse(wayPoints);
        finalPath = wayPoints;
        return wayPoints;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nodeA"></param>
    /// <param name="nodeB"></param>
    /// <returns></returns>
	private int GetDistance(Node nodeA, Node nodeB)
    {
        int dX = Math.Abs(nodeA.gridX - nodeB.gridX);
        int dY = Math.Abs(nodeA.gridY - nodeB.gridY);
        return 14 * Math.Min(dX, dY) + 10 * Math.Abs(dX - dY);
    }

    void OnDrawGizmos()
    {
        if (startNode == null || targetNode == null)
        {
            return;
        }
        float nodeSize = grid.NodeDiameter - 0.1f;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < openSet.Count; ++i)
        {
            Gizmos.DrawCube(openSet.items[i].worldPosition + Vector3.up * nodeSize / 2,
                                Vector3.one * nodeSize);
        }

        Gizmos.color = Color.blue;
        foreach (Node n in closedSet) 
        {
            Gizmos.DrawCube(n.worldPosition + Vector3.up * nodeSize / 2,
                               Vector3.one * nodeSize);
        }

        Gizmos.color = Color.magenta;
        for (int i = 0; i < finalPath.Length; ++i)
        {
            Gizmos.DrawCube(finalPath[i].worldPosition + Vector3.up * nodeSize / 2,
                               Vector3.one * nodeSize);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawCube(startNode.worldPosition + Vector3.up * nodeSize / 2,
                                Vector3.one * nodeSize);

        Gizmos.color = Color.red;
        Gizmos.DrawCube(targetNode.worldPosition + Vector3.up * nodeSize / 2,
                                Vector3.one * nodeSize);
    }
}
