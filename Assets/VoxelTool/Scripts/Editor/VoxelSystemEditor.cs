using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(VoxelSystem))]
public class VoxelSystemEditor : Editor
{
    bool create = true;
    bool bake = true;
    bool generate = true;
    bool fill = true;

    private void Awake()
    {
        VoxelSystem myScript = (VoxelSystem)target;
        create = EditorPrefs.GetBool("create", create);
        bake = EditorPrefs.GetBool("bake", bake);
        generate = EditorPrefs.GetBool("generate", generate);
        fill = EditorPrefs.GetBool("fill", fill);

        if (Application.isPlaying && !myScript.voxelizationDone)
        {
            myScript.voxelizationDone = true;
            if (create)
                myScript.Create();
            if (bake)
                myScript.Bake();
            if (generate)
                myScript.GenerateMesh();
            if (fill)
                myScript.FloodFill();
        }
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        VoxelSystem myScript = (VoxelSystem)target;

        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.5f, 0.5f, 0.5f, 1));

        EditorGUILayout.Separator();

        create = EditorGUILayout.ToggleLeft("Create Structure", create);
        if(create) bake = EditorGUILayout.ToggleLeft("Bake Mesh", bake);
        if(create && bake) generate = EditorGUILayout.ToggleLeft("Generate Mesh", generate);
        if(create && bake && generate) fill = EditorGUILayout.ToggleLeft("Flood Fill", fill);

        EditorGUILayout.Space();

        if (create && GUILayout.Button("Voxelize"))
        {
            myScript.Create();
            if (bake)
                myScript.Bake();
            if (generate)
                myScript.GenerateMesh();
            if (fill)
                myScript.FloodFill();
        }
        EditorGUILayout.Separator();
        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.5f, 0.5f, 0.5f, 1));
        EditorGUILayout.Separator();

        Filler[] fillers = myScript.GetFillers();
        for (int i = 0; i < fillers.Length; i++)
        {
            if (GUILayout.Button("Activate filler " + (i+1) + " [\"" + fillers[i].name + "\"]"))
            {
                if (!fillers[i].isFullFill)
                {
                    if (fillers[i].erase)
                    {
                        myScript.FloodErase(fillers[i].transform.position, fillers[i].range);
                    }
                    else
                    {
                        myScript.FloodAtRange(fillers[i].transform.position, fillers[i].range);
                    }
                    
                }
                else
                {
                    myScript.FloodFill(fillers[i].transform.position);
                }
            }
        }
        EditorGUILayout.Separator();

        EditorPrefs.SetBool("create", create);
        EditorPrefs.SetBool("bake", bake);
        EditorPrefs.SetBool("generate", generate);
        EditorPrefs.SetBool("fill", fill);
    }
}
