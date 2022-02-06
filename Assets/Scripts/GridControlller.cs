using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridControlller : MonoBehaviour
{
    public TilemapRenderer grassTop;
    public TilemapRenderer grassBottom;
    public TilemapRenderer grassLeft;
    public TilemapRenderer grassRight;
    private HashSet<SpriteRenderer> renderers = new HashSet<SpriteRenderer>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGrassTexture(string name, Texture2D texture)
    {
        foreach (var renderer in GetRenderers())
        {
            renderer.material.SetTexture(name, texture);
        }
    }

    public void SetGrasFloat(string name, float value)
    {
        foreach (var renderer in GetRenderers())
        {
            renderer.material.SetFloat(name, value);
        }
    }

    public void AddPlant(SpriteRenderer renderer)
    {
        renderers.Add(renderer);
    }

    public void SetGrassVector(string name, Vector4 value)
    {
        foreach (var renderer in GetRenderers())
        {
            renderer.material.SetVector(name, value);
        }
    }

    private IEnumerable<Renderer> GetRenderers()
    {
        yield return grassTop;
        yield return grassLeft;
        yield return grassBottom;
        yield return grassRight;
        var candidatesToDelete = new List<SpriteRenderer>();
        foreach (var renderer in renderers)
        {
            if (renderer == null)
            {
                candidatesToDelete.Add(renderer);
                continue;
            }
            yield return renderer;
        }
        foreach (var candidate in candidatesToDelete)
        {
            renderers.Remove(candidate);
        }
    }
}
