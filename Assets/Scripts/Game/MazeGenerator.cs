using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    [Header("Tilemap Setup")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private Tilemap fogOfWarTilemap;
    [SerializeField] private TilesSettingsScriptableObject tilesSettings;    

    [Header("Maze Size (Number of Cells)")]

    [SerializeField] private int numCellsX = 30;
    [SerializeField] private int numCellsY = 30;
    [SerializeField] private int bufferSize = 10;

    [SerializeField] private float extraCarveProbability = 0.05f;
    [SerializeField] private int extraCarvePasses = 2;

    private int mapWidth;
    private int mapHeight;
    private bool[,] visited;
    private Vector3Int exitCoordinates;
    
    private static readonly Vector2Int[] directions = {
        new(1, 0),
        new(0, -1),
        new(-1, 0),
        new(0, 1)
    };

    [ContextMenu("Generate Maze")]
    public void GenerateAllTiles()
    {
        mapWidth = numCellsX * 2 + 1;
        mapHeight = numCellsY * 2 + 1;

        // Clear previous maze
        groundTilemap.ClearAllTiles();
        wallsTilemap.ClearAllTiles();

        FillTilemap(groundTilemap, tilesSettings.FloorTiles, mapWidth + bufferSize * 2, mapHeight + bufferSize * 2, new Vector3Int(-bufferSize, -bufferSize, 0));
        FillTilemap(wallsTilemap, tilesSettings.WallTiles, mapWidth, mapHeight, Vector3Int.zero);

        if (fogOfWarTilemap != null)
        {
            fogOfWarTilemap.ClearAllTiles();
            FillTilemap(fogOfWarTilemap, tilesSettings.FogTiles, mapWidth + bufferSize * 2, mapHeight + bufferSize * 2, new Vector3Int(-bufferSize, -bufferSize, 0));
        }
        

        int centerX = numCellsX / 2;
        int centerY = numCellsY / 2;
        Vector2Int start = new Vector2Int(centerX, centerY);
        CarveFloor(wallsTilemap, start.x, start.y);

        GenerateMaze(start, wallsTilemap);
        GenerateExtraBranching(start, wallsTilemap);

        PlaceExit();
    }

    public void GenerateMaze(Vector2Int start, Tilemap tilemap)
    {
        visited = new bool[numCellsX, numCellsY];

        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        visited[start.x, start.y] = true;
        stack.Push(start);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> unvisitedNeighbors = GetUnvisitedNeighbors(current);

            if (unvisitedNeighbors.Count > 0)
            {
                int randomIndex = Random.Range(0, unvisitedNeighbors.Count);
                Vector2Int next = unvisitedNeighbors[randomIndex];

                CarvePassage(tilemap, current, next);

                CarveFloor(tilemap, next.x, next.y);
                visited[next.x, next.y] = true;
                stack.Push(next);
            }
            else
            {
                stack.Pop();
            }
        }
    }

    private void FillTilemap(Tilemap tilemap, TileBase[] tiles, int width, int height, Vector3Int offset)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0) + offset, tiles[Random.Range(0, tiles.Length)]);
            }
        }
    }

    private void CarveFloor(Tilemap tilemap, int cellX, int cellY)
    {
        Vector3Int pos = new Vector3Int(cellX * 2 + 1, cellY * 2 + 1, 0);
        tilemap.SetTile(pos, null);
    }

    private void CarvePassage(Tilemap tilemap, Vector2Int from, Vector2Int to)
    {
        int passageX = (from.x + to.x + 1);
        int passageY = (from.y + to.y + 1);
        Vector3Int pos = new Vector3Int(passageX, passageY, 0);
        tilemap.SetTile(pos, null);
    }        

    private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        for (int i = 0; i < directions.Length; i++)
        {
            // Shuffle for randomness
            int randomIndex = Random.Range(i, directions.Length);
            Vector2Int temp = directions[i];
            directions[i] = directions[randomIndex];
            directions[randomIndex] = temp;

            Vector2Int next = pos + directions[i];
            if (next.x >= 0 && next.x < numCellsX && next.y >= 0 && next.y < numCellsY && !visited[next.x, next.y])
            {
                neighbors.Add(next);
            }
        }
        return neighbors;
    }

    private void GenerateExtraBranching(Vector2Int start, Tilemap tilemap)
    {
        const int tooCloseToStartDistance = 2;

        for (int pass = 0; pass < extraCarvePasses; pass++)
        {
            for (int x = 0; x < numCellsX; x++)
            {
                for (int y = 0; y < numCellsY; y++)
                {
                    if (Random.value >= extraCarveProbability) continue;

                    Vector2Int from = new Vector2Int(x, y);
                    Vector2Int dir = directions[Random.Range(0, directions.Length)];
                    Vector2Int to = from + dir;

                    if (to.x < 0 || to.x >= numCellsX || to.y < 0 || to.y >= numCellsY)
                        continue;

                    int fromDist = Mathf.Max(Mathf.Abs(from.x - start.x), Mathf.Abs(from.y - start.y));
                    int toDist = Mathf.Max(Mathf.Abs(to.x - start.x), Mathf.Abs(to.y - start.y));
                    if (fromDist <= tooCloseToStartDistance || toDist <= tooCloseToStartDistance)
                        continue;

                    int passageX = (from.x + to.x + 1);
                    int passageY = (from.y + to.y + 1);
                    Vector3Int passagePos = new Vector3Int(passageX, passageY, 0);
                    if (tilemap.GetTile(passagePos) != null)
                    {
                        CarvePassage(tilemap, from, to);
                        CarveFloor(tilemap, from.x, from.y);
                        CarveFloor(tilemap, to.x, to.y);
                    }
                }
            }
        }
    }

    private void PlaceExit()
    {
        // Random corner exit
        Vector3Int[] cornerExits = {
            new Vector3Int(0, 1, 0),                          // Top-left
            new Vector3Int(mapWidth - 1, 1, 0),               // Top-right
            new Vector3Int(0, mapHeight - 2, 0),              // Bottom-left
            new Vector3Int(mapWidth - 1, mapHeight - 2, 0)    // Bottom-right
        };

        int randCorner = Random.Range(0, cornerExits.Length);

        exitCoordinates = cornerExits[randCorner];

        groundTilemap.SetTile(exitCoordinates, tilesSettings.ExitTile);
        wallsTilemap.SetTile(exitCoordinates, null);
    }

    public Vector3 GetCenterPosition()
    {
        var tilemapCenterPosition = groundTilemap.CellToWorld(new Vector3Int(mapWidth / 2, mapHeight / 2));
        return tilemapCenterPosition + groundTilemap.cellSize / 2f;
    }

    public void SetMazeSize(int cellsX, int cellsY)
    {
        numCellsX = cellsX;
        numCellsY = cellsY;
    }

    public Vector3Int GetExitCoordinates()
    {
        return exitCoordinates;
    }
}