using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Biome))]
public class BiomeEditor : Editor
{
    Biome biome;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (biome.groundTiles != null)
        { 
            var jumpEditor = CreateEditor(biome.groundTiles);
            jumpEditor.OnInspectorGUI();
        }
    }

    private void OnEnable()
    {
        biome = (Biome)target;
    }
}
