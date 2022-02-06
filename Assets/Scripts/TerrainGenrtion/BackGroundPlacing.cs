using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BackGroundPlacing : TilePlacer
{
    private Tilemap background;
    private Tilemap backgroundOutline;

    public BackGroundPlacing(Tilemap background, Tilemap backgroundOutline)
    {
        this.background = background;
        this.backgroundOutline = backgroundOutline;
    }

    public override void PlaceGrass(Sprite sprite, Vector2Int side, Vector2Int cord)
    {
        
    }

    public override void PlaceGrassOutline(Sprite sprite, Vector2Int cord)
    {
        
    }

    public override void PlaceGround(Sprite sprite, Vector2Int cord)
    {
        PlaceAnyTile(sprite, cord, background, Tile.ColliderType.None);
    }

    public override void PlaceGroundOutline(Sprite sprite, Vector2Int cord)
    {
        PlaceAnyTile(sprite, cord, backgroundOutline, Tile.ColliderType.None);
    }
}
