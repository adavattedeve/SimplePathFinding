using UnityEngine;
using System.Collections;

public class PathfindingTest : MonoBehaviour {
    [SerializeField]
    private Transform start;
    [SerializeField]
    private Transform target;

    private Node[] path;
    private Pathfinding pf;
    private PathfindingVisualizer pfVisualizer;

    void Start()
    {
        pf = GetComponent<Pathfinding>();
        pfVisualizer = GetComponent<PathfindingVisualizer>();
    }

    void Update()
    {
        if (start != null && target != null)
        {
            path = pf.FindPath(start.position, target.position, pfVisualizer);
        }
    }

}
