using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BoardObject : NetworkBehaviour
{
    protected BoardManager BoardManager { get { return BoardManager.Instance; } }
    public Vector3Int CellPosition
    {
        get
        {
            if (!BoardManager.TryGetObjectPosition(this, out Vector3Int startPos))
            {
                Debug.LogError("not found in objects postion");
            }

            return startPos;
        }

        set
        {
            if(BoardManager.IsCanMoveTo(value))
                BoardManager.ObjectsPosition[this] = value;
        }
    }

    public Vector3 WorldPosition
    {
        get
        {
            return BoardManager.getWorldPosition(CellPosition);
        }
    }
}
