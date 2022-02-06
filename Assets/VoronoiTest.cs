using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiTest : MonoBehaviour
{
    public int diameter;
    public float scale;
    public SpriteRenderer renderer;

    private Voronoi voronoi;

    private void Generate()
    {
        voronoi = new Voronoi(diameter - 1, diameter - 1, scale);
        renderer.sprite = CreateSprite();
    }

    private Sprite CreateSprite()
    {
        var r = new Rect(0, 0, 16 * diameter, 16 * diameter);
        var pivot = Vector2.one / 2f;
        var texture = CreateTexture();
        var sprite = Sprite.Create(texture, r, pivot, 16);
        return sprite;
    }

    private Texture2D CreateTexture()
    {
        var tex = new Texture2D(16 * diameter, 16 * diameter);
        for (var cordX = 0; cordX < diameter; cordX++)
        {
            for (var cordY = 0; cordY < diameter; cordY++)
            {
                for (var x = 0; x < 16; x++)
                {
                    for (var y = 0; y < 16; y++)
                    {
                        var uv = new Vector2Int(cordX, cordY) +  new Vector2(x, y) / 16f;
                        var value = voronoi.GetValue(uv);
                        tex.SetPixel(cordX * 16 + x, cordY * 16 + y, Color.Lerp(Color.black, Color.white, value));
                    }
                }
            }
        }
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        return tex;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    private void OnValidate()
    {
        Generate();
    }
}
