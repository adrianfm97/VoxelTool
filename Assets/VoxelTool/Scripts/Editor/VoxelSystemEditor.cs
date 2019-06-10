using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(VoxelSystem))]
public class VoxelSystemEditor : Editor
{
    string DrawButtonsTitle = "Draw Bounds";
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        
        VoxelSystem myScript = (VoxelSystem)target;
        if (GUILayout.Button("Create"))
        {
            myScript.Create();
        }
        if (GUILayout.Button("Bake"))
        {
            myScript.Bake();
        }
        if (GUILayout.Button("Create&Bake&Mesh"))
        {
            myScript.Create();
            myScript.Bake();
            myScript.GenerateMesh();
        }
        if (GUILayout.Button(DrawButtonsTitle))
        {
            if(DrawButtonsTitle.Equals("Draw Bounds"))
            {
                DrawButtonsTitle = "Erase Bounds";
            }
            else
            {
                DrawButtonsTitle = "Draw Bounds";
            }
            //Debug.Log(DrawButtonsTitle);
            myScript.DebugDraw();
        }
        if (GUILayout.Button("FloodFill"))
        {
            myScript.FloodFill();
        }
        if (GUILayout.Button("Generate Mesh"))
        {
            myScript.GenerateMesh();
        }
        if (GUILayout.Button("CheckCube"))
        {
            myScript.CheckCube();
            EditorApplication.QueuePlayerLoopUpdate();
        }
    }
}
