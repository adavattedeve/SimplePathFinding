using System.Collections.Generic;

public interface IPathfindingListener {
    void OnStart(Node start, Node target);
    void OnCurrentNodeChange(Node currentNode, ICollection<Node> newOpenSetNodes);
    void OnPathFound(Node[] path);
}