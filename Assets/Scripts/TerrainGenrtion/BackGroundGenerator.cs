using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BackGroundGenerator : MonoBehaviour
{
    public bool Draw;
    [Range(0, 1)]
    public float FillPercent;

    public int SmoothNumber;

    private int[,] generatedMap;

    private const float NeighbourPercent = 0.5f;
    private const int MaxNeighbours = 2;

    private TerrainGenerator generator;

    public readonly Dictionary<Vector2Int, bool> backGround = new Dictionary<Vector2Int, bool>();

    private void Start()
    {
        generator = GetComponent<TerrainGenerator>();
        if (!Draw)
            return;
        generator.GenerateMap();
        GenerateBackground(generator.RawMap);
    }

    private void Update()
    {
        if (!Draw)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            backGround.Clear();
            generator.GenerateMap();
            GenerateBackground(generator.RawMap);
        }
            
    }

    public void GenerateBackground(int[,] generatedMap)
    {
        this.generatedMap = generatedMap;
        SetBackground();
        for (var i = 0; i < SmoothNumber; i++)
        {
            SmoothMap();
        }
        RemoveDisconectedPices();
    }

    private void RemoveDisconectedPices()
    {
        var checkedCords = new HashSet<Vector2Int>();
        var pointsToDelete = new List<Vector2Int>();
        foreach (var cord in backGround.Keys
            .Where(c => backGround[c])
            .Where(c => !checkedCords.Contains(c)))
        {
            var region = new List<Vector2Int>();
            var pointsToOpen = new Queue<Vector2Int>();
            pointsToOpen.Enqueue(cord);
            var delete = true;
            while (pointsToOpen.Count != 0)
            {
                var pointToCheck = pointsToOpen.Dequeue();
                region.Add(pointToCheck);
                checkedCords.Add(pointToCheck);
                foreach (var n in GetNeighbours(pointToCheck))
                {
                    if (checkedCords.Contains(n))
                        continue;
                    if (generatedMap[n.x, n.y] == 1)
                        delete = false;
                    else
                        pointsToOpen.Enqueue(n);
                }
            }
            if (delete)
                pointsToDelete.AddRange(region);
        }
        foreach (var pointToDelete in pointsToDelete)
        {
            backGround[pointToDelete] = false;
        }
    }

    private IEnumerable<Vector2Int> GetNeighbours(Vector2Int n)
    {
        for (var x = -1; x <= 1; x++)
        {
            for (var y = -1; y <= 1; y++)
            {
                if (Mathf.Abs(x) == Mathf.Abs(y))
                    continue;
                var cord = n + new Vector2Int(x, y);
                if (CheckCord(cord))
                    yield return cord;
            }
        }
    }

    private void SetBackground()
    {
        var backgroundCords = new HashSet<Vector2Int>();
        for (var x = 0; x < generatedMap.GetLength(0); x++)
        {
            for (var y = 0; y < generatedMap.GetLength(1); y++)
            {
                if (generatedMap[x, y] == 1)
                    continue;
                backGround.Add(new Vector2Int(x, y), Random.value <= FillPercent);
            }
        }
    }

    private void SmoothMap()
    {
        var cellsToChange = new HashSet<Vector2Int>();
        foreach (var cord in backGround.Keys)
        {
            var count = GetNeighboursCount(cord);
            if (count > MaxNeighbours * NeighbourPercent && !backGround[cord])
                cellsToChange.Add(cord);
            else if (count < MaxNeighbours * NeighbourPercent && backGround[cord])
                cellsToChange.Add(cord);
        }
        foreach (var cellToChange in cellsToChange)
        {
            backGround[cellToChange] = !backGround[cellToChange];
        }
    }

    private int GetNeighboursCount(Vector2Int n)
    {
        var count = 0;
        /*for (var x = -1; x <= 1; x++)
        {
            for (var y = -1; y <= 1; y++)
            {
                if (x == y && x == 0)
                    continue;
                var cord = n + new Vector2Int(x, y);
                if (CheckCord(cord))
                    count++;
            }
        }*/
        var addCord = n + Vector2Int.up;
        if (CheckCord(addCord))
            count++;
        addCord = n - Vector2Int.up;
        if (CheckCord(addCord))
            count++;
        return count;
    }

    private bool CheckCord(Vector2Int cord)
    {
        return (backGround.ContainsKey(cord) && backGround[cord]) ||
                    (cord.x >= 0 && cord.y >= 0 &&
                    cord.x < generatedMap.GetLength(0) &&
                    cord.y < generatedMap.GetLength(1) &&
                    generatedMap[cord.x, cord.y] == 1);
    }

    private void OnDrawGizmos()
    {
        if (!Draw || generatedMap == null)
            return;
        for (var x = 0; x < generatedMap.GetLength(0); x++)
        {
            for (var y = 0; y < generatedMap.GetLength(1); y++)
            {
                if (generatedMap[x, y] == 1)
                    Gizmos.color = Color.black;
                else if (backGround[new Vector2Int(x, y)])
                    Gizmos.color = Color.gray;
                else
                    Gizmos.color = Color.white;
                Gizmos.DrawCube(transform.position + new Vector3(x, y) * 0.1f, Vector3.one * 0.1f);
            }
        }
    }
}
