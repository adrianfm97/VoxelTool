using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInside : MonoBehaviour
{
    public MeshFilter meshFilter;
    public bool isDrawing = false;

    Mesh mesh;

    List<Vector3> insidePositions = new List<Vector3>();
    List<Vector3> outsidePositions = new List<Vector3>();
    Vector3 extents;
    Bounds bound;

    void Start()
    {
        mesh = meshFilter.sharedMesh;
        bound = meshFilter.gameObject.GetComponent<BoxCollider>().bounds;
    }

    void Update()
    {
        transform.position =    new Vector3(
                                    Random.Range(-bound.extents.x + bound.center.x, bound.extents.x + bound.center.x),
                                    Random.Range(-bound.extents.y + bound.center.y, bound.extents.y + bound.center.y),
                                    Random.Range(-bound.extents.z + bound.center.z, bound.extents.z + bound.center.z)
                                );
        CheckInsideEditor();
    }

    private void OnDrawGizmos()
    {
        if (!isDrawing) return;
        foreach(Vector3 pos in insidePositions)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(pos, 0.02f);
        }
        return;
        foreach (Vector3 pos in outsidePositions)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pos, 0.02f);
        }
    }

    public void CheckInsideEditor()
    {
        mesh = meshFilter.sharedMesh;
        Vector3[] v = new Vector3[3];
        Triangle[] triangles = new Triangle[mesh.triangles.Length / 3];
        float minDistance = float.PositiveInfinity;
        Vector3 minPoint = new Vector3();
        Vector3 minPointNormal = new Vector3();
        Triangle minTriangle = new Triangle();
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            float closerDistance = 0;
            Vector3 closerPoint = new Vector3();
            for (int j = 0; j < 3; j++)
            {
                v[j] = mesh.vertices[mesh.triangles[i + j]];
                if (j == 0)
                {
                    closerDistance = (v[j] - transform.position).sqrMagnitude;
                    closerPoint = v[j];
                }
                else
                {
                    float auxCloserDistance = (v[j] - transform.position).sqrMagnitude;
                    if (closerDistance > auxCloserDistance)
                    {
                        closerDistance = auxCloserDistance;
                        closerPoint = v[j];
                    }
                }
            }
            Triangle tri = new Triangle(v);
            if (i == 0)
            {
                minTriangle = tri;
            }
            if(minDistance == closerDistance)
            {
                minDistance = closerDistance;
                Triangle aux = CheckNearerTriangle(tri, minTriangle);
                minPointNormal = aux.normal;
                minTriangle = aux;
                //minTriangle.DebugDraw(Color.red);
                minPoint = closerPoint;
            }
            else if (minDistance > closerDistance)
            {
                minDistance = closerDistance;
                minPointNormal = tri.normal;
                minTriangle = tri;
                minPoint = closerPoint;
            }
            if (i == mesh.triangles.Length - 3)
            {
                
            }
        }
        Debug.DrawRay(minPoint, minPointNormal*1000, Color.green);
        //minTriangle.DebugDraw(Color.red);
        if (Vector3.Dot(minPoint - transform.position, minPointNormal) >= 0)
        {
            Debug.Log("Inside");
            insidePositions.Add(transform.position);
        }
        else
        {
            Debug.Log("Outside");
            outsidePositions.Add(transform.position);
        }
    }

    public void CheckInside()
    {
        Vector3[] v = new Vector3[3];
        Triangle[] triangles = new Triangle[mesh.triangles.Length/3];
        float minDistance = float.PositiveInfinity;
        Vector3 minPoint = new Vector3();
        Vector3 minPointNormal = new Vector3();
        for (int i = 0; i < mesh.triangles.Length; i+=3)
        {
            float closerDistance = 0;
            Vector3 closerPoint = new Vector3();
            for (int j = 0; j < 3; j++)
            {
                v[j] = mesh.vertices[mesh.triangles[i+j]];
                if (j == 0)
                {
                    closerDistance = (v[j] - transform.position).sqrMagnitude;
                    closerPoint = v[j];
                }
                else
                {
                    float auxCloserDistance = (v[j] - transform.position).sqrMagnitude;
                    if (closerDistance > auxCloserDistance)
                    {
                        closerDistance = auxCloserDistance;
                        closerPoint = v[j];
                    }
                }
            }
            Triangle tri = new Triangle(v);

            if (minDistance > closerDistance)
            {
                minDistance = closerDistance;
                minPointNormal = tri.normal;
                minPoint = closerPoint;
            }
        }
        //Debug.DrawRay(minPoint, minPointNormal*1000, Color.green);
        if (Vector3.Dot(minPoint - transform.position, minPointNormal) >= 0)
        {
            Debug.Log("Inside");
            insidePositions.Add(transform.position);
        }
        else
        {
            Debug.Log("Outside");
            outsidePositions.Add(transform.position);
        }
    }

    private Triangle CheckNearerTriangle(Triangle t1, Triangle t2)
    {
        if((t1.Center - transform.position).sqrMagnitude < (t2.Center - transform.position).sqrMagnitude)
        {
            return t1;
        }
        else
        {
            return t2;
        }
        //return ((t1.Center - transform.position).sqrMagnitude < (t2.Center - transform.position).sqrMagnitude ? t1 : t2);
    }

    public void TriangleNumber()
    {
        print("Triangles: " + meshFilter.sharedMesh.triangles.Length / 3);
    }

}
