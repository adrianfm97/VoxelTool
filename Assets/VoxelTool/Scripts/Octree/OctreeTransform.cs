using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OctreeTransform
{
    public Vector3 worldPos, worldScale;
    public Quaternion worldRot;
    public Vector3 localPos, localScale;
    public Quaternion localRot;
    public OctreeTransform(Transform transform)
    {
        worldPos = transform.position;
        worldRot = transform.rotation;
        worldScale = transform.localScale;

        localPos = transform.localPosition;
        localRot = transform.localRotation;
        localScale = transform.localScale;

    }
}
