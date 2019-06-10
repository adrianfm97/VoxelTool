using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestInside))]
public class TestInsideEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TestInside script = (TestInside)target;
        if(GUILayout.Button("Is Inside?"))
        {
            script.CheckInsideEditor();
        }
        if(GUILayout.Button("Triangle Number"))
        {
            script.TriangleNumber();
        }
    }
}
