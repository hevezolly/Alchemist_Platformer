using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorController
{
    public Color[] colors;

    public static Texture2D CombineColors(IEnumerable<Color> c)
    {
        var colors = new List<Color>(c);
        var t = new Texture2D(colors.Count, 1);
        for (var i = 0; i < colors.Count; i++)
        {
            t.SetPixel(i, 0, colors[i]);
        }
        t.filterMode = FilterMode.Point;
        t.Apply();
        return t;
    }
}
