using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathfindingVisualizer : MonoBehaviour, IPathfindingListener {

    private bool animate = true;

    [SerializeField]
    private float stepTime = 0.05f;

    private GameObject tilePrefab;
    private Material gridMat;
    private Material targetMat, startMat, currentMat, openSetMat, closedSetMat, pathMat, unPassableMat;
    

    private Grid grid;
    private List<Tile> tilePool;
    private List<Tile> tiles;
    private GameObject tileParent;

    private HashSet<Tile> unPassables;
    private GameObject unPassableParent;

    //Pathfinding data
    private List<PathfindingStepData> pfStepData;
    private Node startNode, targetNode;
    private Node[] path;

    private int pfStepIndex = 0;
    private int poolSize = 150;

    void Awake()
    {
        //Load materials and tile prefab from resources
        targetMat = Resources.Load<Material>("Materials/Target");
        startMat = Resources.Load<Material>("Materials/Start");
        currentMat = Resources.Load<Material>("Materials/Current");
        openSetMat = Resources.Load<Material>("Materials/OpenSet");
        closedSetMat = Resources.Load<Material>("Materials/ClosedSet");
        pathMat = Resources.Load<Material>("Materials/Path");
        unPassableMat = Resources.Load<Material>("Materials/UnPassable");

        gridMat = Resources.Load<Material>("Materials/Grid");
        tilePrefab = Resources.Load<GameObject>("TilePrefab");

        grid = GetComponent<Grid>();

        tilePool = new List<Tile>();
        tiles = new List<Tile>();
        pfStepData = new List<PathfindingStepData>();
        tileParent = new GameObject("Tiles");

        unPassables = new HashSet<Tile>();
        unPassableParent = new GameObject("UnPassableTiles");
        //Instantiate base amount of tile game objects to pool
        for (int i = 0; i < poolSize; ++i)
        {
            GameObject tile = Instantiate(tilePrefab) as GameObject;
            tile.transform.SetParent(tileParent.transform);
            AddTileToPool(tile.GetComponent<Tile>());
        }

    }

    
    void Start()
    {
        CreateGridVisualization();
    }

    public void SetAnimate(bool value)
    {
        animate = value;
    }

    #region IPathfindingListener methods
    public void OnStart(Node startNode, Node targetNode)
    {
        pfStepData.Clear();
        StopAllCoroutines();

        this.startNode = startNode;
        this.targetNode = targetNode;
    }

    public void OnCurrentNodeChange(Node currentNode, ICollection<Node> newOpenSetNodes)
    {
        pfStepData.Add(new PathfindingStepData(currentNode, newOpenSetNodes));
    }

    public void OnPathFound(Node[] path)
    {
        this.path = path;
        VisualizeStart();
    }
    #endregion

    #region Visualization methods

    private void CreateGridVisualization()
    {
        GameObject gridGO = GameObject.CreatePrimitive(PrimitiveType.Plane);
        gridGO.name = "GridGameObject";
        int tilesInPlane = 10;
        float edgeWidthInTexture = 0.1f;
        float baseScaleX = grid.GridSizeX / tilesInPlane;
        float baseScaleY = grid.GridSizeY / tilesInPlane;

        float additionalScale = edgeWidthInTexture / tilesInPlane;
        gridGO.transform.localScale = new Vector3(baseScaleX + additionalScale, 1, baseScaleY + additionalScale);
        gridMat.mainTextureScale = new Vector2(grid.GridSizeX + edgeWidthInTexture, grid.GridSizeY + edgeWidthInTexture);
        gridMat.mainTextureOffset = new Vector2(-edgeWidthInTexture / 2, -edgeWidthInTexture / 2);
        gridGO.GetComponent<MeshRenderer>().sharedMaterial = gridMat;

        Vector3 gridDiagonal = grid.GetNode(grid.GridSizeX - 1, grid.GridSizeY - 1).worldPosition - grid.GetNode(0, 0).worldPosition;
        gridGO.transform.position = grid.GetNode(0, 0).worldPosition + gridDiagonal / 2 + Vector3.down * 0.1f;

        //Destroy old unPassables
        foreach (Tile t in unPassables)
        {
            Destroy(t.gameObject);
        }
        unPassables.Clear();

        //Create unpassable tiles
        for (int y = 0; y < grid.GridSizeY; ++y)
        {
            for (int x = 0; x < grid.GridSizeX; ++x)
            {
                if (!grid.GetNode(x, y).isPassable)
                {
                    Tile tile = (Instantiate(tilePrefab) as GameObject).GetComponent<Tile>();
                    tile.transform.SetParent(unPassableParent.transform);
                    tile.transform.position = grid.GetNode(x, y).worldPosition;
                    tile.Material = unPassableMat;
                    unPassables.Add(tile);
                }
            }
        }

        //Set camera to fit the grid
        Camera.main.orthographicSize = grid.NodeDiameter * grid.GridSizeY / 2;
    }

    /// <summary>
    /// Starts the pathfinding visualization
    /// </summary>
    private void VisualizeStart()
    {
        //Pool all tiles used in earlier visualization
        while (tiles.Count > 0)
        {
            AddTileToPool(tiles[0]);
        }

        pfStepIndex = 0;

        Tile temp = GetTileFromPool(startNode.worldPosition);
        temp.Material = startMat;

        temp = GetTileFromPool(targetNode.worldPosition);
        temp.Material = targetMat;
        StartCoroutine(VisualizeSteps());
    }

    /// <summary>
    /// Visualizes pathfinding step by step. Waits stepTime amount of time between steps
    /// </summary>
    /// <returns></returns>
    private IEnumerator VisualizeSteps()
    {
        Tile startTile = GetTileAt(startNode.worldPosition);
        Tile targetTile = GetTileAt(targetNode.worldPosition);
        Tile currentTile = null;

        while (pfStepIndex < pfStepData.Count)
        {
            if (currentTile != null)
            {
                currentTile.Material = closedSetMat;
            }
            

            foreach (Node n in pfStepData[pfStepIndex].OpenNodes)
            {
                Tile tile = GetTileAt(n.worldPosition);

                if (tile == null)
                {
                    tile = GetTileFromPool(n.worldPosition);
                }
                tile.Material = openSetMat;
            }

            currentTile = GetTileAt(pfStepData[pfStepIndex].CurrentNode.worldPosition);
            currentTile.Material = currentMat;

            startTile.Material = startMat;
            targetTile.Material = targetMat;

            ++pfStepIndex;
            if (animate)
            {
                yield return new WaitForSeconds(stepTime);
            }
        }

        if (currentTile != null)
        {
            currentTile.Material = closedSetMat;
        }
        StartCoroutine(VisualizePath());
    }

    /// <summary>
    /// Visualizes the final path step by step
    /// </summary>
    /// <returns></returns>
    private IEnumerator VisualizePath()
    {
        if (path == null)
        {
            yield break;
        }
            
        for (int i = 0; i < path.Length - 1; ++i)
        {
            Tile tile = GetTileAt(path[i].worldPosition);
            tile.Material = pathMat;
            if (animate)
            {
                yield return new WaitForSeconds(stepTime);
            }
        }
    }
    #endregion

    #region Pooling methods
    /// <summary>
    /// Gets tile from pool and moves it to given position
    /// </summary>
    /// <param name="position"> Position in world coordinates where the returned tile should be moved </param>
    /// <returns> Tile from pool or new tile if the pool is empty </returns>
    private Tile GetTileFromPool(Vector3 position)
    {
        Tile tile;
        if (tilePool.Count == 0)
        {
            tile = (Instantiate(tilePrefab) as GameObject).GetComponent<Tile>();
            tile.transform.SetParent(tileParent.transform);
        }
        else
        {            
            tile = tilePool[0];
            tilePool.RemoveAt(0);
        }

        tile.gameObject.SetActive(true);
        tile.transform.position = position;
        tiles.Add(tile);

        return tile;
    }

    /// <summary>
    /// Adds new tile to the pool
    /// </summary>
    /// <param name="tile"> Tile to be added </param>
    private void AddTileToPool(Tile tile)
    {
        tile.gameObject.SetActive(false);
        tiles.Remove(tile);
        tilePool.Add(tile);
    }
    #endregion

    /// <summary>
    /// Returns tile at given position
    /// </summary>
    /// <param name="position"> Position in world coordinates where the wanted tile is </param>
    /// <returns> Tile that is found from given position else null </returns>
    private Tile GetTileAt(Vector3 position)
    {
        Ray ray = new Ray(position + Vector3.up * 10, -Vector3.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 20))
        {
            return hit.collider.GetComponent<Tile>();
        }

        return null;
    }

    /// <summary>
    /// Container for relevant data from one step in pathfinding process
    /// </summary>
    private struct PathfindingStepData
    {
        private Node currentNode;
        private ICollection<Node> openNodes;

        public Node CurrentNode 
        {
            get
            {
                return currentNode;
            }
        }

        public ICollection<Node> OpenNodes
        {
            get
            {
                return openNodes;
            }
        }

        public PathfindingStepData(Node currentNode, ICollection<Node> openNodes)
        {
            this.currentNode = currentNode;
            this.openNodes = openNodes;
        }
    }
}
