using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PathfindingVisualizer : MonoBehaviour, IPathfindingListener {

    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private Material targetMat, startMat, currentMat, openSetMat, closedSetMat, pathMat;
    [SerializeField]
    private float stepTime = 0.15f;

    private Grid grid;
    private List<GameObject> tilePool;
    private List<GameObject> tiles;

    //Pathfinding data
    private List<PathfindingStepData> pfStepData;
    private Node startNode, targetNode;
    //private HashSet<Node> allNodes;
    private Node[] path;

    private int poolSize = 150;
    private float timer = 0;

    void Awake()
    {
        //Load mats from resources
        tilePool = new List<GameObject>();
        tiles = new List<GameObject>();
        pfStepData = new List<PathfindingStepData>();

        for (int i = 0; i < poolSize; ++i)
        {
            AddTileToPool(Instantiate(tilePrefab) as GameObject);
        }

    }

    void Update()
    {

    }

    public void OnStart(Node startNode, Node targetNode)
    {
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

    private void VisualizeStart()
    {
        while (tiles.Count > 0)
        {
            AddTileToPool(tiles[0]);
        }

        pfStepData.Clear();

        GameObject temp = TileFromPool(startNode.worldPosition);
        temp.GetComponent<MeshRenderer>().sharedMaterial = startMat;

        temp = TileFromPool(targetNode.worldPosition);
        temp.GetComponent<MeshRenderer>().sharedMaterial = targetMat;
        //Pool all the old tiles
        //Start && target
    }

    private void VisualizeStep()
    {
        //old current node -> closed Node
        //new current node
        //Instantiate new Open nodes
        //Some animation for old open nodes??
    }

    private void VisualizePath()
    {

    }

    private GameObject TileFromPool(Vector3 position)
    {
        GameObject tile;
        if (tilePool.Count == 0)
        {
            tile = Instantiate(tilePrefab) as GameObject;
        } else
        {            
            tile = tilePool[0];
            tilePool.RemoveAt(0);
        }

        tile.SetActive(true);
        tile.transform.position = position;
        tiles.Add(tile);

        return tile;
    }

    private void AddTileToPool(GameObject tile)
    {
        tile.SetActive(false);
        tiles.Remove(tile);
        tilePool.Add(tile);
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
