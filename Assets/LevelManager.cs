using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Fusion;

public enum TeamTurn
{
    Player,
    Enemy,
}

public class LevelManager : NetworkBehaviour
{
    static LevelManager instance;
    public static LevelManager Instance {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("LevelManager");
                go.AddComponent<LevelManager>();
            }

            return instance; 
        } 
    }
    [Networked] public TeamTurn TeamTurn { get; private set; }

    Unit currentTurn;
    [Networked] public Unit CurrentTurn { 
        get 
        { 
            return currentTurn; 
        } 
        set 
        {
            CurrentTurn.OnEndTurn();
            currentTurn = value;
            CurrentTurn.OnStartTurn();
        } 
    }

    //public List<Player> Players { get; set; }
    //public List<Enemy> Enemies { get; set; }
    public List<Unit> Units { get; set; }

    IEnumerator UnitEnumerator { get; set; }

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // when everyone ready
    [Button]
    public void StartLevel()
    {
        //Players = new List<Player>(FindObjectsOfType<Player>());
        //Enemies = new List<Enemy>(FindObjectsOfType<Enemy>());

        Units = new List<Unit>();
        Units.AddRange(FindObjectsOfType<Player>());
        Units.AddRange(FindObjectsOfType<Enemy>());

        StartNextTurn();
    }

    public void EndTurn(Unit unit)
    {
        if(CurrentTurn == unit)
        {
            StartNextTurn();
        }
    }

    void StartNextTurn()
    {
        Debug.Log("StartNextTurn()");
        if (UnitEnumerator != null && UnitEnumerator.MoveNext())
        {
            Debug.Log("MoveNext");
            CurrentTurn = (Unit) UnitEnumerator.Current;

            if (CurrentTurn == null)
                StartNextTurn();
        }
        else
        {
            Debug.Log($"ResetUnitEnumerator() => {Units.Count} Unit(s)");

            if(Units.Count == 0)
            {
                Debug.LogError("Units can't be empty.");
                return;
            }
            else
            {
                UnitEnumerator = Units.GetEnumerator();
                StartNextTurn();
            }
        }
    }

    // 
    public void AddUnit(Unit unit)
    {
        if(unit is Player)
        {
            // add to the end of players
            int lastPlayerIndex = Units.FindLastIndex((unit) => unit is Player );
            Units.Insert(lastPlayerIndex+1, unit);
        }
        else  // assume it as enemy
        {
            Units.Add(unit);
        }
    }

    public void RemoveUnit(Unit unit)
    {
        Units.Remove(unit);
    }
}