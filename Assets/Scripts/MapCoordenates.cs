using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public static class MapCoordenates
{
    static public Tilemap tilemap { get; private set; }
    public static bool initialized;
    // Start is called before the first frame update
    public static void Initialize()
    {
        GameObject tileMapObject = GameObject.FindWithTag("Map");
        if (tileMapObject != null)
        {
            tilemap = tileMapObject.GetComponent<Tilemap>();
            if (tilemap == null) Debug.LogError("TileMap Object not containing TileMapComponent");
        }
        else
        {
            Debug.LogError("No TileMap Object found");
        }
        initialized = true;
    }

    static public Vector2Int WorldPosTocoordenate(Vector3 worldPos)
    {
        if (tilemap == null) Initialize();
        GridLayout gridLayout = tilemap.transform.parent.GetComponentInParent<GridLayout>();
        return (Vector2Int)gridLayout.WorldToCell(worldPos);
    }

    static public bool IsSolid(Vector3 worldPos)
    {
        Tile tile = GetTile(worldPos);

        if (tile != null)
        {
            return true;
        }

        return false;

    }

    public static Bounds? GetCellBounds(Vector3 worldPos)
    {
        if (tilemap == null) Initialize();
        Vector3Int coordenate = tilemap.WorldToCell(worldPos);
        Bounds bounds =tilemap.GetBoundsLocal(coordenate);
        bounds.center += bounds.extents;
        return bounds;

        //return new Bounds(position, tilemap.cellSize);
    }

    static Tile GetTile(Vector3 worldPos)
    {
        if (tilemap == null) Initialize();
        Vector3Int coordenate = tilemap.WorldToCell(worldPos);
        return tilemap.GetTile(coordenate) as Tile;
    }

    internal static Vector2 GetTilePosition(Vector3 worldPos)
    {
        if (tilemap == null) Initialize();

        Vector3Int coordenate = tilemap.WorldToCell(worldPos);

        Vector3 position = tilemap.GetCellCenterWorld(coordenate);

        return position;
    }
}
