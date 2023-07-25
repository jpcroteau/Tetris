using System;
using System.Collections;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public enum Type
    {
        I,
        J,
        L,
        O,
        S,
        T,
        Z
    }

    [SerializeField] private Type type;
    
    [SerializeField, ColorUsage(true, true)] private Color color;

    private bool canMove = true;
    private float previousTime = 0f;

    private TetrisGrid grid;
    private Action<Tetromino> stopCallback = null;
    
    private bool positionLocked = false;

    private void OnValidate()
    {
        SetColor(color);
    }

    private void Awake()
    {
        SetColor(color);
    }
    
    private void Update()
    {
        if (canMove)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                // fallDelay = 0.5f;
                canMove = false;
                positionLocked = true;
            }

            // // Move the tetromino left
            // if (Input.GetKeyDown(KeyCode.LeftArrow))
            // {
            //     Move(Vector2Int.left);
            // }
            // // Move the tetromino right
            // else if (Input.GetKeyDown(KeyCode.RightArrow))
            // {
            //     Move(Vector2Int.right);
            // }
            // // Rotate the tetromino clockwise
            // else if (Input.GetKeyDown(KeyCode.UpArrow))
            // {
            //     Rotate();
            // }
            //
            // // Move the tetromino downwards faster when the player holds the down arrow key
            // if (Input.GetKey(KeyCode.DownArrow))
            // {
            //     if (Time.time - previousTime >= fallSpeed / 4f)
            //     {
            //         Move(Vector2Int.down);
            //         previousTime = Time.time;
            //     }
            // }
        }
    }

    public void Spawn(TetrisGrid grid, Action<Tetromino> stopCallback, int x, int y)
    {
        this.grid = grid;
        this.stopCallback = stopCallback;
        
        SetTetrominoAt(grid.GetPositionAtIndex(x, y), transform.rotation);

        //StartCoroutine(FallCoroutine());
        StartCoroutine(MoveSidewaysCoroutine(0.2f));
    }

    #region Movement
    
    private bool Move(Vector2Int direction)
    {
        var nextPosition = transform.position + new Vector3(direction.x, direction.y, 0);
        return SetTetrominoAt(nextPosition, transform.rotation);
    }

    private bool Rotate()
    {
        if (type == Type.O) return true;
        
        var wantedRotation = transform.rotation * Quaternion.Euler(0f, 0f, -90f);
        return SetTetrominoAt(transform.position, wantedRotation);
    }

    private IEnumerator MoveSidewaysCoroutine(float delay)
    {
        var movement = Vector2Int.right;
        while (!positionLocked)
        {
            yield return new WaitForSeconds(delay);
            
            if (positionLocked) break;
            
            if (!Move(movement))
            {
                movement = -movement;
            }
        }

        StartCoroutine(FallCoroutine(0.01f));
    }

    private IEnumerator FallCoroutine(float delay)
    {
        var fall = true;
        while (fall)
        {
            yield return new WaitForSeconds(delay);

            if (!Move(Vector2Int.down))
            {
                fall = false;
                stopCallback?.Invoke(this);
            }
        }
    }

    #endregion

    #region Position

    // Function to check if the current tetromino position is valid in the grid
    private bool IsValidGridPosition()
    {
        foreach (Transform child in transform)
        {
            Vector2Int index = grid.GetIndexAtPosition(child.position);
            if (!grid.IsInsideGrid(index))
            {
                return false;
            }
            
            // above grid is valid
            if ( index.y >= grid.GridHeight )
            {
                continue;
            }

            var blockOnGrid = grid.GetGridElement(index.x, index.y);
            if (blockOnGrid != null && blockOnGrid.parent != transform)
            {
                return false;
            }
        }
        
        return true;
    }
    
    #endregion
    
    #region Grid

    private bool SetTetrominoAt(Vector3 wantedPosition, Quaternion wantedRotation)
    {
        bool isValid = true;
        
        // First, clear the current position of the tetromino from the grid
        foreach (Transform child in transform)
        {
            Vector2Int index = grid.GetIndexAtPosition(child.position);
            StoreBlockInGrid(index, null);
        }
        
        var previousRotation = transform.rotation;
        var previousPosition = transform.position;
        
        transform.position = wantedPosition;
        transform.rotation = wantedRotation;

        if (!IsValidGridPosition())
        {
            transform.position = previousPosition;
            transform.rotation = previousRotation;
            isValid = false;
        }

        // Then, store the new position of the tetromino in the grid
        foreach (Transform child in transform)
        {
            Vector2Int index = grid.GetIndexAtPosition(child.position);
            StoreBlockInGrid(index, child);
        }

        return isValid;
    }
    
    private void StoreBlockInGrid(Vector2Int index, Transform block)
    {
        // above grid is valid
        if ( index.y >= grid.GridHeight ) return;
        grid.SetGridElement(index.x, index.y, block);
    }

    #endregion

    #region Utils

    private void SetColor(Color color)
    {
        var pb = new MaterialPropertyBlock();
        pb.SetColor("_BaseColor", color);
        
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            r.SetPropertyBlock(pb);
        }
    }

    #endregion
}
