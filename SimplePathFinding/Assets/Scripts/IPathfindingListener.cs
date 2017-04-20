using System.Collections.Generic;

/// <summary>
/// Interface for other classes to listen pathfinding process step by step
/// </summary>
public interface IPathfindingListener {

    /// <summary>
    /// Called when pathfinding process starts
    /// </summary>
    /// <param name="start"> Starting node </param>
    /// <param name="target"> Target node </param>
    void OnStart(Node start, Node target);

    /// <summary>
    /// Called when pathfinder changes current node and adds new nodes to open set
    /// </summary>
    /// <param name="currentNode"> New current node </param>
    /// <param name="newOpenSetNodes"> New open set nodes </param>
    void OnCurrentNodeChange(Node currentNode, ICollection<Node> newOpenSetNodes);

    /// <summary>
    /// Called when path is found
    /// </summary>
    /// <param name="path"> Final path </param>
    void OnPathFound(Node[] path);
}