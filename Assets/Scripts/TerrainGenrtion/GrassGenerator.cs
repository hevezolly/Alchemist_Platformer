using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GrassGenerator : ScriptableObject
{
    protected Dictionary<string, Texture2D> secondaryTextures;
    public abstract Sprite GenerateGrass(Vector2Int cord, Vector2Int side, out SingleOutline outline);

    public virtual void Initiate(Dictionary<string, Texture2D> secondaryTextures)
    {
        this.secondaryTextures = secondaryTextures;
    }   

    protected float GetFallOff(Vector2Int side, Vector2 uv)
    {
        if (side.x > 0 || side.y > 0)
            uv = Vector2.one - uv;
        if (side.x == 0)
            return Mathf.Lerp(0.01f, 1, uv.y);
        return Mathf.Lerp(0.01f, 1, uv.x);
    }
}
