using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Voronoi
{
    private Dictionary<Vector2Int, VoronoiCell> grid = new Dictionary<Vector2Int, VoronoiCell>();
    private readonly float MaxDistance = Mathf.Sqrt(2);
    private readonly Vector2Int widthHeight;
    private readonly float scale;

    public Voronoi(int width, int height, float scale)
    {
        widthHeight = new Vector2Int(width, height);
        this.scale = scale;
    }

    private IEnumerable<System.Tuple<Vector2Int, Vector2Int>> GetNeighbours(Vector2Int cord)
    {
        for (var x = - 1; x <= + 1; x++)
        {
            for (var y = - 1; y <= + 1; y++)
            {
                if (x != 0 || y != 0)
                {
                    var virt = GetVirtualCord(cord.x + x, cord.y + y);
                    var dir = new Vector2Int(x, y);
                    yield return new System.Tuple<Vector2Int, Vector2Int>(
                        virt, dir);
                }
            }
        }
    }

    private Vector2 GetVirtualPos(Vector2 cord)
    {
        return new Vector2(mod(cord.x, (int)(widthHeight.x * scale)), mod(cord.y, (int)(widthHeight.y * scale)));
    }

    private Vector2Int GetVirtualCord(int x, int y)
    {
        return new Vector2Int(mod(x, (int)(widthHeight.x * scale)), mod(y, (int)(widthHeight.y * scale)));
    }

    private int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    private float mod(float x, float m)
    {
        return (x % m + m) % m;
    }

    public float GetValue(Vector2 givenPos)
    {
        var pos = GetVirtualPos(givenPos * scale);
        var cellCord = Vector2Int.FloorToInt(pos);
        GenerateCell(cellCord);
        var minDistance = GetDistance(pos, grid[cellCord], Vector2Int.zero);
        foreach (var n in GetNeighbours(cellCord))
        {
            var currentDist = GetDistance(pos, grid[n.Item1], n.Item2);
            if (currentDist < minDistance)
                minDistance = currentDist;
        }
        return Mathf.Clamp01(minDistance);
    }

    private float GetDistance(Vector2 pos, VoronoiCell cell, Vector2Int side)
    {
        var cord = Vector2Int.FloorToInt(pos);
        var newCord = cord + side;
        return Vector2.Distance(pos, newCord + cell.centroidCord);
    }

    private void GenerateCell(Vector2Int cord, bool continueToNeighbours = true)
    {
        if (!grid.ContainsKey(cord))
        {
            var cell = new VoronoiCell(cord);
            grid[cord] = cell;
        }
        if (continueToNeighbours)
        {
            foreach (var n in GetNeighbours(cord))
            {
                GenerateCell(n.Item1, false);
            }
        }
    }
}

public class VoronoiCell
{
    public readonly Vector2Int cellCord;
    public readonly Vector2 centroidCord;

    public VoronoiCell(Vector2Int cord)
    {
        cellCord = cord;
        centroidCord = new Vector2(Random.value, Random.value);
    }

    public Vector2 position => cellCord + centroidCord;
}
