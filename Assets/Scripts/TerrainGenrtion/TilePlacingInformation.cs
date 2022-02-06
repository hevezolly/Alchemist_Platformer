using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePlacingInformation: TilePlacer
{
    private readonly Dictionary<Vector2Int, Tilemap> grassMap;
    private readonly Tilemap groundMap;
    private readonly Tilemap grassOutlineMap;
    private readonly Tilemap groundOutlineMap;

    public TilePlacingInformation(Dictionary<Vector2Int, Tilemap> grass,
        Tilemap grassOutline,
        Tilemap groundMap,
        Tilemap groundOutline)
    {
        grassMap = grass;
        this.groundMap = groundMap;
        grassOutlineMap = grassOutline;
        groundOutlineMap = groundOutline;
    }

    public override void PlaceGround(Sprite sprite, Vector2Int cord)
    {
        PlaceAnyTile(sprite, cord, groundMap, Tile.ColliderType.Grid);
    }

    public override void PlaceGrass(Sprite sprite, Vector2Int side, Vector2Int cord)
    {
        PlaceAnyTile(sprite, cord, grassMap[side], Tile.ColliderType.None);
    }

    public override void PlaceGrassOutline(Sprite sprite, Vector2Int cord)
    {
        PlaceAnyTile(sprite, cord, grassOutlineMap, Tile.ColliderType.None);
    }

    public override void PlaceGroundOutline(Sprite sprite, Vector2Int cord)
    {
        PlaceAnyTile(sprite, cord, groundOutlineMap, Tile.ColliderType.None);
    }
}
