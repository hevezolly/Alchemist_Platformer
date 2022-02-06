using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PotionEffect), true)]
public class PotionEffectEditor : Editor
{
    PotionEffect potEffect;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (potEffect.BaseProperty != null)
        {
            var style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 15;
            GUILayout.Space(30);
            GUILayout.Label("Mutable Properties:", style);
            GUILayout.Space(10);
            var editor = CreateEditor(potEffect.BaseProperty);
            editor.OnInspectorGUI();
        }
    }

    private void OnEnable()
    {
        potEffect = (PotionEffect)target;
    }
}


