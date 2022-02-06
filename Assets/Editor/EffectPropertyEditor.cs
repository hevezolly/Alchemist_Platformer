using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PotionEffectPropery))]
public class EffectPropertyEditor : Editor
{
    PotionEffectPropery property;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var aspects = new List<PropertyAspect>(property.GetAspects());
        if (aspects.Count > 0)
        {
            var style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            foreach (var aspect in aspects)
            {
                GUILayout.Space(20);
                GUILayout.Label(aspect.GetType().Name, style);
                var editor = CreateEditor(aspect);
                editor.OnInspectorGUI();
            }
        }
    }

    private void OnEnable()
    {
        property = (PotionEffectPropery)target;
    }
}
