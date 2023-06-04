using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Fusion;

[System.Serializable]
public class Unit : BoardObject
{
    LevelManager LevelManager { get { return LevelManager.Instance; } }
    [SerializeField] public Vector3 offset;
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
