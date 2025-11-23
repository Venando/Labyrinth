using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TilesSettingsScriptableObject : ScriptableObject
{
    public TileBase[] FloorTiles;
    public TileBase ExitTiles;
    public TileBase[] WallTiles;
    public TileBase[] FogTiles;

    private Dictionary<string, TileBase> tilesDictionary = new();

    public TileBase NextFloorTile => FloorTiles[Random.Range(0, FloorTiles.Length)];
    public TileBase ExitTile => ExitTiles;
    public TileBase NextWallTile => WallTiles[Random.Range(0, WallTiles.Length)];
    public TileBase NextFogTile => FogTiles[Random.Range(0, FogTiles.Length)];

    public bool TryGetTile(string name, out TileBase tileBase)
    {
        if (tilesDictionary.Count == 0)
        {
            tilesDictionary = FloorTiles.Concat(WallTiles)
                .Concat(FogTiles).Append(ExitTiles)
                .ToDictionary(tile => tile.name, tile => tile);
        }
        return tilesDictionary.TryGetValue(name, out tileBase);
    }
}
