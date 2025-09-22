using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class RecursiveDivisionMaze : MonoBehaviour
{
    public Tilemap wallTilemap;
    public Tilemap floorTilemap;
    public Tilemap pathTilemap;
    public Tilemap startFinishTilemap;

    public TileBase wallTile;
    public TileBase floorTile;
    public TileBase pathTile;
    public TileBase startTile;
    public TileBase finishTile;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public int width = 11;
    public int height = 11;

    public int[,] maze;

    public GameObject player;
    public GameObject enemy;

    public float enemySpeed = 2.0f; // Kecepatan musuh
    private List<Vector3Int> enemyPath; // Jalur yang akan diikuti musuh
    private int currentPathIndex = 0; // Indeks jalur saat ini untuk musuh

    void Start()
    {
        GenerateMaze();
        DrawMaze();
        SetStartAndFinish();
        SpawnPlayerAndEnemy();
        StartEnemyPathfinding();
    }

    public void GenerateNewMaze()
    {
        ClearMaze(); //hapus maze sebelumnya
        GenerateMaze(); // buat maze baru
        DrawMaze(); // gambar ulang maze
        SetStartAndFinish();// tentukan ulang posisi start dan finish
        SpawnPlayerAndEnemy(); //spawn ulang player dan enemy
        StartEnemyPathfinding(); //enemy melakukan pencarian jalur
    }

    public void ShowPathfinding()
    {
        List<Vector3Int> path = FindPathAStar(new Vector3Int(1, 1, 0), new Vector3Int(width - 2, height - 2, 0));
        DrawPath(path);
    }

    void ClearMaze()
    {
        wallTilemap.ClearAllTiles();
        floorTilemap.ClearAllTiles();
        pathTilemap.ClearAllTiles();
        startFinishTilemap.ClearAllTiles();

        if (player != null) Destroy(player);
        if (enemy != null) Destroy(enemy);
    }

    void GenerateMaze()//Membuat Maze
    {
        maze = new int[height, width];

        // Mengisi border dengan dinding
        for (int x = 0; x < width; x++)
        {
            maze[0, x] = 1;
            maze[height - 1, x] = 1;
        }
        for (int y = 0; y < height; y++)
        {
            maze[y, 0] = 1;
            maze[y, width - 1] = 1;
        }

        // Mulai pembagian rekursif
        RecursiveDivision(1, height - 2, 1, width - 2, true);
    }

    void RecursiveDivision(int startRow, int endRow, int startCol, int endCol, bool horizontal)
    {
        if (endRow - startRow < 2 || endCol - startCol < 2)
            return;

        if (horizontal)
        {
            int wallRow = Random.Range(startRow + 1, endRow - 1);
            for (int col = startCol; col <= endCol; col++)
            {
                maze[wallRow, col] = 1;
            }

            // Buat dua lubang pada dinding horizontal
            int holeCol1 = Random.Range(startCol, endCol + 1);
            int holeCol2;
            do
            {
                holeCol2 = Random.Range(startCol, endCol + 1);
            } while (holeCol2 == holeCol1);

            maze[wallRow, holeCol1] = 0;
            maze[wallRow, holeCol2] = 0;

            // Rekursif pada dua bagian yang baru dibuat
            RecursiveDivision(startRow, wallRow - 1, startCol, endCol, !horizontal);
            RecursiveDivision(wallRow + 1, endRow, startCol, endCol, !horizontal);
        }
        else
        {
            int wallCol = Random.Range(startCol + 1, endCol - 1);
            for (int row = startRow; row <= endRow; row++)
            {
                maze[row, wallCol] = 1;
            }

            // Buat dua lubang pada dinding vertikal
            int holeRow1 = Random.Range(startRow, endRow + 1);
            int holeRow2;
            do
            {
                holeRow2 = Random.Range(startRow, endRow + 1);
            } while (holeRow2 == holeRow1);

            maze[holeRow1, wallCol] = 0;
            maze[holeRow2, wallCol] = 0;

            // Rekursif pada dua bagian yang baru dibuat
            RecursiveDivision(startRow, endRow, startCol, wallCol - 1, !horizontal);
            RecursiveDivision(startRow, endRow, wallCol + 1, endCol, !horizontal);
        }
    }

    void DrawMaze()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                if (maze[y, x] == 1)
                {
                    wallTilemap.SetTile(position, wallTile);
                }
                else
                {
                    floorTilemap.SetTile(position, floorTile);
                }
            }
        }
    }

    void SetStartAndFinish()
    {
        Vector3Int startPosition = new Vector3Int(1, 1, 0);
        startFinishTilemap.SetTile(startPosition, startTile);

        Vector3Int finishPosition = new Vector3Int(width - 2, height - 2, 0);
        startFinishTilemap.SetTile(finishPosition, finishTile);
    }

    void SpawnPlayerAndEnemy()
    {
        Vector3Int startPosition = new Vector3Int(1, 1, 0);
        player = Instantiate(playerPrefab, startFinishTilemap.CellToWorld(startPosition) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);

        Vector3Int enemyPosition = new Vector3Int(width / 2, height / 2, 0);
        enemy = Instantiate(enemyPrefab, startFinishTilemap.CellToWorld(enemyPosition) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
    }

    List<Vector3Int> FindPathAStar(Vector3Int start, Vector3Int end)
    {
        Dictionary<Vector3Int, int> gScore = new Dictionary<Vector3Int, int>();
        Dictionary<Vector3Int, int> fScore = new Dictionary<Vector3Int, int>();
        Dictionary<Vector3Int, Vector3Int?> previous = new Dictionary<Vector3Int, Vector3Int?>();
        List<Vector3Int> openSet = new List<Vector3Int> { start };

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (maze[y, x] == 0)
                {
                    gScore[pos] = int.MaxValue;
                    fScore[pos] = int.MaxValue;
                    previous[pos] = null;
                }
            }
        }

        gScore[start] = 0;
        fScore[start] = Heuristic(start, end);

        while (openSet.Count > 0)
        {
            openSet.Sort((a, b) => fScore[a] - fScore[b]);
            Vector3Int current = openSet[0];

            if (current == end)
            {
                List<Vector3Int> path = new List<Vector3Int>();
                while (previous[current] != null)
                {
                    path.Add(current);
                    current = previous[current].Value;
                }
                path.Reverse();
                return path;
            }

            openSet.Remove(current);

            foreach (var neighbor in GetNeighbors(current))
            {
                int tentativeGScore = gScore[current] + 1;

                if (tentativeGScore < gScore[neighbor])
                {
                    previous[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, end);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    int Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    List<Vector3Int> GetNeighbors(Vector3Int node)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0)
        };

        foreach (var direction in directions)
        {
            Vector3Int neighbor = node + direction;
            if (neighbor.x >= 0 && neighbor.x < width && neighbor.y >= 0 && neighbor.y < height)
            {
                if (maze[neighbor.y, neighbor.x] == 0)
                {
                    neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
    }

    void DrawPath(List<Vector3Int> path)
    {
        pathTilemap.ClearAllTiles();

        foreach (var position in path)
        {
            if (position != new Vector3Int(1, 1, 0) && position != new Vector3Int(width - 2, height - 2, 0))
            {
                pathTilemap.SetTile(position, pathTile);
            }
        }
    }

    void Update()
    {
        if (enemyPath != null && enemyPath.Count > 0)
        {
            MoveEnemy();
        }
    }

    public void StartEnemyPathfinding()
    {
        Vector3Int enemyPosition = wallTilemap.WorldToCell(enemy.transform.position);
        Vector3Int playerPosition = wallTilemap.WorldToCell(player.transform.position);
        enemyPath = FindPathAStar(enemyPosition, playerPosition);
        currentPathIndex = 0;
    }

    void MoveEnemy()
    {
        if (currentPathIndex >= enemyPath.Count) return;

        Vector3 targetPosition = wallTilemap.CellToWorld(enemyPath[currentPathIndex]) + new Vector3(0.5f, 0.5f, 0);
        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetPosition, enemySpeed * Time.deltaTime);

        if (Vector3.Distance(enemy.transform.position, targetPosition) < 0.1f)
        {
            currentPathIndex++;
        }

        if (currentPathIndex >= enemyPath.Count)
        {
            StartEnemyPathfinding(); // Pencarian jalur ulang jika pemain bergerak
        }
    }
}
