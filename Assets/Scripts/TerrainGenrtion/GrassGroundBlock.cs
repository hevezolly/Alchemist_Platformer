using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "new grass block", menuName = "game/blocks/grass block")]
public class GrassGroundBlock : GroundBlock
{
    
    [Header("Grass Block")]
    public int NumberOfDifferentSprites;
    public int voronoiDiameter;
    public float scale;
    [Range(0.01f, 1)]
    public float FallOffWeight;

    [Range(-0.0001f, 1)]
    public float ChanceToIgnoreFalloff;
    [Range(0, 1)]
    public float IgnoredFallOffWeight;

    public GrassGenerator topGrass;

    public GrassGenerator leftGrass;

    public GrassGenerator rightGrass;

    public GrassGenerator bottomGrass;

    public List<grassColorVariant> colorTypes;

    private Voronoi voronoi;

    private Vector2Int cordinate;

    private float summWeight;

    private List<Vector2Int> directions;

    private BlockOutline outlines;

    private BlockOutline grassOutline;

    private bool isIgnoreFallOff;

    private Sprite EmptySprite;
    private List<Sprite> ignoredFallOffSprites;
    private Dictionary<CordinateInformation, Sprite> sideSprites;

    public override void Initiate(Dictionary<string, Texture2D> secondaryTextures)
    {
        base.Initiate(secondaryTextures);
        sideSprites = new Dictionary<CordinateInformation, Sprite>();
        voronoi = new Voronoi(voronoiDiameter, voronoiDiameter, scale);
        summWeight = colorTypes.Sum(c => c.weight);
        topGrass.Initiate(secondaryTextures);
        leftGrass.Initiate(secondaryTextures);
        rightGrass.Initiate(secondaryTextures);
        bottomGrass.Initiate(secondaryTextures);
        ignoredFallOffSprites = new List<Sprite>();
        EmptySprite = null;
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
                standartShape[vertexIndex] += -d;
            }
        }

        return standartShape;
    }

    public override void PlaceSprite(List<Vector2Int> directions, Vector2Int cord, 
        BlockOutline outline, TilePlacer placingInformation)
    {
        outlines = outline;
        this.directions = directions;
        cordinate = cord;
        var grassOutlines = new Dictionary<Vector2Int, SingleOutline>();
        foreach (var side in directions)
        {
            var grassSprite = GetGrassGenerator(side).GenerateGrass(cord + side, side, out var o);
            placingInformation.PlaceGrass(grassSprite, side, side + cord);
            grassOutlines[side] = new SingleOutline(o);
        }
        grassOutline = new BlockOutline(grassOutlines);
        var sprite = GetSprite();
        placingInformation.PlaceGround(sprite, cord);
        var groundOutlineSprite = GetOutlneSprite(outline);
        if (groundOutlineSprite != null)
            placingInformation.PlaceGroundOutline(groundOutlineSprite, cord);
        var grassOutlineSprite = GetOutlneSprite(grassOutline);
        if (grassOutlineSprite != null)
            placingInformation.PlaceGrassOutline(grassOutlineSprite, cord);
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
        var savedCord = new Vector2Int(cordinate.x % voronoiDiameter, cordinate.y % voronoiDiameter);
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
        var tex = CreateTexture();
        var rect = new Rect(0, 0, tex.width, tex.height);
        var pivot = Vector2.one / 2;
        var sprite = Sprite.Create(tex, rect, pivot, 16);
        var shape = new List<Vector2[]>();
        shape.Add(GetPhysicsShape(rect));
        sprite.OverridePhysicsShape(shape);
        return sprite;
    }

    private GrassGenerator GetGrassGenerator(Vector2Int side)
    {
        if (side == Vector2Int.up)
            return topGrass;
        if (side == Vector2Int.right)
            return rightGrass;
        if (side == Vector2Int.left)
            return leftGrass;
        return bottomGrass;
    }

    private Texture2D CreateTexture()
    {
        
        var tex = new Texture2D(16, 16);
        for (var x = 0; x < 16; x++)
        {
            for (var y = 0; y < 16; y++)
            {
                var pixel = new Vector2Int(x, y);
                var color = CalculateColor(pixel);
                tex.SetPixel(x, y, color);
            }
        }
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        return tex;
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

    private float GetValue(Vector2Int pixel)
    {
        var uv = (Vector2)pixel / 15f;
        return (1 - voronoi.GetValue(cordinate + uv)) * summWeight;
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

    private Color CalculateColor(Vector2Int pixel)
    {
        var current = 0f;
        var rawValue = GetValue(pixel);
        var fallOff = GetFallOff(pixel);
        var value = rawValue * Mathf.Pow(fallOff, (isIgnoreFallOff) ? IgnoredFallOffWeight : FallOffWeight);
        foreach (var c in colorTypes)
        {
            if (current <= value && current + c.weight > value)
            {   
                if (c.useTranslation && GetNeighbours(pixel).Any(n =>
                {
                    var rv = GetValue(n);
                    var f = GetFallOff(n);
                    var v = rv * Mathf.Pow(f, (isIgnoreFallOff) ? IgnoredFallOffWeight : FallOffWeight);
                    return (current > v || current + c.weight <= v);
                }))
                    return c.translationColor;
                return c.color;
            }
            current += c.weight;
        }
        return colorTypes[colorTypes.Count - 1].color;
    }
}

[System.Serializable]
public class grassColorVariant
{
    public Color color;
    [Range(0, 1)]
    public float weight;
    public bool useTranslation;
    public Color translationColor;
}
