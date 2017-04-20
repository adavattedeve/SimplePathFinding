using UnityEngine;

public class Node : IHeapItem<Node>
{
    public Node parent; //From which node pathfinder arrived to this node
    public Vector3 worldPosition;

    public int gridX, gridY;
    public int gCost;
    public int hCost;
    public bool isPassable;

    private int heapIndex;

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }
    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    /// <summary>
    /// Constructor for Node
    /// </summary>
    /// <param name="gridX"> X position as index in the grid </param>
    /// <param name="gridY"> Y position as index in the grid </param>
    /// <param name="worldPosition"> Position in world coordinates </param>
    /// <param name="isPassable"> Is this node passable </param>
    public Node(int gridX, int gridY, Vector3 worldPosition, bool isPassable)
    {
        this.gridX = gridX;
        this.gridY = gridY;
        this.worldPosition = worldPosition;
        this.isPassable = isPassable;
    }

    /// <summary>
    /// Compares F cost with other node. If it equals -> compare hCost instead
    /// </summary>
    /// <param name="other"> Node that this node is compared against </param>
    /// <returns> this < other = -1, this == other = 0, this < other = 1 </other> </returns>
    public int CompareTo(Node other)
    {
        int compare = FCost.CompareTo(other.FCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }
        return compare;
    }
}
