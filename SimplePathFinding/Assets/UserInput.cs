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
	
        if (Input.GetMouseButton(0))
        {
            Node newStart = GetNodeFromMouse();
            if (newStart != null && newStart != start)
            {
                start = newStart;
                CallPathFinding();
            }
        }

        if (Input.GetMouseButton(1))
        {
            Node newTarget = GetNodeFromMouse();
            if (newTarget != null && newTarget != target)
            {
                target = newTarget;
                CallPathFinding();
            }
        }
    }

    private void CallPathFinding()
    {
        if (start != null && target != null)
        {
            pf.FindPath(start.worldPosition, target.worldPosition, pfVisualizer);
        }
    }

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
