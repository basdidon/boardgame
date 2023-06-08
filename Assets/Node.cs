using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Sirenix.OdinInspector;

public class Node : NetworkBehaviour
{
    public MeshRenderer MeshRenderer { get; private set; }
    public Vector3Int CellPosition { get; set; }
    public NodeType NodeType { get; set; }
    public Unit Unit { get; set; }

    private void Awake()
    {
        MeshRenderer = GetComponentInChildren<MeshRenderer>();

        if (MeshRenderer == null)
            Debug.Log("NO MeshRenderer in Children");

    }

    public void SetUpNode(Vector3Int cellPos)
    {
        Debug.Log($"SetUpNode()");
        CellPosition = cellPos;

        foreach(var nodePreset in ProceduralMapGenerator.Instance.NodePresets)
        {
            if(nodePreset.TryGetValue(CellPosition,out NodeType nodeType))
            {
                NodeType = nodeType;
            }
        }

        if(NodeType == null)
        {
            if (ProceduralMapGenerator.NodeTypes.TryGetValue(CellPosition,out NodeType _nodeType))
            {
                NodeType = _nodeType;
            }
            else
            {
                Debug.LogError($"can't get value from NodeTypes");
            }
        }

        MeshRenderer.material.mainTexture = NodeType.GetTexture2D();
    }

    private void OnMouseEnter() 
    {
        MeshRenderer.material.SetFloat("_IsFocus", 1);
        BoardManager.Instance.IsFocus = true;
        BoardManager.Instance.SetFocusCell(CellPosition);
    }
    private void OnMouseExit() { 
        MeshRenderer.material.SetFloat("_IsFocus", 0);
        BoardManager.Instance.IsFocus = false;
    }

    public bool AssignUnit(Unit unit)
    {
        if(Unit == null)
        {
            Unit = unit;
            Unit.transform.SetParent(transform);
            Unit.transform.localPosition = Vector3.zero + Vector3.up * 0.5f + unit.offset;
            return true;
        }
        else
        {
            return false;
        }
    }
}