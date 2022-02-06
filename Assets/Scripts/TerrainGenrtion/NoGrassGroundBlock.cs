using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "new no grass block", menuName = "game/blocks/no grass stone")]
public class NoGrassGroundBlock : GroundBlock
{
    [Header("No Grass Block")]
    public int NumberOfDifferentSprites;
    public int voronoiDiameter;
    public float scale;
    [Range(0, 8)]
    public int AlphaFallOffCutOut;

    [Range(0.01f, 1)]
    public float fallOffWeight;
    [Space]
    [Range(-0.0001f, 1)]
    public float ChanceToIgnoreFalloff;
    [Range(0, 1)]
    public float IgnoredFallOffWeight;

    private Voronoi voronoi;

    private bool isIgnoreFallOff = false;
    private float alphaCutOut => Mathf.Lerp(MinFallOff, 1, 1 - AlphaFallOffCutOut / 8f);
    public List<colorVariant> colorTypes;

    private float maxWeight;

    private List<Vector2Int> directions;

    private Vector2Int offset;

    private BlockOutline outline;

    private Sprite EmptySprite;
    private List<Sprite> ignoredFallOffSprites;

    private Dictionary<CordinateInformation, Sprite> sideSprites;
    

    public override void Initiate(Dictionary<string, Texture2D> secondaryTextures)
    {
        EmptySprite = null;
        ignoredFallOffSprites = new List<Sprite>();
        sideSprites = new Dictionary<CordinateInformation, Sprite>();
        base.Initiate(secondaryTextures);
        voronoi = new Voronoi(voronoiDiameter, voronoiDiameter, scale);
        maxWeight = colorTypes.Sum(c => c.weight);
    }

    public override void PlaceSprite(List<Vector2Int> directions, Vector2Int offset, 
        BlockOutline outline, TilePlacer placingInformation)
    {
        this.outline = outline;
        this.offset = offset;
        this.directions = directions;
        var sprite = GetSprite();
        var outlineSprite = GetOutlneSprite(outline);
        placingInformation.PlaceGround(sprite, offset);
        if (outlineSprite != null)
            placingInformation.PlaceGroundOutline(outlineSprite, offset);
    }

    private Sprite GetSprite()
    {
        isIgnoreFallOff = false;
        if (directions.Count == 0)
        {
            var value = UnityEngine.Random.value;
            isIgnoreFallOff = ChanceToIgnoreFalloff >= value;
            return GetInsideSprite();
        }
        return GetEdgeSprite();
    }

    private Sprite GetEdgeSprite()
    {
        var savedCord = new Vector2Int(offset.x % voronoiDiameter, offset.y % voronoiDiameter);
        var cordInfo = new CordinateInformation(savedCord, directions);
        if (!sideSprites.ContainsKey(cordInfo))
            sideSprites[cordInfo] = CreateSprite();
        return sideSprites[cordInfo];
    }

    private Sprite GetInsideSprite()
    {
        if (!isIgnoreFallOff)
        {
            if (EmptySprite == null)
                EmptySprite = CreateSprite();
            return EmptySprite;
        }
        if (NumberOfDifferentSprites <= 0 ||
            ignoredFallOffSprites.Count < NumberOfDifferentSprites)
        {
            var sprite = CreateSprite();
            ignoredFallOffSprites.Add(sprite);
            return sprite;
        }
        return ignoredFallOffSprites[UnityEngine.Random.Range(0, ignoredFallOffSprites.Count)];
    }

    private Sprite CreateSprite()
    {
        var tex = GenerateTexture();
        var rect = new Rect(0, 0, tex.width, tex.height);
        var pivot = Vector2.one / 2;
        var sprite = Sprite.Create(tex, rect, pivot, 16);
        var shape = new List<Vector2[]>();
        shape.Add(GetPhysicsShape(rect));
        sprite.OverridePhysicsShape(shape);
        return sprite;
    }

    private Vector2[] GetPhysicsShape(Rect spriteRect)
    {
        var standartShape = new Vector2[] {
            new Vector2(0, 0),
            new Vector2(0, spriteRect.height),
            new Vector2(spriteRect.width, spriteRect.height),
            new Vector2(spriteRect.width, 0)
        };

        Func<Vector2Int, int[]> getSides = dir => 
        {
            if (dir == Vector2Int.left)
                return new int[] { 0, 1 };
            if (dir == Vector2.right)
                return new int[] { 2, 3 };
            if (dir == Vector2.up)
                return new int[] { 1, 2 };
            if (dir == Vector2.down)
                return new int[] { 0, 3 };
            return new int[0];
        };
        
        foreach (var d in directions)
        {
            var side = getSides(d);
            foreach (var vertexIndex in side)
            {
                standartShape[vertexIndex] += -d * AlphaFallOffCutOut;
            }
        }

        return standartShape;
    }

    private Texture2D GenerateTexture()
    {
        var tex = new Texture2D(16, 16);
        for (var x = 0; x < 16; x++)
        {
            for (var y = 0; y < 16; y++)
            {
                var pixel = new Vector2Int(x, y);
                var c = GetColor(pixel);
                //var c = Color.Lerp(Color.black, Color.white, Mathf.Pow(fallOffValue, fallOffWeight));

                tex.SetPixel(x, y, c);
            }
        }
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        return tex;
    }

    private float GetValue(Vector2Int pixel)
    {
        var uv = (Vector2)pixel / 15;
        return (1 - voronoi.GetValue(uv + offset)) * maxWeight;
    }

    private float GetFallOff(Vector2Int pixel)
    {
        var uv = (Vector2)pixel / 15;
        var dirs = directions;
        if (isIgnoreFallOff)
        {
            dirs = new List<Vector2Int>(new Vector2Int[]
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            });
        }
        var fallOffValue = GetFallOff(uv, dirs);
        return (isIgnoreFallOff) ? 1 - fallOffValue : fallOffValue;
    }

    private IEnumerable<Vector2Int> GetNeighbours(Vector2Int pixel)
    {
        if (pixel.x != 0)
            yield return new Vector2Int(pixel.x - 1, pixel.y);
        if (pixel.y != 0)
            yield return new Vector2Int(pixel.x, pixel.y - 1);
        if (pixel.x != 15)
            yield return new Vector2Int(pixel.x + 1, pixel.y);
        if (pixel.y != 15)
            yield return new Vector2Int(pixel.x, pixel.y + 1);
    }

    private Color GetColor(Vector2Int pixel)
    {
        var current = 0f;
        var rawValue = GetValue(pixel);
        var fallOff = GetFallOff(pixel);
        var value = rawValue * Mathf.Pow(fallOff, (isIgnoreFallOff)? IgnoredFallOffWeight : fallOffWeight);
        foreach (var c in colorTypes)
        {
            if (current <= value && current + c.weight > value)
            {
                if (fallOff > alphaCutOut && c.canBeTransparent && !isIgnoreFallOff)
                    return Color.clear;
                if (c.useTranslation && 
                    (pixel.x == 0 || 
                    pixel.y == 0 || 
                    pixel.x == 15 ||
                    pixel.y == 15 ||
                    GetNeighbours(pixel).Any(n =>
                {
                    var rv = GetValue(n);
                    var f = GetFallOff(n);
                    var v = rv * Mathf.Pow(f, fallOffWeight);
                    return (current > v || current + c.weight <= v);
                })))
                    return c.translationColor;
                return c.color;
            }
            current += c.weight;
        }
        return colorTypes[colorTypes.Count - 1].color;
    }
}

[System.Serializable]
public class colorVariant
{
    public Color color;
    public bool canBeTransparent;
    [Range(0, 1)]
    public float weight;
    public bool useTranslation;
    public Color translationColor;
}
