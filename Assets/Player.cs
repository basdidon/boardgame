using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Player : Unit
{
    [Button]
    public void SetCellPosition(Vector3Int cellPos)
    {
        BoardManager.AddObject( this,cellPos);
    }
}
