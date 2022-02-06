using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName ="new biome", menuName ="game/biome")]
public class Biome : ScriptableObject
{
    public new string name;
    [Tooltip("only for gizmos")]
    public Color color;

    public GroundGenerator groundTiles;
    public List<Tile> ladders;
    public Tile LastLadderTile;

    public void Initiate(OutlinesData data, 
        Dictionary<string, Texture2D> secondaryTextures)
    {
        groundTiles.Initiate(data, secondaryTextures);
    }

    public void GenerateBlockAt(Vector2Int cord, 
        List<Vector2Int> Sides,
        TilePlacer tilePlacer, bool needToGeneratePlants = false)
    {
        groundTiles.PlaceBlock(cord, Sides, tilePlacer);
        if (needToGeneratePlants)
            groundTiles.GeneratePlants(cord, Sides);
    }

    public void GenerateLadderAt(Vector2Int cord, Tilemap map, bool isLast)
    {
        var tile = LastLadderTile;
        if (!isLast)
            tile = ladders[Random.Range(0, ladders.Count)];
        map.SetTile((Vector3Int)cord, tile);
    }
}
