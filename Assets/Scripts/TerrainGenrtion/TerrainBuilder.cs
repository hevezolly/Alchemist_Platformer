using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[RequireComponent(typeof(TerrainMarker), typeof(BackgroundColorSetter))]
public class TerrainBuilder : MonoBehaviour
{
    private const float Scale = 0.1f;

    private TerrainMarker marker;
    private HashSet<Vector2Int> shownBlocks;
    public Camera cam;
    private Vector3 camPos;
    private float distance;
    
    public Tilemap map;
    public Tilemap ladders;
    [Space]
    public Tilemap grassTop;
    public Tilemap grassRight;
    public Tilemap grassLeft;
    public Tilemap grassBottom;
    [Space]
    public Tilemap grassOutline;
    public Tilemap groundOutline;
    [Space]
    public Tilemap background;
    public Tilemap backgroundOutline;
    public GridControlller grid;

    private Dictionary<Vector2Int, Tilemap> grassTiles = new Dictionary<Vector2Int, Tilemap>();

    public GameObject Player;

    private TerrainGenerator generator;

    private BackgroundColorSetter colorSetter;

    private GameObject playerObj;

    void Start()
    {
        colorSetter = GetComponent<BackgroundColorSetter>();
        camPos = cam.transform.position;
        marker = GetComponent<TerrainMarker>();
        generator = GetComponent<TerrainGenerator>();
        distance = Mathf.Sqrt(cam.orthographicSize * cam.orthographicSize +
            (cam.orthographicSize * cam.aspect) *
            (cam.orthographicSize * cam.aspect));
        shownBlocks = new HashSet<Vector2Int>();
        grassTiles[Vector2Int.up] = grassTop;
        grassTiles[Vector2Int.right] = grassRight;
        grassTiles[Vector2Int.left] = grassLeft;
        grassTiles[Vector2Int.down] = grassBottom;
        BuildLevel();
        
    }

    // Start is called before the first frame update
    private void BuildLevel()
    {   
        marker.MarkTerrain();
        map.ClearAllTiles();
        ladders.ClearAllTiles();
        marker.InitiateBiomes();
        var foregroundPlacing = new TilePlacingInformation(grassTiles, grassOutline, map, groundOutline);
        var backgrounPlacing = new BackGroundPlacing(background, backgroundOutline);
        map.GetComponent<TilemapRenderer>().chunkCullingBounds = new Vector3(distance, distance);
        for (var x = 0; x < marker.terrain.GetLength(0); x++)
        {
            for (var y = 0; y < marker.terrain.GetLength(1); y++)
            {
                var cell = marker.terrain[x, y];
                if (cell.type == TerrainType.Spawn)
                {
                    var position = map.LocalToWorld(new Vector3Int(x, y, 0));
                    playerObj = Instantiate(Player, position, Quaternion.identity);
                    cam.GetComponent<CameraMove>().AssignPlayer(playerObj.transform);
                    colorSetter.Initiate(marker.backgroundColor, playerObj.transform);
                }
                else if (cell.type == TerrainType.Ladder)
                {
                    var AtTop = marker.terrain[x, y + 1].type == TerrainType.Empty;
                    if (AtTop)
                        cell.biome.GenerateLadderAt(new Vector2Int(x, y + 1), ladders, true);
                    cell.biome.GenerateLadderAt(new Vector2Int(x, y), ladders, false);
                }
                else if (cell.type != TerrainType.Empty)
                {
                    cell.biome.GenerateBlockAt(new Vector2Int(x, y), new List<Vector2Int>(cell.sides), foregroundPlacing, true);
                }
                if (cell.hasBackground)
                    cell.biome.GenerateBlockAt(new Vector2Int(x, y), new List<Vector2Int>(cell.sides), backgrounPlacing);
                else if (cell.type == TerrainType.Ground &&
                    generator.GetStraightNeighbours(new Vector2Int(x, y)).Any(c => marker.terrain[c.x, c.y].hasBackground))
                    cell.biome.GenerateBlockAt(new Vector2Int(x, y), new List<Vector2Int>(), backgrounPlacing);
            }
        }
        foreach (var name in marker.secondaryTextures.Keys)
        {
            marker.secondaryTextures[name].Apply();
            grid.SetGrassTexture(name, marker.secondaryTextures[name]);
        }
        ladders.GetComponent<CompositeCollider2D>().GenerateGeometry();
        map.GetComponent<CompositeCollider2D>().GenerateGeometry();
        GetComponent<VectorField>().SetTextures();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        Gizmos.DrawWireSphere(camPos, distance);
    }
}
