using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "standart ground generator", menuName = "game/biome content generator/standart ground")]
public class StandartGroundGenerator : GroundGenerator
{
    [Header("Standart Ground generator")]
    public float scale;
    public List<PlacingBlock> Blocks;
    private Vector2 offset;
    private Dictionary<Vector2Int, GroundBlock> generatedBlocks;
    private float summWeight { get
        {
            return Blocks.Sum(c => c.Weight);
        }
    }

    public override void PlaceBlock(Vector2Int cord, List<Vector2Int> directions, TilePlacer mapInformation)
    {
        var block = GetRandomBlock(cord);
        var generatedOutlines = new Dictionary<Vector2Int, SingleOutline>();
        if (!outlinesData.outlines.ContainsKey(cord))
        {
            foreach (var side in BlockOutline.GetSides())
            {
                if (directions.Contains(side))
                    continue;
                var sideCord = cord + side;
                var biome = outlinesData.GetBiome(sideCord);
                GroundBlock sideBlock = null;
                if (biome != null)
                    sideBlock = biome.groundTiles.GetRandomBlock(sideCord);
                if (sideBlock == null || sideBlock == block)
                    continue;
                var singleOutline = new SingleOutline();
                if (outlinesData.outlines.ContainsKey(sideCord))
                    singleOutline = new SingleOutline(outlinesData.outlines[sideCord][-side]);
                else
                {
                    var isHorizontal = side.x == 0;
                    var outlineStart = outlinesData.offset + (Vector2)cord * outlinesData.offset * 16 +
                        (Vector2)side * outlinesData.offset * 8 -
                        (isHorizontal ? Vector2.right : Vector2.up) * outlinesData.offset * 8;
                    singleOutline = new SingleOutline(
                        outlineStart,
                        isHorizontal,
                        outlinesData.step,
                        outlinesData.scale,
                        block.intersectionColor,
                        sideBlock.intersectionColor);
                }
                generatedOutlines.Add(side, singleOutline);
            }
            var blockOutline = new BlockOutline(generatedOutlines);
            outlinesData.outlines[cord] = blockOutline;
        }
        block.PlaceSprite(directions, cord, outlinesData.outlines[cord], mapInformation);
    }

    public override void Initiate(OutlinesData outlines, Dictionary<string, Texture2D> secondaryTextures)
    {
        base.Initiate(outlines, secondaryTextures);
        offset = Random.insideUnitCircle * 10;
        generatedBlocks = new Dictionary<Vector2Int, GroundBlock>();
        foreach (var b in Blocks)
        {
            b.block.Initiate(secondaryTextures);
        }
    }

    public override GroundBlock GetRandomBlock(Vector2Int blockCordinate)
    {
        if (generatedBlocks.ContainsKey(blockCordinate))
            return generatedBlocks[blockCordinate];
        var cord = (Vector2)blockCordinate * scale + offset;
        var value = Mathf.PerlinNoise(cord.x, cord.y) * summWeight;
        var currentPoint = 0f;
        foreach (var c in Blocks)
        {
            if (value >= currentPoint && value < currentPoint + c.Weight)
            {
                generatedBlocks[blockCordinate] = c.block;
                return c.block;
            }
            currentPoint += c.Weight;
        }
        generatedBlocks[blockCordinate] = Blocks[Blocks.Count - 1].block;
        return Blocks[Blocks.Count - 1].block;
    }
}
[System.Serializable]
public class PlacingBlock
{
    [Range(0, 1)]
    public float Weight;
    public GroundBlock block;
}
