using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

    [SerializeField]
    private int gridSizeX = 20, gridSizeY = 20;
    [SerializeField]
    private float nodeDiameter = 1.0f; 
    [SerializeField]
    private Vector3 gridLBCorner;
    [SerializeField]
    private bool displayGridGizmos = true;
    [SerializeField]
    private LayerMask unwalkableMask;

    private Node[,] grid;

    private float unWalkableSphereRadius = 0.95f;
    private float nodeRadius;

    public int MaxNodes
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    public float NodeDiameter
    {
        get
        {
            return nodeDiameter;
        }
    }

    public int GridSizeX
    {
        get
        {
            return gridSizeX;
        }
    }

    public int GridSizeY
    {
        get
        {
            return gridSizeY;
        }
    }

    void Awake()
    {
        nodeRadius = nodeDiameter / 2;
        CreateGrid();
    }

    public void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                Vector3 worldPoint = gridLBCorner;
                worldPoint += Vector3.right * (x * nodeDiameter + nodeRadius) + 
                              Vector3.forward * (y * nodeDiameter + nodeRadius);

                bool isPassable = !(Physics.CheckSphere(worldPoint, nodeRadius * unWalkableSphereRadius, unwalkableMask));
                grid[x, y] = new Node(x, y, worldPoint, isPassable);
            }
        }

    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = node.gridX - 1; x <= node.gridX + 1; x++)
        {
            for (int y = node.gridY - 1; y <= node.gridY + 1; y++)
            {
                if (x == node.gridX && y == node.gridY)
                {
                    continue;
                }

                Node n = GetNode(x, y);
                if (n != null)
                {
                    neighbours.Add(grid[x, y]);

                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x - gridLBCorner.x / (nodeRadius * 2));
        int y = Mathf.FloorToInt(worldPosition.z - gridLBCorner.z / (nodeRadius * 2));

        return GetNode(x, y);
    }

    public Node GetNode(int x, int y)
    {
        if (x < gridSizeX && y < gridSizeY && y >= 0 && x >= 0)
        {
            return grid[x, y];
        }

        return null;
    }

    void OnDrawGizmos()
    {
        if (grid != null && displayGridGizmos)
        {
            Gizmos.color = Color.blue;
            Vector3 gridSize = new Vector3(gridSizeX * nodeDiameter, 1, gridSizeY * nodeDiameter);
            Vector3 gridCenter = gridLBCorner + gridSize / 2;

            Gizmos.DrawWireCube(gridCenter, gridSize);
            foreach (Node n in grid)
            {
                float nodeSize = (nodeDiameter - 0.1f);
                Gizmos.color = (n.isPassable) ? Color.white : Color.black;
                Gizmos.DrawCube(n.worldPosition + Vector3.up * nodeSize / 2,
                                Vector3.one * nodeSize);
            }
        }
    }

}
