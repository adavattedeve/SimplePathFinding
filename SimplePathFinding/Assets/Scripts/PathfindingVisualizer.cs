using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PathfindingVisualizer : MonoBehaviour, IPathfindingListener {

    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private Material gridMat;
    [SerializeField]
    private Material targetMat, startMat, currentMat, openSetMat, closedSetMat, pathMat;
    [SerializeField]
    private float stepTime = 0.05f;

    private Grid grid;
    private List<Tile> tilePool;
    private List<Tile> tiles;
    private GameObject tileParent;

    //Pathfinding data
    private List<PathfindingStepData> pfStepData;
    private Node startNode, targetNode;
    private Node[] path;

    private int pfStepIndex = 0;
    private int poolSize = 150;
    private float timer = 0;

    void Awake()
    {
        grid = GetComponent<Grid>();
        //Create grid game object for visualization
        GameObject gridGO = GameObject.CreatePrimitive(PrimitiveType.Plane);
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

        //Load mats from resources
        tilePool = new List<Tile>();
        tiles = new List<Tile>();
        pfStepData = new List<PathfindingStepData>();
        tileParent = new GameObject("Tiles");
        for (int i = 0; i < poolSize; ++i)
        {
            GameObject tile = Instantiate(tilePrefab) as GameObject;
            tile.transform.SetParent(tileParent.transform);
            AddTileToPool(tile.GetComponent<Tile>());
        }

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

    #region visualization methods
    private void VisualizeStart()
    {
        while (tiles.Count > 0)
        {
            AddTileToPool(tiles[0]);
        }

        pfStepIndex = 0;

        Tile temp = TileFromPool(startNode.worldPosition);
        temp.Material = startMat;

        temp = TileFromPool(targetNode.worldPosition);
        temp.Material = targetMat;
        StartCoroutine(VisualizeSteps());
    }

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
                    tile = TileFromPool(n.worldPosition);
                }
                tile.Material = openSetMat;
            }

            currentTile = GetTileAt(pfStepData[pfStepIndex].CurrentNode.worldPosition);
            currentTile.Material = currentMat;

            startTile.Material = startMat;
            targetTile.Material = targetMat;

            ++pfStepIndex;
            yield return new WaitForSeconds(stepTime);
        }
        currentTile.Material = closedSetMat;
        StartCoroutine(VisualizePath());
    }

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
            yield return new WaitForSeconds(stepTime);
        }
    }
    #endregion

    #region pooling methods
    private Tile TileFromPool(Vector3 position)
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

    private void AddTileToPool(Tile tile)
    {
        tile.gameObject.SetActive(false);
        tiles.Remove(tile);
        tilePool.Add(tile);
    }
    #endregion

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
