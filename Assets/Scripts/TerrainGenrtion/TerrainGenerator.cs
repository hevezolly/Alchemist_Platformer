using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TerrainGenerator : MonoBehaviour
{
    public bool draw;
    public Texture2D multyplyTexture;
    [Range(0, 1)]
    public float TextureWeight;
    public Vector2Int WidthHeight;
    public int Seed;
    public bool UseRandomSeed;

    [Space]
    [Range(0, 1)]
    public float fillPercent;
    public int SmoothNumber;
    public int SmoothRadius = 1;
    [Range(0, 1)]
    public float SmoothCountValue;
    private int maxNeighbours => (SmoothRadius * 2 + 1) * (SmoothRadius * 2 + 1) - 1;

    [Space]
    public bool GeneratePassages;
    public int PassageThickness;

    [Space]
    public int WallThreshold;
    public int EmptyThreshold;

    [Space]
    public float IslendsDistance;
    [Range(0, 1)]
    public float IslandCellPropability;
    public int IslandSize;


    public List<Room> rooms { get; private set; }

    public int[,] RawMap { get; private set; }

    public System.Random mainRandom { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        if (draw)
        {
            if (Input.GetMouseButtonDown(0))
                GenerateMap();
        }
    }

    public void GenerateMap()
    {
        if (UseRandomSeed)
            Seed = UnityEngine.Random.Range(0, 9999999);
        mainRandom = new System.Random(Seed);
        RawMap = GenerateNoiseMap();
        for (var i = 0; i < SmoothNumber; i++)
        {
            SmoothNoiseMap();
        }
        SetIslands();
        RemoveSmallAreas(0, EmptyThreshold);
        RemoveSmallAreas(1, WallThreshold);
        if (!GeneratePassages)
            return;
        rooms = GetRooms();
        if (rooms.Count == 0)
            return;
        rooms.Sort();
        rooms[0].IsMainRoom = true;
        rooms[0].IsAccessibleFromMainRoom = true;
        ConnectClosestRooms();
        RemoveSmallAreas(1, WallThreshold);
    }

    private void ConnectClosestRooms(bool forceAccessibility = false)
    {
        var roomListA = new List<Room>();
        var roomListB = new List<Room>();

        if (forceAccessibility)
        {
            foreach (var r in rooms)
            {
                if (r.IsAccessibleFromMainRoom)
                    roomListB.Add(r);
                else
                    roomListA.Add(r);
            }
        }
        else
        {
            roomListA = rooms;
            roomListB = rooms;
        }

        var bestDistance = 0;
        var bestCordA = new Vector2Int();
        var bestCordB = new Vector2Int();
        var bestRoomA = new Room();
        var bestRoomB = new Room();
        var possibleConnectionFound = false;

        foreach (var roomA in roomListA)
        {
            if (!forceAccessibility)
            {
                possibleConnectionFound = false;
                if (roomA.connectedRooms.Count > 0)
                    continue;
            }
            foreach (var roomB in roomListB)
            {
                if (roomB == roomA || roomA.IsConnected(roomB))
                    continue;
                foreach(var edgeA in roomA.edgeTiles)
                {
                    foreach (var edgeB in roomB.edgeTiles)
                    {
                        var dist = (int)Vector2Int.Distance(edgeA, edgeB);
                        if (dist < bestDistance || !possibleConnectionFound)
                        {
                            bestDistance = dist;
                            possibleConnectionFound = true;
                            bestCordA = edgeA;
                            bestCordB = edgeB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }
            if (possibleConnectionFound && !forceAccessibility)
            {
                CreatePassage(bestRoomA, bestRoomB, bestCordA, bestCordB);
            }
        }

        if (possibleConnectionFound && forceAccessibility)
        {
            CreatePassage(bestRoomA, bestRoomB, bestCordA, bestCordB);
            ConnectClosestRooms(true);
        }

        if (!forceAccessibility)
            ConnectClosestRooms(true);
    }

    private void CreatePassage(Room roomA, Room roomB, Vector2Int tileA, Vector2Int tileB)
    {
        Room.ConnectRooms(roomA, roomB);
        var line = GetLine(tileA, tileB);
        foreach (var cord in line)
        {
            EmptyCircle(cord, PassageThickness);
        }
    }

    void EmptyCircle(Vector2Int center, int radius)
    {
        for (var x = -radius; x <= radius; x++)
        {
            for (var y = -radius; y <= radius; y++)
            {
                if (x*x + y*y <= radius * radius)
                {
                    var cord = new Vector2Int(x, y) + center;
                    if (x == 0 || y == 0 || 
                        x == WidthHeight.x - 1 || 
                        y == WidthHeight.y - 1)
                        continue;
                    if (IsInRange(cord.x, cord.y))
                        RawMap[cord.x, cord.y] = 0;
                }
            }
        }
    }

    private List<Vector2Int> GetLine(Vector2Int from, Vector2Int to)
    {
        var cords = new List<Vector2Int>();
        var delta = to - from;

        var x = from.x;
        var y = from.y;

        var step = Math.Sign(delta.x);
        var gradientStep = Math.Sign(delta.y);

        var inverted = false;
        var longest = Math.Abs(delta.x);
        var shortest = Math.Abs(delta.y);

        if (longest < shortest)
        {
            inverted = true;
            longest = Math.Abs(delta.y);
            shortest = Math.Abs(delta.x);

            step = Math.Sign(delta.y);
            gradientStep = Math.Sign(delta.x);
        }

        var gradientAccumulation = longest / 2;
        for (var i = 0; i < longest; i++)
        {
            cords.Add(new Vector2Int(x, y));
            if (inverted)
                y += step;
            else
                x += step;

            gradientAccumulation += shortest;
            if (gradientAccumulation >= longest)
            {
                if (inverted)
                    x += gradientStep;
                else
                    y += gradientStep;
                gradientAccumulation -= longest;
            }

        }
        return cords;
    }

    private List<Room> GetRooms()
    {
        var areas = GetRegionsOfType(0);
        var rooms = new List<Room>();
        foreach (var area in areas)
        {
            rooms.Add(new Room(area, RawMap));
        }
        return rooms;
    }

    private void SetIslands()
    {
        for (var x = 0; x < WidthHeight.x; x++)
        {
            for (var y = 0; y < WidthHeight.y; y++)
            {
                if (!IsAloneAtDistance(x, y, IslendsDistance))
                    continue;
                var value = mainRandom.NextDouble();
                GenerateIsland(x, y, IslandSize);
            }
        }
    }

    private void RemoveSmallAreas(int type, int threshold)
    {
        var walls = GetRegionsOfType(type);
        foreach (var r in walls)
        {
            if (r.Count < threshold)
            {
                foreach (var cord in r)
                {
                    RawMap[cord.x, cord.y] = 1 - type;
                }
            }
        }
    }

    private List<HashSet<Vector2Int>> GetRegionsOfType(int type)
    {
        var regions = new List<HashSet<Vector2Int>>();
        for (var x = 0; x < WidthHeight.x; x++)
        {
            for (var y = 0; y < WidthHeight.y; y++)
            {
                if (RawMap[x, y] != type)
                    continue;
                if (regions.Any(r => r.Contains(new Vector2Int(x, y))))
                    continue;
                regions.Add(GetRegion(x, y));
            }
        }
        return regions;
    }

    private HashSet<Vector2Int> GetRegion(int x, int y)
    {
        var visited = new HashSet<Vector2Int>();
        var mapFlags = new int[WidthHeight.x, WidthHeight.y];
        var tileType = RawMap[x, y];

        var queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(x, y));
        mapFlags[x, y] = 1;
        
        while(queue.Count > 0)
        {
            var tile = queue.Dequeue();
            visited.Add(tile);

            foreach (var n in GetStraightNeighbours(tile))
            {
                if (mapFlags[n.x, n.y] == 0 && RawMap[n.x, n.y] == tileType)
                {
                    mapFlags[n.x, n.y] = 1;
                    queue.Enqueue(n);
                }
            }
        }
        return visited;
    }

    public IEnumerable<Vector2Int> GetStraightNeighbours(Vector2Int cord)
    {
        if (cord.x > 0)
            yield return new Vector2Int(cord.x - 1, cord.y);
        if (cord.y < WidthHeight.y - 1)
            yield return new Vector2Int(cord.x, cord.y + 1);
        if (cord.x < WidthHeight.x - 1)
            yield return new Vector2Int(cord.x + 1, cord.y);
        if (cord.y > 0)
            yield return new Vector2Int(cord.x, cord.y - 1);
        yield break;
    }

    private void GenerateIsland(int x, int y, int size)
    {
        var pointsToOpen = new Queue<Vector2Int>();
        var failedPoints = new List<Vector2Int>();
        pointsToOpen.Enqueue(new Vector2Int(x, y));
        while (size > 0)
        {
            if (pointsToOpen.Count == 0)
                break;
            var currentPoint = pointsToOpen.Dequeue();
            RawMap[currentPoint.x, currentPoint.y] = 1;
            size--;
            foreach (var n in GetNeighbours(currentPoint))
            {
                if (n.x <= 0 ||
                    n.x >= WidthHeight.x - 1 ||
                    n.y <= 0 ||
                    n.y >= WidthHeight.y - 1)
                    continue;
                if (RawMap[n.x, n.y] == 1)
                    continue;
                var value = mainRandom.NextDouble();
                if (value <= IslandCellPropability)
                    pointsToOpen.Enqueue(n);
                else
                    failedPoints.Add(n);
            }
        }
        foreach (var p in failedPoints)
        {
            if (GetNeighboursCount(p.x, p.y) >= 3)
                RawMap[p.x, p.y] = 1;
        }
    }

    private IEnumerable<Vector2Int> GetNeighbours(Vector2Int point)
    {
        for (var x = point.x - 1; x <= point.x + 1; x++)
        {
            for (var y = point.y - 1; y <= point.y + 1; y++)
            {
                if (x == point.x && y == point.y)
                    continue;
                if (x < 0 || y < 0 || x >= WidthHeight.x || y >= WidthHeight.y)
                    continue;
                yield return new Vector2Int(x, y);
            }
        }
    }

    private bool IsAloneAtDistance(int centerX, int centerY, float dist)
    {
        var center = new Vector2(centerX, centerY);
        for (var x = centerX - (int)dist; x <= centerX + (int)dist; x++)
        {
            for (var y = centerY - (int)dist; y <= centerY + (int)dist; y++)
            {
                if (!IsInRange(x, y))
                    continue;
                var point = new Vector2(x, y);
                var currentDist = Vector2.Distance(center, point);
                if (currentDist > dist)
                    continue;
                if (RawMap[x, y] == 1)
                    return false;
            }
        }
        return true;
    }

    private int[,] GenerateNoiseMap()
    {
        var map = new int[WidthHeight.x, WidthHeight.y];
        for (var x = 0; x < WidthHeight.x; x++)
        {
            for (var y = 0; y < WidthHeight.y; y++)
            {
                var uv = new Vector2((float)x / WidthHeight.x, (float)y / WidthHeight.y);
                var rawTextureValue = multyplyTexture.GetPixelBilinear(uv.x, uv.y).r;
                var textureValue = Mathf.Lerp(1, Mathf.Clamp01(rawTextureValue), TextureWeight);
                var value = (float)mainRandom.NextDouble() * textureValue;
                map[x, y] = (value < fillPercent) ? 1 : 0;
            }
        }
        return map;
    }

    private void SmoothNoiseMap()
    {
        var smoothValue = maxNeighbours * SmoothCountValue;
        var candidates = new Dictionary<Vector2Int, int>();
        for (var x = 0; x < WidthHeight.x; x++)
        {
            for (var y = 0; y < WidthHeight.y; y++)
            {
                var neighbours = GetNeighboursCount(x, y, SmoothRadius);
                if (neighbours > smoothValue)
                    candidates.Add(new Vector2Int(x, y), 1);
                else if (neighbours < smoothValue)
                    candidates.Add(new Vector2Int(x, y), 0);
            }
        }
        foreach (var cord in candidates.Keys)
        {
            RawMap[cord.x, cord.y] = candidates[cord];
        }
    }

    private int GetNeighboursCount(int centerX, int centerY, int radius = 1)
    {
        if (centerX == 0 ||
            centerX == WidthHeight.x - 1 ||
            centerY == 0 ||
            centerY == WidthHeight.y - 1)
            return (radius * 2 + 1) * (radius * 2 + 1) - 1;
        var count = 0;
        for (var x = centerX - radius; x <= centerX + radius; x++)
        {
            for (var y = centerY - radius; y <= centerY + radius; y++)
            {
                if (x == centerX && y == centerY)
                    continue;
                if (!IsInRange(x, y))
                    continue;
                count += RawMap[x, y];
            }
        }
        return count;
    }

    public bool IsInRange(int centerX, int centerY)
    {
        return !(centerX < 0 ||
            centerX > WidthHeight.x - 1 ||
            centerY < 0 ||
            centerY > WidthHeight.y - 1);
    }

    private void OnDrawGizmos()
    {
        if (RawMap == null || !draw)
            return;
        for (var x = 0; x < WidthHeight.x; x++)
        {
            for (var y = 0; y < WidthHeight.y; y++)
            {
                if (RawMap[x, y] == 0)
                    Gizmos.color = Color.white;
                else
                    Gizmos.color = Color.black;
                Gizmos.DrawCube(transform.position + new Vector3(x, y) * 0.1f, Vector3.one * 0.1f);
            }
        }
    }
}

public class Room: System.IComparable
{
    public HashSet<Vector2Int> tiles;
    public List<Vector2Int> edgeTiles;
    public HashSet<Room> connectedRooms;

    public Vector2Int center;
    public int RoomSize { get; private set; }
    public bool IsAccessibleFromMainRoom;
    public bool IsMainRoom;

    public Room() { }

    public Room(IEnumerable<Vector2Int> roomTiles, int[,] map)
    {
        tiles = new HashSet<Vector2Int>(roomTiles);
        RoomSize = tiles.Count;
        connectedRooms = new HashSet<Room>();

        var avgX = (int)roomTiles.Average(c => c.x);
        var avgY = (int)roomTiles.Average(c => c.y);

        center = new Vector2Int(avgX, avgY);

        edgeTiles = new List<Vector2Int>();
        foreach (var cord in tiles)
        {
            for (var x = cord.x - 1; x <= cord.x + 1; x++)
            {
                for (var y = cord.y - 1; y <= cord.y + 1; y++)
                {
                    if (x == cord.x || y == cord.y)
                    {
                        if ((x < 0 ||
                            x > map.GetLength(0) - 1 ||
                            y < 0 ||
                            y > map.GetLength(1) - 1))
                            continue;
                        if (map[x, y] == 1)
                            edgeTiles.Add(cord);
                    }
                }
            }
        }
    }

    public void SetAccessibility()
    {
        if (IsAccessibleFromMainRoom)
            return;
        IsAccessibleFromMainRoom = true;
        foreach (var r in connectedRooms)
        {
            r.IsAccessibleFromMainRoom = true;
        }
    }

    public static void ConnectRooms(Room A, Room B)
    {
        if (A.IsAccessibleFromMainRoom)
            B.SetAccessibility();
        else if (B.IsAccessibleFromMainRoom)
            A.SetAccessibility();
        A.connectedRooms.Add(B);
        B.connectedRooms.Add(A);
    }

    public bool IsConnected(Room other)
    {
        return connectedRooms.Contains(other);
    }

    public int CompareTo(object obj)
    {
        return RoomSize.CompareTo(((Room)obj).RoomSize);
    }
}
