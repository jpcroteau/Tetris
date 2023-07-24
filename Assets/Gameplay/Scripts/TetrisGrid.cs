using System;
using UnityEngine;

public class TetrisGrid : MonoBehaviour
{
    public static TetrisGrid Instance = null;
    
    public static int gridWidth = 10;
    public static int gridHeight = 10;
    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Function to draw the grid using Gizmos in the Unity Editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;

        var gridPosition = Vector3.zero; //transform.position;
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
    }
    
    // Function to check if a position is within the grid boundaries
    public static bool IsInsideGrid(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridWidth && position.y >= 0;
    }

    // Function to Round a position to the nearest integer (snap to grid)
    public static Vector2Int RoundVector(Vector2 position)
    {
        return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }

    // Function to Clear a row and shift all rows above it downwards
    public static void ClearRow(int row)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            Destroy(grid[x, row].gameObject);
            grid[x, row] = null;
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
    public static bool IsRowFull(int row)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (grid[x, row] == null)
            {
                return false;
            }
        }

        return true;
    }

    // Function to remove all completed rows
    public static void ClearCompletedRows()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            if (IsRowFull(y))
            {
                ClearRow(y);
            }
        }
    }
}