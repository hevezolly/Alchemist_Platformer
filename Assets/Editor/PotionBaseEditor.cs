using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PotionBase))]
public class PotionBaseEditor : Editor
{
    PotionBase potBase;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (potBase.modificators != null)
        {
            var style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            foreach (var modifyer in potBase.modificators)
            {
                GUILayout.Space(20);
                GUILayout.Label(modifyer.GetType().Name, style);
                var editor = CreateEditor(modifyer);
                editor.OnInspectorGUI();

            }
        }
    }

    private void OnEnable()
    {
        potBase = (PotionBase)target;
    }
}

