using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BlockOutline 
{
    private Dictionary<Vector2Int, SingleOutline> outlines = new Dictionary<Vector2Int, SingleOutline>();
    public bool isEmpty => GetSides().All(s => outlines[s].simple);

    public static IEnumerable<Vector2Int> GetSides()
    {
        yield return Vector2Int.up;
        yield return Vector2Int.right;
        yield return Vector2Int.down;
        yield return Vector2Int.left;
    }

    public BlockOutline(Dictionary<Vector2Int, SingleOutline> generatedSides)
    {
        foreach (var side in GetSides())
        {
            if (generatedSides.ContainsKey(side))
                outlines.Add(side, generatedSides[side]);
            else
                outlines.Add(side, new SingleOutline());
        }
    }

    public SingleOutline this[Vector2Int side]
    {
        get { return outlines[side]; }
    }
}

public class SingleOutline
{
    private int[] offsets;
    public readonly Color selfColor = Color.clear;
    public readonly Color otherColor = Color.clear;
    public bool simple = false;

    public SingleOutline()
    {
        offsets = new int[16];
        simple = true;
    }

    public SingleOutline(Vector2 offset, bool isHorizontal, float step, float scale, Color one, Color other)
    {
        selfColor = one;
        otherColor = other;
        offsets = new int[16];
        var vectorStep = (isHorizontal) ? Vector2.right : Vector2.up;
        for (var x = 0; x < 16; x++)
        {
            var multipluyer =1 - Mathf.Abs(x - 7.5f) / 7.5f;
            var position = offset + vectorStep * step * x;
            var rawValue = (Mathf.PerlinNoise(position.x, position.y) - 0.5f) * 2;
            offsets[x] = Mathf.RoundToInt(rawValue * scale);
        }
    }

    public SingleOutline(SingleOutline adjasent)
    {
        offsets = new int[16];
        selfColor = adjasent.otherColor;
        otherColor = adjasent.selfColor;
        for (var x = 0; x < 16; x++)
        {
            offsets[x] = -adjasent.offsets[x];
        }
        simple = adjasent.simple;
    }

    public SingleOutline(int[] offsets, Color c1, Color c2)
    {
        if (offsets.Length != 16)
            throw new System.ArgumentException("not correct offsets array");
        this.offsets = offsets;
        selfColor = c1;
        otherColor = c2;
    }

    public int GetOffset(int cord)
    {
        return offsets[cord];
    }
}
