using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ModulatedPlayerMovement))]
public class ModulatedPlayerMovementEditor : Editor
{
    ModulatedPlayerMovement movement;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (movement.jump != null)
        {
            var jumpEditor = CreateEditor(movement.jump);
            jumpEditor.OnInspectorGUI();
        }
        foreach (var option in movement.moveOptions)
        {
            var editor = CreateEditor(option);
            editor.OnInspectorGUI();
        }
    }

    private void OnEnable()
    {
        movement = (ModulatedPlayerMovement)target;
    }
}
