using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

public class Player : Unit
{
    Inputs Inputs;
    [SerializeField][Range(1f,10f)] float moveSpeed = 2f;

    private void OnEnable() => Inputs.Enable();
    private void OnDisable() => Inputs.Disable();

    private void Awake()
    {
        Inputs = new Inputs();
        Inputs.Player.MoveCommand.performed += _ => MoveCommand();
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
        List<Vector3Int> moves = new();
        
        if(BoardManager.IsFocus)
            UpdateFocusCell(BoardManager.FocusCell);

        void UpdateFocusCell(Vector3Int focusCell)
        {
            if (PathFinder.TryFindPath(CellPosition, focusCell, out moves))
            {
                Vector3[] path = new Vector3[moves.Count + 1];
                path[0] = CellPosition;
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
                StartCoroutine(ExecutePath(new Queue<Vector3Int>(moves)));
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
            Debug.Log("MoveCommand was called, waiting for input ........................");
        }
        else
        {
            Debug.Log("Not your turn.");
        }
    }


    public IEnumerator ExecutePath(Queue<Vector3Int> movesQueue)
    {
        if (movesQueue.TryDequeue(out Vector3Int dir))
        {
            Debug.Log($"ExecutePath: {dir}");
            Vector3 start = WorldPosition;
            CellPosition += dir;
            Vector3 des = WorldPosition;

            transform.LookAt(des);
            float LerpDuration = 1/moveSpeed;
            float timeElapsed = 0f;

            while (timeElapsed < LerpDuration)
            {
                transform.position = Vector3.Lerp(start, des, timeElapsed / LerpDuration);
                yield return null;
                timeElapsed += Time.deltaTime;
            }

            transform.position = des;
            yield return ExecutePath(movesQueue);
        }

        yield return null;
        BoardManager.Instance.DrawLine(null);
    }
}
