using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Sirenix.OdinInspector;

public class Node : NetworkBehaviour
{
    public MeshRenderer MeshRenderer { get; private set; }
    [SerializeField] Vector3Int CellPosition;

    private void Awake()
    {
        MeshRenderer = GetComponentInChildren<MeshRenderer>();

        if (MeshRenderer == null)
            Debug.Log("NO MeshRenderer in Children");

    }

    public void SetCellPosition(Vector3Int cellPos)
    {
        Debug.Log($"SetCellPosition()");
        CellPosition = cellPos;

        var texture = ProceduralMapGenerator.Instance.GetBiomeByCellPos(CellPosition).GetTexture2D();
        MeshRenderer.material.mainTexture = texture;
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