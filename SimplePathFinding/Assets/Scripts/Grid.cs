using UnityEngine;
using System.Collections;

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

    void Awake()
    {
        nodeRadius = nodeDiameter / 2;
    }

    void Start () {
        CreateGrid();
	}
	
	void Update () {
	
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
                Gizmos.color = (n.isPassable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition + Vector3.up * nodeSize / 2,
                                Vector3.one * nodeSize);
            }
        }
    }

}
