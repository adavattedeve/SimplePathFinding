using UnityEngine;

public class Node {
    public int gridX, gridY;
    public bool isPassable;
    public Vector3 worldPosition;
    public Node(int gridX, int gridY, Vector3 worldPosition, bool isPassable)
    {
        this.gridX = gridX;
        this.gridY = gridY;
        this.worldPosition = worldPosition;
        this.isPassable = isPassable;
    }
}
