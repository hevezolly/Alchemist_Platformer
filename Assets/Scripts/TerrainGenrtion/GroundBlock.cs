using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public abstract class GroundBlock : ScriptableObject
{

    public Color intersectionColor;

    public List<PlantVariant> plants;

    protected const float MinFallOff = 0.1f;

    public virtual void Initiate(Dictionary<string, Texture2D> secondaryTextures)
    {

    }

    public abstract void PlaceSprite(List<Vector2Int> directions, Vector2Int cord, BlockOutline outline, TilePlacer placingInformation);

    protected Sprite GetOutlneSprite(BlockOutline outline)
    {
        if (outline.isEmpty)
            return null;
        var texture = new Texture2D(16, 16);
        for (var x = 0; x < 16; x++)
        {
            for (var y = 0; y < 16; y++)
            {
                var cord = new Vector2Int(x, y);
                var color = GetIntersectionColor(cord, outline);
                if (color == null)
                    texture.SetPixel(x, y, Color.clear);
                else
                    texture.SetPixel(x, y, color.Value);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 16, 16), Vector2.one / 2, 16);
    }

    private Color? GetIntersectionColor(Vector2Int pixel, BlockOutline outlines)
    {
        foreach (var side in BlockOutline.GetSides())
        {
            var outline = outlines[side];
            int outlinePos;
            int direction;
            if (side.x == 0)
            {
                outlinePos = pixel.x;
                direction = side.y;
            }
            else
            {
                direction = side.x;
                outlinePos = pixel.y;
            }
            var originX = (direction < 0) ? -1 : 16;
            Vector2Int origin;
            if (side.x == 0)
                origin = new Vector2Int(pixel.x, originX);
            else
                origin = new Vector2Int(originX, pixel.y);
            var sideOffset = outline.GetOffset(outlinePos);
            if (sideOffset >= 0)
                continue;
            var resultPixel = origin + side * sideOffset;
            var toPixel = pixel - resultPixel;
            var product = Vector2.Dot(toPixel, side);
            if (product >= 0)
                return outline.otherColor;
        }
        return null;
    }

    protected float GetFallOff(Vector2 position, List<Vector2Int> directions)
    {
        var xFallOff = MinFallOff;
        var yFallOff = MinFallOff;
        if (position.x <= 0.5f && directions.Contains(Vector2Int.left))
        {
            xFallOff = Mathf.Lerp(1, MinFallOff, position.x * 2);
        }
        else if (position.x >= 0.5f && directions.Contains(Vector2Int.right))
        {
            xFallOff = Mathf.Lerp(MinFallOff, 1, (position.x - 0.5f) * 2);
        }

        if (position.y <= 0.5f && directions.Contains(Vector2Int.down))
        {
            yFallOff = Mathf.Lerp(1, MinFallOff, position.y * 2);
        }
        else if (position.y >= 0.5f && directions.Contains(Vector2Int.up))
        {
            yFallOff = Mathf.Lerp(MinFallOff, 1, (position.y - 0.5f) * 2);
        }

        return Mathf.Max(xFallOff, yFallOff);
    }

    public virtual Sprite GenerateGrass(Vector2Int grass)
    {
        return null;
    }

    public virtual void GeneratePlants(Vector2Int blockPosition, List<Vector2Int> sides)
    {
        foreach (var side in sides)
        {
            
            var value = UnityEngine.Random.value;
            foreach (var plant in plants
                .Where(p => p.plant.GetSides().Contains(side))
                .OrderBy(p => p.GetChance(side)).ThenBy(p => UnityEngine.Random.value))
            {
                if (value <= plant.GetChance(side))
                {
                    plant.plant.PlacePlant(side, blockPosition + Vector2.one / 2 + (Vector2)side / 2);
                    break;
                }
            }
        }
    }
}

public class CordinateInformation
{
    private int directionsValue;
    private Vector2Int cordValue;

    public CordinateInformation(Vector2Int cord, List<Vector2Int> dirs)
    {
        cordValue = cord;
        directionsValue = 0;
        foreach (var dir in dirs)
        {
            if (dir == Vector2Int.up)
                directionsValue += 1;
            else if (dir == Vector2Int.right)
                directionsValue += 2;
            else if (dir == Vector2Int.down)
                directionsValue += 4;
            else if (dir == Vector2Int.left)
                directionsValue += 8;
        }
    }

    public override bool Equals(object obj)
    {
        var other = obj as CordinateInformation;
        if (other == null)
            return false;
        return cordValue == other.cordValue && directionsValue == other.directionsValue;
    }

    public override int GetHashCode()
    {
        return 997 * directionsValue + cordValue.GetHashCode();
    }
}

[System.Serializable]
public class PlantVariant: EmptyVariant
{
    public Plant plant;
}

[System.Serializable]
public class EmptyVariant
{
    [Range(-0.001f, 1)]
    public float UpWeight;
    

    public float GetChance(Vector2Int side)
    {
        return UpWeight;
    }
}
