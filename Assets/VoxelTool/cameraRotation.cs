using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class cameraRotation : MonoBehaviour
{

    public float _Speed = 1;
    bool voxelMode = false;
    void OnEnable()
    {
        EditorApplication.update += CustomUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update = null;
    }

    void CustomUpdate()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, _Speed * Time.deltaTime);
    }

    void changeMesh()
    {
        voxelMode = !voxelMode;
        GameObject.Find("Mesh").GetComponent<MeshRenderer>().enabled = voxelMode;
        GameObject.Find("Robot").GetComponent<MeshRenderer>().enabled = !voxelMode;
    }
}
