using UnityEngine;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour {
    [SerializeField]
    private int heuristicWeight = 1;

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

    /// <summary>
    /// Finds shortest path from start position to target position. 
    /// </summary>
    /// <param name="start"> Start location in world coordinates </param>
    /// <param name="target"> Target location in world coordinates </param>
    /// <param name="pfListener"> Lets us send pathfinding data every step. Used for visualization </param>
    /// <returns> Array of nodes which are the path from start to target. Returns null if path is not found </returns>
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
                    neighbours[i].hCost = GetDistance(neighbours[i], targetNode) * heuristicWeight;
                    neighbours[i].parent = currentNode;

                    if (!openSet.Contains(neighbours[i]))
                    {
                        openSet.Add(neighbours[i]);
                    } else
                    {
                        openSet.SortUp(neighbours[i]); //Node's cost is lowered so it has to be sorted up in the heap.
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

    /// <summary>
    /// Forms the path by retracing from endNode to startNode
    /// </summary>
    /// <param name="startNode"> Path's starting node </param>
    /// <param name="endNode"> Path's ending node </param>
    /// <returns> Path that is formed through retracing </returns>
    private Node[] RetracePath(Node startNode, Node endNode)
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
    /// Returns distance between given nodes. Calculated distance is in it's 
    /// own unit system where:
    /// 10 is distance between two nodes that are side by side and
    /// 14 is distance between two nodes that are adjacent diagonally.
    /// </summary>
    /// <param name="nodeA"> From </param>
    /// <param name="nodeB"> To </param>
    /// <returns> Distance between given nodes. </returns>
	private int GetDistance(Node nodeA, Node nodeB)
    {
        int dX = Math.Abs(nodeA.gridX - nodeB.gridX);
        int dY = Math.Abs(nodeA.gridY - nodeB.gridY);
        return 14 * Math.Min(dX, dY) + 10 * Math.Abs(dX - dY);
    }

    /// <summary>
    /// Draws latest calculated path and other data used in pathfinding process.
    /// </summary>
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
