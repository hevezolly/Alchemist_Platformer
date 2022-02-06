using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class TilePlacer
{
    public abstract void PlaceGround(Sprite sprite, Vector2Int cord);

    public abstract void PlaceGrass(Sprite sprite, Vector2Int side, Vector2Int cord);

    public abstract void PlaceGrassOutline(Sprite sprite, Vector2Int cord);

    public abstract void PlaceGroundOutline(Sprite sprite, Vector2Int cord);

    protected void PlaceAnyTile(Sprite sprite, Vector2Int cord, Tilemap map, Tile.ColliderType type)
    {
        var grassTile = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
        grassTile.colliderType = type;
        grassTile.sprite = sprite;
        map.SetTile((Vector3Int)(cord), grassTile);
    }

}
