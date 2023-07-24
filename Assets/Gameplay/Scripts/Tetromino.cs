using System.Collections;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    [SerializeField, ColorUsage(true, true)]
    private Color color;

    [SerializeField] 
    private float fallSpeed = 1f; // The speed at which the tetromino falls
    
    private bool canMove = true;
    private float previousTime = 0f;
    
    private void OnValidate()
    {
        SetColor(color);
    }

    private void Awake()
    {
        SetColor(color);
    }
    
    private void Start()
    {
        // Spawn the tetromino at a specific position on the grid
        SpawnAtGridPosition(new Vector2Int(5, 10)); // Example position (x=5, y=20)

        // Start the coroutine for the falling behavior
        StartCoroutine(FallRoutine());
    }
    
    private void Update()
    {
        if (canMove)
        {
            // Move the tetromino left
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Move(Vector2Int.left);
            }
            // Move the tetromino right
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Move(Vector2Int.right);
            }
            // Rotate the tetromino clockwise
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Rotate();
            }

            // Move the tetromino downwards faster when the player holds the down arrow key
            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (Time.time - previousTime >= fallSpeed / 4f)
                {
                    Move(Vector2Int.down);
                    previousTime = Time.time;
                }
            }
        }
    }
    
    // Function to move the tetromino
    private bool Move(Vector2Int direction)
    {
        // Move the tetromino by the given direction
        transform.position += new Vector3(direction.x, direction.y, 0);
        bool isValid = IsValidGridPosition();
        
        // Check if the new position is valid before moving
        if (isValid)
        {
            // If the new position is valid, update the tetromino's position in the grid
            UpdateTetrominoInGrid();
        }
        else
        {
            // If the new position is not valid, revert the move
            transform.position -= new Vector3(direction.x, direction.y, 0);
        }
        
        return isValid;
    }

    // Function to rotate the tetromino clockwise
    private bool Rotate()
    {
        // Rotate the tetromino
        transform.Rotate(0, 0, -90);

        bool isValid = IsValidGridPosition();
        
        // Check if the new rotation is valid before rotating
        if (isValid)
        {
            // If the new rotation is valid, update the tetromino's position in the grid
            UpdateTetrominoInGrid();
        }
        else
        {
            // If the new rotation is not valid, revert the rotation
            transform.Rotate(0, 0, 90);
        }
        
        return isValid;
    }

    // Coroutine for the falling behavior
    private IEnumerator FallRoutine()
    {
        while (canMove)
        {
            yield return new WaitForSeconds(fallSpeed);

            // Move the tetromino down
            if (!Move(Vector2Int.down))
            {
                canMove = false;
            }
        }
    }

    private void SetBlockGrid(Vector2Int index, Transform block)
    {
        // above grid is valid
        if ( index.y >= TetrisGrid.gridHeight ) return;
        TetrisGrid.grid[index.x, index.y] = block;
    }
    
    private Vector3 GetGridPosition(Vector2Int index)
    {
        return TetrisGrid.Instance.transform.position + new Vector3(index.x, index.y, 0);
    }
    
    private Vector2Int GetGridIndex(Vector3 position)
    {
        position -= TetrisGrid.Instance.transform.position;
        return TetrisGrid.RoundVector(position);
    }

    // Function to update the tetromino's position in the grid after a successful move
    private void UpdateTetrominoInGrid()
    {
        // First, clear the current position of the tetromino from the grid
        foreach (Transform child in transform)
        {
            Vector2Int index = GetGridIndex(child.position);
            SetBlockGrid(index, null);
        }

        // Then, store the new position of the tetromino in the grid
        StoreTetrominoInGrid();
    }

    // Function to spawn the tetromino at a specific position on the grid
    public void SpawnAtGridPosition(Vector2Int gridPosition)
    {
        // Snap the tetromino to the specified grid position
        var previousPosition = transform.position;
        transform.position = GetGridPosition(gridPosition);

        // Check if the grid position is valid before placing the tetromino
        if (IsValidGridPosition())
        {
            // Store the tetromino in the grid
            StoreTetrominoInGrid();
        }
        else
        {
            // The position is invalid, so destroy the tetromino or handle it accordingly
            //Destroy(gameObject);
            transform.position = previousPosition;
        }
    }

    // Function to check if the current tetromino position is valid in the grid
    private bool IsValidGridPosition()
    {
        foreach (Transform child in transform)
        {
            Vector2Int index = GetGridIndex(child.position);
            if (!TetrisGrid.IsInsideGrid(index))
            {
                return false;
            }
            
            // above grid is valid
            if ( index.y >= TetrisGrid.gridHeight )
            {
                return true;
            }

            var blockOnGrid = TetrisGrid.grid[index.x, index.y];
            if (blockOnGrid != null && blockOnGrid.parent != transform)
            {
                return false;
            }
        }
        return true;
    }

    // Function to store the tetromino in the grid
    private void StoreTetrominoInGrid()
    {
        foreach (Transform child in transform)
        {
            Vector2Int index = GetGridIndex(child.position);
            SetBlockGrid(index, child);
        }
    }
    
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
}
