using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    public static BoardManager BoardManager { get { return BoardManager.Instance; } }

    struct Node
    {
        public Vector3Int CellPosition { get; set; }
        public List<Vector3Int> directionMoveToNode; // like, go left, left, up, left
        public int G { get; set; }  // cumulative cost to this node, In this context mean ActionPoint
        public float H { get; set; }  // cost to targetCell,that ignore all obstacles
        public float F => G + H;

        public Node(Vector3Int cellPosition, int g, Vector3Int targetCell)
        {
            CellPosition = cellPosition;
            directionMoveToNode = new List<Vector3Int>();
            G = g;
            H = Mathf.Abs(cellPosition.x - targetCell.x) + Mathf.Abs(cellPosition.y - targetCell.y);
        }

        public Node(Vector3Int cellPosition, int g, Vector3Int targetCell, List<Vector3Int> dirs)
        {
            CellPosition = cellPosition;
            directionMoveToNode = dirs;
            G = g;
            H = Mathf.Abs(cellPosition.x - targetCell.x) + Mathf.Abs(cellPosition.y - targetCell.y);
        }
    }

    public static bool TryFindPath(Vector3Int startCell,Vector3Int targetCell, out List<Vector3Int> directionMoves)
    {
        directionMoves = new();

        if (!BoardManager.IsFreeTile(targetCell))
        {
            Debug.Log("target can't reach");
            return false;
        }

        Node startNode = new (startCell, 0, targetCell);
        var toSearch = new List<Node>() { startNode };
        var processed = new List<Node>();

        List<Vector3Int> directions = new() { Vector3Int.forward, Vector3Int.back, Vector3Int.left, Vector3Int.right };

        while (toSearch.Count > 0)
        {
            Node currentNode = toSearch[0];
            foreach (var t in toSearch)
                if (t.F < currentNode.F || t.F == currentNode.F && t.H < currentNode.H)
                    currentNode = t;

            processed.Add(currentNode);
            toSearch.Remove(currentNode);

            foreach (var direction in directions)
            {
                if (BoardManager.Instance.TryDirectionalMove(currentNode.CellPosition, direction, out Vector3Int resultCell))
                {

                    var newNodePath = new List<Vector3Int>(currentNode.directionMoveToNode) { direction };
                    if (processed.Exists(e => e.CellPosition == resultCell))
                    {
                        Node processedNode = processed.Find(e => e.CellPosition == resultCell);

                        // if new path use cost less than old node ,update that node
                        processedNode.G = currentNode.G + 1 < processedNode.G ? currentNode.G : processedNode.G;
                        processedNode.directionMoveToNode = newNodePath;
                    }
                    else
                    {
                        if (currentNode.CellPosition + direction == targetCell)
                        {
                            Debug.Log("found");
                            directionMoves = newNodePath;
                            return true;
                        }
                        // add new node
                        toSearch.Add(new Node(currentNode.CellPosition + direction, currentNode.G + 1, targetCell, newNodePath));
                    }
                }
            }
        }

        return false;
    }

    /*
    public static List<Vector3Int> FindDirectionMovePath(Vector3Int startCell, Vector3Int targetCell)
    {
        var directionMoves = new List<Vector3Int>();
        if (!BoardManager.IsFreeTile(targetCell))
        {
            Debug.Log("target can't reach");
            return directionMoves;
        }

        Node startNode = new Node(startCell, 0, targetCell);
        var toSearch = new List<Node>() { startNode };
        var processed = new List<Node>();

        //var searchCount = 0;

        while (toSearch.Count > 0)
        {
            //Debug.Log(searchCount++ + "toSearch left:" + toSearch.Count);
            Node currentNode = toSearch[0];
            foreach (var t in toSearch)
                if (t.F < currentNode.F || t.F == currentNode.F && t.H < currentNode.H)
                    currentNode = t;

            processed.Add(currentNode);
            toSearch.Remove(currentNode);

            List<Vector3Int> directions = new() { Vector3Int.forward, Vector3Int.back, Vector3Int.left, Vector3Int.right };

            foreach (var direction in directions)
            {
                if (BoardManager.Instance.TryDirectionalMove(currentNode.CellPosition, direction, out Vector3Int resultCell))
                {

                    var newNodePath = new List<Vector3Int>(currentNode.directionMoveToNode) { direction };
                    if (processed.Exists(e => e.CellPosition == resultCell))
                    {
                        Node processedNode = processed.Find(e => e.CellPosition == resultCell);
                        //Debug.Log("update old node");

                        // if new path use cost less than old node ,update that node
                        processedNode.G = currentNode.G + 1 < processedNode.G ? currentNode.G : processedNode.G;
                        processedNode.directionMoveToNode = newNodePath;
                    }
                    else
                    {
                        if (currentNode.CellPosition + direction == targetCell)
                        {
                            Debug.Log("found");
                            return newNodePath;
                        }
                        // add new node
                        toSearch.Add(new Node(currentNode.CellPosition + direction, currentNode.G + 1, targetCell, newNodePath));
                        //Debug.Log("add node toSearch");
                    }
                }
                else
                {
                    Debug.Log("cant move to " + currentNode.CellPosition + direction);
                }
            }
        }

        return directionMoves;
    }
    */
}
