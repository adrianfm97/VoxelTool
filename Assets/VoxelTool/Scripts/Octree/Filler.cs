using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Filler : MonoBehaviour
{
    [HideInInspector]
    public int index;
    public bool isFullFill;
    public int range;
    public bool erase;

    void Save()
    {
        EditorPrefs.SetBool("Filler" + index, isFullFill);
        EditorPrefs.SetInt("Filler" + index, range);
    }

    void Load(out bool outBoolean)
    {
        outBoolean =  EditorPrefs.GetBool("Filler" + index);
    }

    void Load(out int outInt)
    {
        outInt = EditorPrefs.GetInt("Filler" + index);
    }
}
