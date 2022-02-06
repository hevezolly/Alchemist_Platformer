using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "new standart grass", menuName = "game/grass/standart")]
public class StandartGrassGenerator : GrassGenerator
{
    public int NumberOfDifferentSprites;
    [Range(0.01f, 9)]
    public float height;
    [Range(0, 1)]
    public float EmptyBedChance;
    private Vector2Int side;
    private Vector2 offset;
    [Range(0, 1)]
    public float emptyWeight;
    public Vector2Int bedOverlay;
    public Color color;
    private float summWeight;

    private Dictionary<Vector2Int, float> values;

    private List<Vector2Int> bedPixels;
    private List<int> grassLength;

    private Dictionary<Vector2Int, Dictionary<Sprite, SingleOutline>> savedSprites;
 
    public override void Initiate(Dictionary<string, Texture2D> st)
    {
        base.Initiate(st);
        savedSprites = new Dictionary<Vector2Int, Dictionary<Sprite, SingleOutline>>();
        savedSprites[Vector2Int.up] = new Dictionary<Sprite, SingleOutline>();
        savedSprites[Vector2Int.down] = new Dictionary<Sprite, SingleOutline>();
        savedSprites[Vector2Int.left] = new Dictionary<Sprite, SingleOutline>();
        savedSprites[Vector2Int.right] = new Dictionary<Sprite, SingleOutline>();
    }

    public override Sprite GenerateGrass(Vector2Int cord, Vector2Int side, out SingleOutline outline)
    {
        if (NumberOfDifferentSprites <= 0 ||
            savedSprites[side].Count < NumberOfDifferentSprites)
        {
            var sprite = CreateNewSprite(side, out outline);
            savedSprites[side][sprite] = outline;
            return sprite;
        }
        else
        {
            var all = savedSprites[side].Keys.ToList();
            var chousen = all[Random.Range(0, all.Count)];
            outline = savedSprites[side][chousen];
            return chousen;
        }
    }

    private Sprite CreateNewSprite(Vector2Int side, out SingleOutline outline)
    {
        values = new Dictionary<Vector2Int, float>();
        bedPixels = new List<Vector2Int>();
        grassLength = new List<int>();
        this.side = side;
        var texture = GetTexture();
        var rect = new Rect(0, 0, texture.width, texture.height);
        var pivot = Vector2.one / 2;
        var sprite = Sprite.Create(texture, rect, pivot, 16);
        OverrideGeometry(sprite);
        outline = GenerateOutline();
        return sprite;
    }

    private void OverrideGeometry(Sprite sprite)
    {
        Vector2Int forward;
        if (side.x == 0)
            forward = Vector2Int.right;
        else
            forward = Vector2Int.up;

        var vertices = new Vector2[4 * grassLength.Count + 4];
        var tringles = new ushort[3 * vertices.Length];
        var index = 0;
        var tringlesIndex = 0;
        var offset = Vector2Int.zero;
        if (side.x < 0 || side.y < 0)
            offset = -side;
        for (var i = 0; i < bedPixels.Count; i++)
        {
            var bedPixel = bedPixels[i];
            vertices[index] = bedPixel + offset;
            vertices[index + 1] = bedPixel + side * grassLength[i] + offset;
            vertices[index + 2] = bedPixel + side * grassLength[i] + forward + offset;
            vertices[index + 3] = bedPixel + forward + offset;

            tringles[tringlesIndex] = (ushort)index;
            tringles[tringlesIndex + 3] = (ushort)index;
            if (side.x >= 0 && side.y <= 0)
            {
                tringles[tringlesIndex + 1] = (ushort)(index + 1);
                tringles[tringlesIndex + 2] = (ushort)(index + 2);
                tringles[tringlesIndex + 4] = (ushort)(index + 2);
                tringles[tringlesIndex + 5] = (ushort)(index + 3);
            }
            else
            {
                tringles[tringlesIndex + 2] = (ushort)(index + 1);
                tringles[tringlesIndex + 1] = (ushort)(index + 2);
                tringles[tringlesIndex + 5] = (ushort)(index + 2);
                tringles[tringlesIndex + 4] = (ushort)(index + 3);
            }
            index += 4;
            tringlesIndex += 6;
        }
        var start = Vector2.zero;
        if (side == Vector2Int.down)
            start = new Vector2(0, 15);
        if (side == Vector2Int.left)
            start = new Vector2(15, 0);
        vertices[index] = start + offset;
        vertices[index + 1] = start+ side + offset;
        vertices[index + 2] = start + side + forward * 15 + offset;
        vertices[index + 3] = start + forward * 15 + offset;
        tringles[tringlesIndex] = (ushort)index;
        tringles[tringlesIndex + 3] = (ushort)index;
        if (side.x >= 0 && side.y <= 0)
        {
            tringles[tringlesIndex + 1] = (ushort)(index + 1);
            tringles[tringlesIndex + 2] = (ushort)(index + 2);
            tringles[tringlesIndex + 4] = (ushort)(index + 2);
            tringles[tringlesIndex + 5] = (ushort)(index + 3);
        }
        else
        {
            tringles[tringlesIndex + 2] = (ushort)(index + 1);
            tringles[tringlesIndex + 1] = (ushort)(index + 2);
            tringles[tringlesIndex + 5] = (ushort)(index + 2);
            tringles[tringlesIndex + 4] = (ushort)(index + 3);
        }
        sprite.OverrideGeometry(vertices, tringles);
    }

    private SingleOutline GenerateOutline()
    {
        var outline = new int[16];
        for (var x = 0; x < 16; x++)
        {
            if (CheckForEmptiness(GetValue(bedPixels[x]), true))
            {
                outline[x] = 0;
                continue;
            }
            outline[x] = Random.Range(bedOverlay.x, bedOverlay.y);
        }
        return new SingleOutline(outline, color, Color.clear);
    }

    private Texture2D GetTexture()
    {
        offset = Random.insideUnitCircle * 10;
        var tex = new Texture2D(16, 16);
        for (var x = 0; x < 16; x++)
        {
            for (var y = 0; y < 16; y++)
            {
                var pixel = new Vector2Int(x, y);

                tex.SetPixel(x, y, GetColor(pixel));
                
            }
        }
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        return tex;
    }

    private Color GetColor(Vector2Int pixel)
    {
        var uv = (Vector2)pixel / 16;
        var fallOff = GetFallOff(side, uv);
        var pseudoPixel = GetPseudoPixel(pixel);
        if (!bedPixels.Contains(pseudoPixel))
        {
            bedPixels.Add(pseudoPixel);
            grassLength.Add(0);
        }
        var grassIndex = bedPixels.IndexOf(pseudoPixel);
        var rowValue = GetValue(pseudoPixel);
        if (CheckForEmptiness(rowValue, pseudoPixel == pixel))
            rowValue = 0;
        var value = Mathf.Pow(rowValue, height);
        if (value > fallOff)
            rowValue = 0;
        rowValue *= fallOff;
        if (rowValue > 0)
        {
            if ((pseudoPixel - pixel).magnitude > grassLength[grassIndex])
                grassLength[grassIndex] = (int)(pseudoPixel - pixel).magnitude;
            return color;
        }
        return Color.clear;
    }

    private bool CheckForEmptiness(float value, bool isbed)
    {
        return value < emptyWeight * ((isbed) ? EmptyBedChance : 1);
    }

    private Vector2Int GetPseudoPixel(Vector2Int pixel)
    {
        if (side == Vector2Int.up)
            return new Vector2Int(pixel.x, 0);
        if (side == Vector2Int.down)
            return new Vector2Int(pixel.x, 15);
        if (side == Vector2Int.left)
            return new Vector2Int(15, pixel.y);
        return new Vector2Int(0, pixel.y);
    }

    private float GetValue(Vector2Int pixel)
    {
        if (!values.ContainsKey(pixel))
            values[pixel] = Random.value;
        return values[pixel];
    }
}

[System.Serializable]
public class GrassColor
{
    [Range(0, 1)]
    public float weight;
    public Color color;
}
