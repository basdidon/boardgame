using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Sirenix.OdinInspector;

public class Node : NetworkBehaviour
{
    public MeshRenderer MeshRenderer { get; private set; }
    [SerializeField] Vector3Int CellPosition;
    [SerializeField] NodeType NodeType;

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

        NodeType ??= ProceduralMapGenerator.Instance.GetBiomeByCellPos(CellPosition).GetNodeType();
        MeshRenderer.material.mainTexture = NodeType.GetTexture2D();
    }

    private void OnMouseEnter()
    {
        Debug.Log($"OnMouseEnter() on {CellPosition}");
        MeshRenderer.material.SetFloat("_IsFocus",1);
    }

    private void OnMouseExit()
    {
        MeshRenderer.material.SetFloat("_IsFocus", 0);
    }
}