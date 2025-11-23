using UnityEngine;
using UnityEngine.Tilemaps;

using System.Collections.Generic;

[RequireComponent(typeof(Tilemap))]
public class TilemapSaver : MonoBehaviour, ISavable
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TilesSettingsScriptableObject tilesSettings;
    

    private static Dictionary<string, TileBase> tileCache = new Dictionary<string, TileBase>();

    public string SaveKey => $"Tilemap_{gameObject.name}";

    public string CaptureState()
    {
        if (tilemap == null) tilemap = GetComponent<Tilemap>();
        var bounds = tilemap.cellBounds;
        var tiles = new List<TileData>();

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var pos = new Vector3Int(x, y, 0);
                var tile = tilemap.GetTile(pos);
                if (tile == null) continue;
                tiles.Add(new TileData { tileName = tile.name, x = x, y = y });
            }
        }

        var data = new TilemapData
        {
            tiles = tiles.ToArray(),
            boundsX = bounds.size.x,
            boundsY = bounds.size.y
        };

        return JsonUtility.ToJson(data);
    }

    public void RestoreState(string json)
    {
        if (tilemap == null) tilemap = GetComponent<Tilemap>();
        if (string.IsNullOrEmpty(json)) return;

        var data = JsonUtility.FromJson<TilemapData>(json);
        if (data == null) return;

        tilemap.ClearAllTiles();

        if (data.tiles == null) return;
        foreach (var t in data.tiles)
        {
            if (!tilesSettings.TryGetTile(t.tileName, out TileBase tile))
                continue;

            var pos = new Vector3Int(t.x, t.y, 0);
            tilemap.SetTile(pos, tile);
        }

        tilemap.RefreshAllTiles();
    }
}

[System.Serializable]
public class TilemapData
{
    public TileData[] tiles;
    public int boundsX;
    public int boundsY;
}

[System.Serializable]
public class TileData
{
    public string tileName;
    public int x;
    public int y;
}