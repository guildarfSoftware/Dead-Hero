using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public static class MapCoordenates
{
    static public Grid grid { get; private set; }
    public static bool initialized;
    private static string[] solidLayers = { "Obstacles" };

    static Vector2 halfCellSize;

    // Start is called before the first frame update
    public static void Initialize()
    {
        grid = GameObject.FindObjectOfType<Grid>();
        if (grid == null)
        {
            Debug.LogError("No Grid found in the level");
        }
        else
        {
            halfCellSize = grid.cellSize * 0.5f;
            initialized = true;
        }
    }

    static public Vector2Int WorldPosTocoordenate(Vector3 worldPos)
    {
        if (grid == null) Initialize();
        GridLayout gridLayout = grid.transform.parent.GetComponentInParent<GridLayout>();
        return (Vector2Int)gridLayout.WorldToCell(worldPos);
    }

    static public bool IsSolid(Vector3 worldPos)
    {
        Vector3 tileCenterPosition = WorldToTileCenterPosition(worldPos);

        RaycastHit2D hit2D = Physics2D.BoxCast(tileCenterPosition, halfCellSize, 0, Vector2.zero, 0, LayerMask.GetMask(solidLayers));
        return hit2D.collider != null;

    }

    private static Vector3 WorldToTileCenterPosition(Vector3 worldPos)
    {
        if (grid == null) Initialize();
        Vector3 returnPos = Vector3Int.FloorToInt(worldPos);

        returnPos += (Vector3) halfCellSize;

        return returnPos;
    }

    public static Bounds? GetCellBounds(Vector3 worldPos)
    {
        if (grid == null) Initialize();
        Vector3Int coordenate = grid.WorldToCell(worldPos);
        Bounds bounds = grid.GetBoundsLocal(coordenate);
        bounds.center += bounds.extents;
        return bounds;

        //return new Bounds(position, tilemap.cellSize);
    }

    internal static Vector2 GetTilePosition(Vector3 worldPos)
    {
        if (grid == null) Initialize();

        Vector3Int coordenate = grid.WorldToCell(worldPos);

        Vector3 position = grid.GetCellCenterWorld(coordenate);

        return position;
    }
}
