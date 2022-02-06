using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class GroundGenerator : ScriptableObject
{
    protected OutlinesData outlinesData;
    public virtual void Initiate(OutlinesData outlines, 
        Dictionary<string, Texture2D> secondaryTextures)
    {
        outlinesData = outlines;
    }

    public abstract GroundBlock GetRandomBlock(Vector2Int blockCordinate);
    public abstract void PlaceBlock(Vector2Int cordinate, List<Vector2Int> directions, TilePlacer mapInformation);

    public void GeneratePlants(Vector2Int baseCordinate, List<Vector2Int> directions)
    {
        var block = GetRandomBlock(baseCordinate);
        block.GeneratePlants(baseCordinate, directions);
    }
}
