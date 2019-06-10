using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Test))]
public class TestEditor : Editor
{
    
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        Test s = (Test)target;
        if (GUILayout.Button("Insert"))
            s.Insert();
        if (GUILayout.Button("Encode"))
            s.EncodePos();
        if (GUILayout.Button("Decode"))
            s.DecodeIndex();
        if (GUILayout.Button("GetChildIndex"))
            s.GetChildIndex();
    }
}
