using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

    [SerializeField]
    private Texture2D gridLayout;
    private int gridSizeX, gridSizeY;
    [SerializeField]
    private bool displayGridGizmos = true;

    private Node[,] grid;
    private Vector3 gridPosCenter = Vector3.zero;

    private float nodeDiameter = 1.0f;
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

    public Vector3 GridPositionCenter
    {
        get
        {
            return gridPosCenter;
        }
    }

    void Awake()
    {
        nodeRadius = nodeDiameter / 2;
        CreateGrid(gridLayout);
    }

    /// <summary>
    /// Create new grid from texture where white pixels are passable nodes and other unpassable.
    /// </summary>
    /// <param name="layout"></param>
    public void CreateGrid(Texture2D layout)
    {
        Color[] pixels = layout.GetPixels();

        gridSizeX = layout.width;
        gridSizeY = layout.height;

        grid = new Node[gridSizeX, gridSizeY];
        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                Vector3 worldPoint = gridPosCenter;
                worldPoint += Vector3.right * ((x - gridSizeX / 2) * nodeDiameter + nodeRadius) +
                              Vector3.forward * ((y - gridSizeY / 2) * nodeDiameter + nodeRadius);

                bool isPassable = 
                    pixels[y * gridSizeX + x].r > 0.9f &&
                    pixels[y * gridSizeX + x].g > 0.9f &&
                    pixels[y * gridSizeX + x].b > 0.9f;

                grid[x, y] = new Node(x, y, worldPoint, isPassable);
            }
        }

    }

    /// <summary>
    /// Searches for all the neighbour nodes of the given node from grid
    /// </summary>
    /// <param name="node"> Node that's neighbours will be searched </param>
    /// <returns> Returns list of all the found neighbours </returns>
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

    /// <summary>
    /// Returns node if it is found from given world coordinates
    /// </summary>
    /// <param name="worldPosition"> World coordinates </param>
    /// <returns> Returns node if it is found from given coordinates. Else returns null </returns>
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        Vector3 translatedPos = worldPosition - gridPosCenter;
        int x = Mathf.FloorToInt(translatedPos.x / nodeDiameter + (float)gridSizeX / 2);
        int y = Mathf.FloorToInt(translatedPos.z / nodeDiameter + (float)gridSizeY / 2);

        return GetNode(x, y);
    }

    /// <summary>
    /// Returns node from grid at x, y index
    /// </summary>
    /// <param name="x"> Index x </param>
    /// <param name="y"> Index y </param>
    /// <returns> Node from grid at x, y index. Return null if either or both indexes are out of range </returns>
    public Node GetNode(int x, int y)
    {
        if (x < gridSizeX && y < gridSizeY && y >= 0 && x >= 0)
        {
            return grid[x, y];
        }

        return null;
    }

    /// <summary>
    /// Draws grid with gizmos if displayGridGizmos is true
    /// </summary>
    void OnDrawGizmos()
    {
        if (grid != null && displayGridGizmos)
        {
            Gizmos.color = Color.blue;
            Vector3 gridSize = new Vector3(gridSizeX * nodeDiameter, 1, gridSizeY * nodeDiameter);

            Gizmos.DrawWireCube(gridPosCenter, gridSize);
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
