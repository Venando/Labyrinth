using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Tilemap fogTilemap;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private int revealRadius = 3;

    private Vector3Int lastPlayerCellPosition = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
    
    private static readonly Vector3Int[] AdjacentDirections = new Vector3Int[]
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(1, -1, 0),
        new Vector3Int(1, 1, 0)
    };

    void Update()
    {
        RevealFogAroundPlayer();
    }

    private void RevealFogAroundPlayer()
    {
        Vector3Int playerCellPosition = fogTilemap.WorldToCell(playerTransform.position);

        if (playerCellPosition == lastPlayerCellPosition)
            return;

        lastPlayerCellPosition = playerCellPosition;

        int radiusInCells = Mathf.CeilToInt(revealRadius / fogTilemap.cellSize.x);

        for (int x = -radiusInCells; x <= radiusInCells; x++)
        {
            for (int y = -radiusInCells; y <= radiusInCells; y++)
            {
                Vector3Int currentCell = new Vector3Int(playerCellPosition.x + x, playerCellPosition.y + y, playerCellPosition.z);

                if (!fogTilemap.HasTile(currentCell))
                    continue;

                var success = pathfinder.TryToCalculatePath(playerCellPosition,
                    currentCell, revealRadius, out List<Vector3Int> _);
                
                if (!success)
                    continue;
                    
                fogTilemap.SetTile(currentCell, null);

                RevealWallsAroundCell(currentCell);
            }
        }
    }

    private void RevealWallsAroundCell(Vector3Int currentCell)
    {
        foreach (var dir in AdjacentDirections)
        {
            Vector3Int cell = currentCell + dir;
            if (wallsTilemap.HasTile(cell) && fogTilemap.HasTile(cell))
            {
                fogTilemap.SetTile(cell, null);
            }
        }
    }
}
