using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class TetrisGrid : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private int gridWidth = 10;
    public int GridWidth => gridWidth;
    
    [SerializeField] private int gridHeight = 10;
    public int GridHeight => gridHeight;
    
    [Header("Tetromino")]
    [SerializeField] private Tetromino[] tetrominosPrefab;

    private Transform[,] grid;
    
    public Transform GetGridElement(int x, int y) => grid[x, y];
    public void SetGridElement(int x, int y, Transform value) => grid[x, y] = value;

    public Vector3 GetPositionAtIndex(int x, int y) => transform.position + new Vector3(x, y, 0);
    public Vector2Int GetIndexAtPosition(Vector3 position) => RoundVector(position - transform.position);

    public bool IsGameOver { get; private set; } = false;

    private void Awake()
    {
        grid = new Transform[gridWidth, gridHeight];
    }

    private void Start()
    {
        Reset();
    }

    private void Update()
    {
        if (IsGameOver && Input.GetKeyDown(KeyCode.Backspace))
        {
            Reset();
        }
    }

    private void OnGUI()
    {
        if (IsGameOver)
        {
            GUIStyle guiStyle = new GUIStyle(GUI.skin.label); 
            guiStyle.fontSize = 40;
            guiStyle.alignment = TextAnchor.MiddleCenter;
                
            GUI.color = Color.red;
            GUI.Label(new Rect(0f, 0f, Screen.width, Screen.height), "GAME OVER", guiStyle);
        }
    }

    private void SpawnRandomTetromino()
    {
        if (tetrominosPrefab.Length <= 0) return;
        
        var tetromino = Instantiate(tetrominosPrefab[Random.Range(0, tetrominosPrefab.Length - 1)], Vector3.zero, Quaternion.identity, transform);
        tetromino.Spawn(this, OnTetrominoEnd, gridWidth/2, gridHeight);
    }

    private void OnTetrominoEnd(Tetromino tetromino)
    {
        ClearCompletedRows();
        
        IsGameOver = IsTopRowFilled();
        if (!IsGameOver)
        {
            SpawnRandomTetromino();
        }
    }

    // Function to draw the grid using Gizmos in the Unity Editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;

        var gridPosition = transform.position;
        var offset = new Vector3(-0.5f, -0.5f);

        // Draw vertical lines
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = gridPosition + new Vector3(x, 0, 0);
            Vector3 end = gridPosition + new Vector3(x, gridHeight, 0);
            Gizmos.DrawLine(start + offset, end + offset);
        }

        // Draw horizontal lines
        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = gridPosition + new Vector3(0, y, 0);
            Vector3 end = gridPosition + new Vector3(gridWidth, y, 0);
            Gizmos.DrawLine(start + offset, end + offset);
        }

        if (Application.isPlaying)
        {
            for (int i = 0; i < gridWidth; i++)
            {
                for (int j = 0; j < gridHeight; j++)
                {
                    Gizmos.color = GetGridElement(i, j) == null ? Color.clear : Color.red;
                    Gizmos.DrawCube(GetPositionAtIndex(i, j), Vector3.one);
                }
            }
        }
    }
    
    // Function to check if the top row of the grid is completely filled with tetrominoes
    private bool IsTopRowFilled()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (GetGridElement(x,  gridHeight - 1) != null)
            {
                return true;
            }
        }
        return false;
    }
    
    // Function to check if a position is within the grid boundaries
    public bool IsInsideGrid(Vector2Int index)
    {
        //var gridPosition = Instance.transform.position;
        return index.x >= 0 && index.x < gridWidth && index.y >= 0;
    }

    // Function to Round a position to the nearest integer (snap to grid)
    public Vector2Int RoundVector(Vector2 position)
    {
        return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }

    // Function to Clear a row and shift all rows above it downwards
    public void ClearRow(int row)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            Destroy(GetGridElement(x, row).gameObject);
            SetGridElement(x, row, null);
        }

        // Move all rows above 'row' down by one step
        for (int y = row + 1; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] != null)
                {
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y] = null;
                    grid[x, y - 1].position += Vector3.down;
                }
            }
        }
    }

    // Function to check if a row is full
    public bool IsRowFull(int row)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (GetGridElement(x,  row) == null)
            {
                return false;
            }
        }

        return true;
    }

    // Function to remove all completed rows
    public void ClearCompletedRows()
    {
        for (int y = gridHeight - 1; y >= 0; y--)
        {
            if (IsRowFull(y))
            {
                ClearRow(y);
            }
        }
    }

    private void Reset()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        IsGameOver = false;
        
        SpawnRandomTetromino();
    }
}