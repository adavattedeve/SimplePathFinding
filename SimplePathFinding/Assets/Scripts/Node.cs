using UnityEngine;

public class Node : IHeapItem<Node>
{

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

    public Node(int gridX, int gridY, Vector3 worldPosition, bool isPassable)
    {
        this.gridX = gridX;
        this.gridY = gridY;
        this.worldPosition = worldPosition;
        this.isPassable = isPassable;
    }

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
