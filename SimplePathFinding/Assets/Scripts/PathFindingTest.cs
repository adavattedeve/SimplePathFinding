using UnityEngine;
using System.Collections;

public class PathFindingTest : MonoBehaviour {
    [SerializeField]
    private Transform start;
    [SerializeField]
    private Transform target;

    private Node[] path;
    private Grid grid;
    private PathFinding pf;

    void Start()
    {
        grid = GetComponent<Grid>();
        pf = GetComponent<PathFinding>();        
    }

    void Update()
    {
        if (start != null && target != null)
        {
            path = pf.FindPath(start.position, target.position);
        }
    }

}
