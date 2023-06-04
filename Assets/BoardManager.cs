using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Sirenix.OdinInspector;

public class BoardManager : NetworkBehaviour
{
    public static BoardManager Instance { get; private set; }

    public LineRenderer LineRenderer { get; set; }
    public Vector3 LineOffset;

    public Vector3Int FocusCell { get; set; }

    public static Dictionary<BoardObject, Vector3Int> ObjectsPosition;
    public static Dictionary<Vector3Int, NetworkObject> Nodes;

    private float updateFucusCellTimeElapsed;
    private float updateFucusCellTime;

    public List<Vector3Int> directionalMoves;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        LineRenderer = gameObject.AddComponent<LineRenderer>();
        LineRenderer.startColor = Color.blue;
        LineRenderer.startWidth = .1f;

        ObjectsPosition = new();
        Nodes = new();
    }

    public static bool TryGetNodeObject(Vector3Int cellPos,out NetworkObject nodeObject)
    {
        return Nodes.TryGetValue(cellPos, out nodeObject);
    }

    public static bool TryGetNode(Vector3Int cellPos,out Node node)
    {
        node = null;

        if (Nodes.TryGetValue(cellPos,out NetworkObject networkObject))
            if (networkObject.TryGetBehaviour(out node))
                return true;

        return false;
    }

    public static bool TryGetNodeType(Vector3Int cellPos,out NodeType nodeType)
    {
        nodeType = null;

        if (Nodes.TryGetValue(cellPos, out NetworkObject networkObject))
            if (networkObject.TryGetBehaviour(out Node node))
            {
                nodeType = node.NodeType;
                return true;
            }

        return false;
    }

    public static void AddObject(BoardObject boardObject,Vector3Int cellPos)
    {
        if (IsFreeTile(cellPos))
            if (!ObjectsPosition.ContainsValue(cellPos))
                ObjectsPosition.Add(boardObject, cellPos);
    }

    public static void RemoveObject()
    {

    }

    public static bool IsFreeTile(Vector3Int cellPos)  // is This Tile avaliable
    {
        return Nodes.ContainsKey(cellPos) && !ObjectsPosition.ContainsValue(cellPos);
    }

    // when you move to any direction in game, may be active some event like IceFloor that won't let you stop and keep you going in same direction,
    public bool TryDirectionalMove(Vector3Int startCell, Vector3Int direction, out Vector3Int resultCell)
    {
        resultCell = Vector3Int.zero;

        if (IsFreeTile(startCell + direction))
        {
            resultCell = startCell + direction;
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (LevelManager.Instance.CurrentTurn == null)
            return;

        if (updateFucusCellTimeElapsed > updateFucusCellTime)
        {
            // find the way from 0,0 to focus cell
            if(ObjectsPosition.TryGetValue(LevelManager.Instance.CurrentTurn,out Vector3Int value))
            {
                directionalMoves = PathFinder.FindDirectionMovePath(value, FocusCell);

                Vector3[] path = new Vector3[directionalMoves.Count + 1];
                path[0] = LevelManager.Instance.CurrentTurn.transform.position + LineOffset;
                for (int i = 0; i < directionalMoves.Count; i++)
                {
                    path[i + 1] = path[i] + directionalMoves[i];
                }

                LineRenderer.positionCount = path.Length;
                LineRenderer.SetPositions(path);
            }

            updateFucusCellTimeElapsed = 0f;
        }
        updateFucusCellTimeElapsed += Time.deltaTime;
    }
}
