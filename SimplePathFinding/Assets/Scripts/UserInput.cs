using UnityEngine;
using System.Collections;

public class UserInput : MonoBehaviour {

    private Grid grid;
    private Pathfinding pf;
    private IPathfindingListener pfVisualizer;

    private Node start = null;
    private Node target = null;

    void Awake() {
        pf = GetComponent<Pathfinding>();
        grid = GetComponent<Grid>();
        pfVisualizer = GetComponent(typeof(IPathfindingListener)) as IPathfindingListener;
    }

    void Start()
    {
        start = grid.GetNode(0, 0);
        target = grid.GetNode(grid.GridSizeX - 1, grid.GridSizeY - 1);
    }

    void Update() {

        if (Input.GetMouseButtonDown(0))
        {
            Node newStart = GetNodeFromMouse();
            if (newStart != null)
            {
                start = newStart;
                CallPathFinding();
            }
        } else if (Input.GetMouseButton(0))
        {
            Node newStart = GetNodeFromMouse();
            if (newStart != null && newStart != start)
            {
                start = newStart;
                CallPathFinding();
            }
        }


        if (Input.GetMouseButtonDown(1))
        {
            Node newTarget = GetNodeFromMouse();
            if (newTarget != null)
            {
                target = newTarget;
                CallPathFinding();
            }
        }
        else if (Input.GetMouseButton(1))
        {
            Node newTarget = GetNodeFromMouse();
            if (newTarget != null && newTarget != target)
            {
                target = newTarget;
                CallPathFinding();
            }
        }
    }

    /// <summary>
    /// Calls pathfinding if start and target nodes are valid
    /// </summary>
    private void CallPathFinding()
    {
        if (start != null && target != null && start.isPassable && target.isPassable)
        {
            float pfStarted = Time.realtimeSinceStartup;
            pf.FindPath(start.worldPosition, target.worldPosition, pfVisualizer);
            float totalTime = (Time.realtimeSinceStartup - pfStarted) * 100;
            print("Time to find path: " + totalTime + " ms");
        }
    }

    /// <summary>
    /// Get node from mouse cursor position
    /// </summary>
    /// <returns> Node which is under mouse cursor. If there is none, return null </returns>
    private Node GetNodeFromMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 20))
        {
            return grid.NodeFromWorldPoint(hit.point);
        }
        return null;
    }
}
