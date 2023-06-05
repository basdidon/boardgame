using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Player : Unit
{
    [Button]
    public void SetCellPosition(Vector3Int cellPos)
    {
        BoardManager.Instance.AddObject(this,cellPos);
    }

    [Button]
    public void AddObjectRandomRange(Vector3Int a,Vector3Int b)
    {
        BoardManager.Instance.AddObjectRandomRange(this, a, b);
    }
}
