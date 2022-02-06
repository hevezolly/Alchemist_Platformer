using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColor
{
    Biome mainBiome;
    Dictionary<BiomeShadr, Vector2> shards;

    public BackgroundColor(Biome main)
    {
        mainBiome = main;
        shards = new Dictionary<BiomeShadr, Vector2>();
    }

    public void AssignPosition(BiomeShadr shard, Vector2 pos)
    {
        shards[shard] = pos;
    }

    public Color GetBackgrpundColor(Vector2 position)
    {
        foreach (var shard in shards.Keys)
        {
            var distance = Vector2.Distance(position, shards[shard]);
            if (distance > shard.radius)
                continue;
            if (distance < shard.radius * shard.unTouchedDistance)
                return shard.biome.color;
            var t = Mathf.InverseLerp(shard.radius * shard.unTouchedDistance, shard.radius, distance);
            return Color.Lerp(shard.biome.color, mainBiome.color, t);
        }
        return mainBiome.color;
    }
}
