using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelGenerator myScript = (LevelGenerator)target;
        if (GUILayout.Button("Preview"))
        {
            myScript.Preview();
        }
        if (GUILayout.Button("Generate"))
        {
            myScript.Generate();
        }
    }
}
