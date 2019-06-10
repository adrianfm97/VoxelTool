using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GetSide_Plane_Test : MonoBehaviour
{

    public Vector3 point;
    Plane plane;
    // Update is called once per frame

    private void Start()
    {
        plane = new Plane(transform.up, transform.position);
        print(plane.GetSide(point));
    }
    void Update()
    {
        
        
    }
}
