using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    public Transform mazeContainer;
    private CellData[] allCells;
    private int width = 10;
    private int height = 10;
    private int nextSetID = 1;

    void Start()
    {
        allCells = mazeContainer.GetComponentsInChildren<CellData>();
        GenerateNewMaze();
    }

    // This is the function you will link to your UI Button
    public void GenerateNewMaze()
    {
        ResetMaze();
        RunEllerAlgorithm();
        HighlightStartEnd();
    }

    void ResetMaze()
    {
        nextSetID = 1;
        foreach (CellData cell in allCells)
        {
            cell.setID = 0; // Reset sets [cite: 68-69]
            cell.SetRightWall(true); // Turn all walls back on [cite: 107]
            cell.SetBottomWall(true);
            cell.GetComponent<UnityEngine.UI.Image>().color = Color.white; // Reset color
        }
    }

    void RunEllerAlgorithm()
    {
        for (int y = 0; y < height; y++)
        {
            AssignSets(y);

            if (y < height - 1)
            {
                CreateRightWalls(y);
                CreateBottomWalls(y);
                PrepareNextRow(y);
            }
            else
            {
                HandleFinalRow(y);
            }
        }
    }

    void HighlightStartEnd()
    {
        CellData startCell = GetCell(0, 0);
        CellData endCell = GetCell(width - 1, height - 1);

        startCell.GetComponent<UnityEngine.UI.Image>().color = new Color(0.5f, 1f, 0.5f);
        endCell.GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 0.5f, 0.5f);
        endCell.SetRightWall(false); // Open exit point [cite: 103, 144]
    }

    void AssignSets(int y)
    {
        for (int x = 0; x < width; x++)
        {
            CellData cell = GetCell(x, y);
            if (cell.setID == 0)
                cell.setID = nextSetID++;
        }
    }

    void CreateRightWalls(int y)
    {
        for (int x = 0; x < width - 1; x++)
        {
            CellData current = GetCell(x, y);
            CellData next = GetCell(x + 1, y);

            // Always wall if same set, otherwise random
            if (current.setID == next.setID || Random.value < 0.5f)
            {
                current.SetRightWall(true);
            }
            else
            {
                current.SetRightWall(false);
                MergeSets(next.setID, current.setID);
            }
        }

        GetCell(width - 1, y).SetRightWall(true);
    }

    void CreateBottomWalls(int y)
    {
        Dictionary<int, List<int>> setsInRow = new Dictionary<int, List<int>>();

        for (int x = 0; x < width; x++)
        {
            int id = GetCell(x, y).setID;

            if (!setsInRow.ContainsKey(id))
                setsInRow[id] = new List<int>();

            setsInRow[id].Add(x);
        }

        foreach (var set in setsInRow)
        {
            List<int> cellsInSet = set.Value;
            bool hasExit = false;

            for (int i = 0; i < cellsInSet.Count; i++)
            {
                int x = cellsInSet[i];

                if (cellsInSet.Count > 1 && Random.value < 0.5f &&
                    !( !hasExit && i == cellsInSet.Count - 1))
                {
                    GetCell(x, y).SetBottomWall(true);
                }
                else
                {
                    GetCell(x, y).SetBottomWall(false);
                    hasExit = true;
                }
            }
        }
    }

    void PrepareNextRow(int y)
    {
        for (int x = 0; x < width; x++)
        {
            CellData current = GetCell(x, y);
            CellData below = GetCell(x, y + 1);

            // Transfer set if no bottom wall
            if (!current.bottomWall.activeSelf)
                below.setID = current.setID;
        }
    }

    void HandleFinalRow(int y)
    {
        for (int x = 0; x < width - 1; x++)
        {
            CellData current = GetCell(x, y);
            CellData next = GetCell(x + 1, y);

            current.SetBottomWall(true);

            // Connect remaining sets
            if (current.setID != next.setID)
            {
                current.SetRightWall(false);
                MergeSets(next.setID, current.setID);
            }
            else
            {
                current.SetRightWall(true);
            }
        }

        GetCell(width - 1, y).SetBottomWall(true);
        GetCell(width - 1, y).SetRightWall(true);
    }

    void MergeSets(int oldID, int newID)
    {
        foreach (CellData c in allCells)
        {
            if (c.setID == oldID)
                c.setID = newID;
        }
    }

    CellData GetCell(int x, int y)
    {
        return allCells[y * width + x];
    }
}