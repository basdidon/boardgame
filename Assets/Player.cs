using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

public class Player : Unit
{
    Inputs Inputs;

    private void OnEnable() => Inputs.Enable();
    private void OnDisable() => Inputs.Disable();

    private void Awake()
    {
        Inputs = new Inputs();
    }

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

    IEnumerator FindpathCoroutine()
    {
        bool isCanMove = false;
        bool isEndCoroutine = false;
        //var focusCell = Vector3Int.zero;
        if (!BoardManager.Instance.TryGetObjectPosition(this, out Vector3Int startPos))
        {
            Debug.LogError("not found in objects postion");
        }

        void UpdateFocusCell(Vector3Int focusCell)
        {
            if (PathFinder.TryFindPath(startPos, focusCell, out List<Vector3Int> moves))
            {
                Vector3[] path = new Vector3[moves.Count + 1];
                path[0] = LevelManager.Instance.CurrentTurn.transform.position;
                for (int i = 0; i < moves.Count; i++)
                {
                    path[i + 1] = path[i] + moves[i];
                }

                BoardManager.Instance.DrawLine(path);

                isCanMove = true;
            }
            else
            {
                BoardManager.Instance.DrawLine(null);
                isCanMove = false;
            }
        }

        void TryMove(InputAction.CallbackContext _)
        {
            if (isCanMove)
            {
                //MoveTo();
                isEndCoroutine = true;
            }
        }

        BoardManager.Instance.OnFocusChanged += UpdateFocusCell;
        Inputs.Player.LeftButton.performed += TryMove;

        yield return new WaitUntil(()=>isEndCoroutine);

        BoardManager.Instance.OnFocusChanged -= UpdateFocusCell;
        Inputs.Player.LeftButton.performed -= TryMove;
        BoardManager.Instance.DrawLine(null);
        Debug.Log("EndCoroutine");
    }

    [Button]
    public void MoveCommand()
    {
        // start pathfinding
        // until player left-click to select position
        if(LevelManager.Instance.CurrentTurn == this)
        {
            StartCoroutine(FindpathCoroutine());
        }
        else
        {
            Debug.Log("Not your turn.");
        }
    }

    public void ExecutePath(List<Vector3Int> moves)
    {
        Debug.Log($"ExecutePath: {moves}");
    }
}
