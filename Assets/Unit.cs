using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Fusion;

[System.Serializable]
public class Unit : NetworkBehaviour
{
    LevelManager LevelManager { get { return LevelManager.Instance; } }
    // Action
    [Button]
    void EndTurn()
    {
        Debug.Log($"{GetType()} decided to endturn.");
        LevelManager.EndTurn(this);
    }

    // Event
    public void OnStartTurn()
    {
        Debug.Log($"{GetType()} OnStartTurn().");
    }

    public void OnEndTurn()
    {
        Debug.Log($"{GetType()} OnEndTurn().");
    }
}
