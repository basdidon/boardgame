using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Sirenix.OdinInspector;
using Sirenix.Serialization;



public class BoardManager : NetworkBehaviour
{
    public static BoardManager Instance { get; private set; }

    public LineRenderer LineRenderer { get; set; }
    [field: SerializeField] public Material LineMaterial { get; set; }
    public Vector3 LineOffset;
    public void DrawLine(Vector3[] vertexs)
    {
        if(vertexs == null || vertexs.Length <= 0)
        {
            LineRenderer.positionCount = 0;
        }
        else
        {
            for(int i= 0; i < vertexs.Length; i++)
            {
                vertexs[i] += LineOffset;
            }

            LineRenderer.positionCount = vertexs.Length;
            LineRenderer.SetPositions(vertexs);
        }
    }

    [OdinSerialize] public bool IsFocus;
    [OdinSerialize] public Vector3Int FocusCell;
    public System.Action<Vector3Int> OnFocusChanged;

    public void SetFocusCell(Vector3Int cellPos)
    {
        FocusCell = cellPos;
        OnFocusChanged?.Invoke(cellPos);
    }

    public Dictionary<BoardObject, Vector3Int> ObjectsPosition;
    public Dictionary<Vector3Int, NetworkObject> Nodes;

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
        LineRenderer.material = LineMaterial;
        LineRenderer.startWidth = 0.2f;
        LineRenderer.positionCount = 0;

        ObjectsPosition = new();
        Nodes = new();
    }

    public bool TryGetNodeObject(Vector3Int cellPos,out NetworkObject nodeObject)
    {
        return Nodes.TryGetValue(cellPos, out nodeObject);
    }

    public bool TryGetNode(Vector3Int cellPos,out Node node)
    {
        node = null;

        if (Nodes.TryGetValue(cellPos,out NetworkObject networkObject))
            if (networkObject.TryGetBehaviour(out node))
                return true;

        return false;
    }

    public bool TryGetNodeType(Vector3Int cellPos,out NodeType nodeType)
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

    public bool TryGetObjectPosition(BoardObject boardObject,out Vector3Int cellPos)
    {
        cellPos = Vector3Int.zero;
        if (!ObjectsPosition.ContainsKey(boardObject))
            return false;

        foreach(var objectPos in ObjectsPosition)
        {
            if(objectPos.Key == boardObject)
            {
                cellPos = objectPos.Value;
                return true;
            }
        }

        return false;
    }

    public void AddObject(BoardObject boardObject,Vector3Int cellPos)
    {
        if (IsFreeTile(cellPos) && !ObjectsPosition.ContainsValue(cellPos))
        {
            ObjectsPosition.Add(boardObject, cellPos);
            boardObject.transform.position = cellPos;
        }
    }

    public void AddObjectRandomRange(BoardObject boardObject, Vector3Int a, Vector3Int b)
    {
        Vector3Int minPos = Vector3Int.zero, maxPos = Vector3Int.zero;

        // find minPos and maxPos
        if (a.x <= b.x)
        {
            minPos.x = a.x;
            maxPos.x = b.x;
        }
        else
        {
            minPos.x = b.x;
            maxPos.y = a.x;
        }

        if (a.z <= b.z)
        {
            minPos.z = a.z;
            maxPos.z = b.z;
        }
        else
        {
            minPos.z = b.z;
            maxPos.z = a.z;
        }

        // start random
        while (true)
        {
            var result = new Vector3Int(Random.Range(minPos.x, maxPos.x), 0, Random.Range(minPos.z, maxPos.z));

            if (IsFreeTile(result))
            {
                // try remove it from ObjectsPositon
                AddObject(boardObject, result);
                break;
            }
        }
    }

    public void RemoveObject(BoardObject boardObject)
    {
        ObjectsPosition.Remove(boardObject);
    }

    public bool IsFreeTile(Vector3Int cellPos)  // is This Tile exists and no object on it.
    {
        if (!Nodes.ContainsKey(cellPos))
        {
            Debug.Log("Not found cellPos");
        }

        if (ObjectsPosition.ContainsValue(cellPos)) {
            Debug.Log("Some Unit on this cellPos");
            foreach(var objectPos in ObjectsPosition)
            {
                if(objectPos.Value == cellPos)
                {
                    Debug.Log($"{objectPos.Key.name}");
                }
            }
        }

        return Nodes.ContainsKey(cellPos) && !ObjectsPosition.ContainsValue(cellPos);
    }


    /***  #### TODO ####
     *    some unit can reach cell that others unit won't.
     *    like flying unit can move to water tile or empty tile.
     * */
    public bool IsCanMoveTo(Vector3Int cellPos)
    {
        if (!IsFreeTile(cellPos))
            return false;

        if(TryGetNodeType(cellPos,out NodeType nodeType)){
            return nodeType.IsWalkable;
        }
        return false;
    }

    public bool TryDirectionalMove(Vector3Int startCell, Vector3Int direction, out Vector3Int resultCell)
    {
        resultCell = Vector3Int.zero;
        var targetPos = startCell + direction;
        if (IsCanMoveTo(targetPos))
        {
            resultCell = startCell + direction;
            return true;
        }
        return false;
    }
}
