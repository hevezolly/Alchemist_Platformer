using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(TerrainGenerator), typeof(BackGroundGenerator))]
public class TerrainMarker : MonoBehaviour
{
    public bool draw;
    public int sideOffsetToCheck;
    public int maxHeightToCheck;
    [Space]
    public float outlinesStep;
    public float outlinesScale;
    [Space]
    public Biome mainBiome;
    public int BiomeDistance;
    public List<BiomeShadr> secondaryBiomes;

    public Dictionary<string, Texture2D> secondaryTextures { get; private set; }

    public TerrainCell[,] terrain { get; private set; }
    private TerrainGenerator generator;
    private BackGroundGenerator background;

    private OutlinesData outlines;
    public BackgroundColor backgroundColor { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        backgroundColor = new BackgroundColor(mainBiome);
        generator = GetComponent<TerrainGenerator>();
        background = GetComponent<BackGroundGenerator>();
        if (draw)
            MarkTerrain();
    }

    private void CreateSecondaryTextures()
    {
        secondaryTextures = new Dictionary<string, Texture2D>();
    }

    public void InitiateBiomes()
    {
        mainBiome.Initiate(outlines, secondaryTextures);
        foreach(var second in secondaryBiomes)
        {
            second.biome.Initiate(outlines, secondaryTextures);
        }
    }

    private void ConfigureSides()
    {
        if (generator == null)
            generator = GetComponent<TerrainGenerator>();
        generator.GenerateMap();
        var map = generator.RawMap;
        background.GenerateBackground(map);
        terrain = new TerrainCell[generator.WidthHeight.x, generator.WidthHeight.y];
        for (var x = 0; x < terrain.GetLength(0); x++)
        {
            for (var y = 0; y < terrain.GetLength(1); y++)
            {
                var cord = new Vector2Int(x, y);
                if (map[x, y] == 0)
                {
                    terrain[x, y] = TerrainCell.Empty(cord);
                    terrain[x, y].hasBackground = background.backGround[new Vector2Int(x, y)];
                    foreach (var n in generator.GetStraightNeighbours(cord))
                    {
                        var side = n - cord;
                        if (map[n.x, n.y] == 0 && !background.backGround[new Vector2Int(n.x, n.y)])
                            terrain[x, y].AddSide(side);
                    }
                }
                else
                {
                    terrain[x, y] = TerrainCell.Filled(cord);
                    foreach (var n in generator.GetStraightNeighbours(cord))
                    {
                        var side = n - cord;
                        if (map[n.x, n.y] == 0)
                            terrain[x, y].AddSide(side);
                    }
                }
                terrain[x, y].biome = mainBiome;
            }
        }
    }

    private void SetSpawn()
    {
        var rooms = generator.rooms.OrderByDescending(r => r.center.y).ToList();
        var c = rooms[0].center;
        if (generator.GetStraightNeighbours(c).All(n => terrain[n.x, n.y].type != TerrainType.Empty))
        {
            while (terrain[c.x, c.y].type != TerrainType.Empty)
            {
                c = c + Vector2Int.up;
            }
        }
        else
        {
            while (terrain[c.x, c.y - 1].type == TerrainType.Empty)
                c = c - Vector2Int.up;
        }
        terrain[c.x, c.y].type = TerrainType.Spawn;
    }

    private List<TerrainCell> GetEdge()
    {
        var edge = new List<TerrainCell>();
        for (var x = 0; x < terrain.GetLength(0); x++)
        {
            for (var y = 0; y < terrain.GetLength(1); y++)
            {
                var cell = terrain[x, y];
                if (!cell.isInsideCliff)
                    edge.Add(cell);
            }
        }
        return edge;
    }

    private void PlaceBiomes()
    {
        var edge = GetEdge();
        ShuffleList(edge);
        var neededBiomes = secondaryBiomes.Where(s => s.SouldBeHere).ToList();
        var addBiomes = secondaryBiomes.Where(s => !s.SouldBeHere).ToList();
        foreach (var b in neededBiomes)
            TryPlaceBiome(b, edge);
        foreach (var b in addBiomes)
            TryPlaceBiome(b, edge);

    }

    private void TryPlaceBiome(BiomeShadr biome, List<TerrainCell> cells)
    {
        var cellsToRemove = new List<TerrainCell>();
        foreach (var cell in cells)
        {
            if (IsAreaFree(cell.cord.x, cell.cord.y, BiomeDistance))
            {
                var value = generator.mainRandom.NextDouble();
                if (biome.SouldBeHere || value < biome.chance)
                    SetBiome(biome, cell.cord);
                break;
            }
            else
            {
                cellsToRemove.Add(cell);
            }
        }
        foreach (var c in cellsToRemove)
        {
            cells.Remove(c);
        }
    }

    private void SetBiome(BiomeShadr biome, Vector2Int cord)
    {
        for (var x = cord.x - biome.radius; x <= cord.x + biome.radius; x++)
        {
            for (var y = cord.y - biome.radius; y <= cord.y + biome.radius; y++)
            {
                var dist = Vector2Int.Distance(cord, new Vector2Int(x, y));
                if (dist > biome.radius)
                    continue;
                if (generator.IsInRange(x, y))
                {
                    var distanceAmount = dist / biome.radius;

                    if (distanceAmount < biome.unTouchedDistance)
                        terrain[x, y].biome = biome.biome;
                    else
                    {
                        var t = Mathf.InverseLerp(biome.unTouchedDistance, 1, distanceAmount);
                        var tailChance = Mathf.Lerp(1, biome.distantTileChance, t);
                        var value = generator.mainRandom.NextDouble();
                        if (value < tailChance)
                            terrain[x, y].biome = biome.biome;
                    }
                }
            }
        }
        backgroundColor.AssignPosition(biome, cord);
    }

    private void EnshureConnectivity()
    {
        for (var y = terrain.GetLength(1) - 1; y >= 0; y--)
        {
            for (var x = 0; x < terrain.GetLength(0); x++)
            {
                var cell = terrain[x, y];
                if (cell.type != TerrainType.Ground)
                    continue;
                if (!cell.ContainsSide(Vector2Int.up))
                    continue;
                if (cell.ContainsSide(Vector2Int.left))
                {
                    var needLeader = NeedLeader(new Vector2Int(x, y), -1);
                    if (needLeader)
                    {
                        foreach (var c in GetAllEmptiesBellow(new Vector2Int(x - 1, y)))
                        {
                            c.type = TerrainType.Ladder;
                        }
                    }
                }
                if (cell.ContainsSide(Vector2Int.right))
                {
                    var needLeader = NeedLeader(new Vector2Int(x, y), 1);
                    if (needLeader)
                    {
                        foreach (var c in GetAllEmptiesBellow(new Vector2Int(x + 1, y)))
                        {
                            c.type = TerrainType.Ladder;
                        }
                    }
                }
            }
        }
    }

    private bool NeedLeader(Vector2Int block, int direction)
    {
        var needLedder = true;
        var x = block.x;
        var y = block.y;
        for (var offset = 1; offset <= sideOffsetToCheck; offset++)
        {
            
            var tryX = x + offset * direction;
            if (tryX < 0 || tryX >= terrain.GetLength(0))
                continue;
            if (terrain[tryX, y].type == TerrainType.Ladder)
            {
                needLedder = false;
            }
            else
            {
                var downs = GetAllEmptiesBellow(new Vector2Int(tryX, y));
                if (downs.Count != 0 && downs.Count < maxHeightToCheck)
                {
                    needLedder = false;
                    break;
                }
            }
        }
        var side = GetAllEmptiesBellow(new Vector2Int(x + direction, y));
        if (needLedder)
        {
            if (side.Count != 0)
            {
                if (side.All(d => terrain[d.cord.x - direction, d.cord.y].type != TerrainType.Empty))
                    needLedder = false;
            }
        }
        return needLedder;
    }

    private List<TerrainCell> GetAllEmptiesBellow(Vector2Int start)
    {
        
        var cells = new List<TerrainCell>();
        if (!generator.IsInRange(start.x, start.y))
            return cells;
        var current = terrain[start.x, start.y];
        while (current.type == TerrainType.Empty)
        {
            cells.Add(current);
            if (current.cord.y == 0)
                break;
            current = terrain[current.cord.x, current.cord.y - 1];
        }
        return cells;
    } 

    private bool IsAreaFree(int centerX, int centerY, int radius)
    {
        var center = new Vector2(centerX, centerY);
        for (var x = centerX - (int)radius; x <= centerX + (int)radius; x++)
        {
            for (var y = centerY - (int)radius; y <= centerY + (int)radius; y++)
            {
                if (!generator.IsInRange(x, y))
                    continue;
                var point = new Vector2(x, y);
                var currentDist = Vector2.Distance(center, point);
                if (currentDist > radius)
                    continue;
                if (terrain[x, y].biome != mainBiome)
                    return false;
            }
        }
        return true;
    }

    public void MarkTerrain()
    {
        ConfigureSides();
        CreateSecondaryTextures();
        outlines = new OutlinesData(outlinesScale, outlinesStep, c =>
        {
            if (generator.IsInRange(c.x, c.y))
                return terrain[c.x, c.y].biome;
            return null;
        });
        
        PlaceBiomes();
        SetSpawn();
        EnshureConnectivity();
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i >= 1; i--)
        {
            int j = generator.mainRandom.Next(i + 1);
            var temp = list[j];
            list[j] = list[i];
            list[i] = temp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (draw && Input.GetMouseButtonDown(0))
            MarkTerrain();
    }

    private void OnDrawGizmos()
    {
        if (terrain == null || !draw)
            return;
        for (var x = 0; x < terrain.GetLength(0); x++)
        {
            for (var y = 0; y < terrain.GetLength(1); y++)
            {
                var cell = terrain[x, y];
                var position = transform.position +
                            (Vector3)(Vector2)cell.cord * 0.1f;
                var alpha = 1f;
                if (cell.type == TerrainType.Empty)
                    alpha = 0.5f;
                switch (cell.type)
                {
                    case TerrainType.Ground:
                    case TerrainType.Empty:
                        Gizmos.color = cell.biome.color;
                        break;
                    case TerrainType.Ladder:
                        Gizmos.color = Color.red;
                        break;
                    case TerrainType.Spawn:
                        Gizmos.color = Color.blue;
                        break;
                }
                Gizmos.color = new Color(Gizmos.color.r, 
                    Gizmos.color.g, 
                    Gizmos.color.b,
                    alpha);
                Gizmos.DrawCube(position, Vector3.one * 0.1f);
                Gizmos.color = Color.green;
                if (cell.type == TerrainType.Ground)
                {
                    foreach (var pureSide in cell.sides)
                    {
                        var side = (Vector3)(Vector2)pureSide;
                        var offset = 3 / 8f;
                        var pos = position + side * offset * 0.1f;
                        var right = new Vector3(side.y, -side.x, 1) * 0.1f;
                        var size = side * 0.1f / 4 + right;
                        Gizmos.DrawCube(pos, size * 1.05f);
                    }
                }
            }
        }
    }
}

public class OutlinesData
{
    public readonly float scale;
    public readonly float step;
    public readonly Dictionary<Vector2Int, BlockOutline> outlines;
    public readonly Vector2 offset;
    public readonly System.Func<Vector2Int, Biome> GetBiome;

    public OutlinesData(float scale, float step, System.Func<Vector2Int, Biome> getBiome)
    {
        GetBiome = getBiome;
        this.scale = scale;
        this.step = step;
        outlines = new Dictionary<Vector2Int, BlockOutline>();
        offset = Random.insideUnitCircle * 10;
    }
}

[System.Serializable]
public class BiomeShadr
{
    public Biome biome;
    [Range(0, 1)]
    public float chance;
    [Range(0, 1)]
    public float distantTileChance;
    [Range(0, 1)]
    public float unTouchedDistance;
    public int radius;
    public bool SouldBeHere;
}

public enum TerrainType
{
    Ground, 
    Empty,
    Ladder,
    Spawn
}

public class TerrainCell
{
    public TerrainType type;
    public Vector2Int cord { get; private set; }
    public Biome biome;
    private HashSet<Vector2Int> freeSides = new HashSet<Vector2Int>();
    public bool hasBackground;
    public bool isInsideCliff => freeSides.Count == 0;

    public bool ContainsSide(Vector2Int side)
    {   
        return freeSides.Contains(side);
    }

    public IEnumerable<Vector2Int> sides => freeSides;

    public static TerrainCell Empty(Vector2Int cord)
    {
        var cell = new TerrainCell();
        cell.type = TerrainType.Empty;
        cell.cord = cord;
        return cell;
    }

    public void AddSide(Vector2Int side)
    {
        freeSides.Add(side);
    }

    public static TerrainCell Filled(Vector2Int cord)
    {
        var cell = new TerrainCell();
        cell.type = TerrainType.Ground;
        cell.cord = cord;
        return cell;
    }

    public override int GetHashCode()
    {
        return cord.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return cord.Equals(((TerrainCell)obj).cord);
    }
}