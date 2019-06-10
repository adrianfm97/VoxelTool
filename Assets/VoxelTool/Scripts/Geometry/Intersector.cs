using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersector
{
    public OctreeTransform transform;
    public Triangle triangle;
    public AABB aabb;
    public bool isBound;

    public int triNumber;

    public string intersectionInfo = "No test has been done.";

    public Intersector(Triangle triangle, Vector3 fromCenter)
    {
        isBound = false;
        Vector3[] vertices = new Vector3[3];
        
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = triangle.vertices[i] + fromCenter;
        }
        this.triangle = new Triangle(vertices);
        //this.triangle = triangle;
        aabb = AABB.FromTriangle(this.triangle);
    }

    public Intersector(Triangle triangle, Vector3 fromCenter, int triNumber)
    {
        isBound = false;
        Vector3[] vertices = new Vector3[3];

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = triangle.vertices[i] + fromCenter;
        }
        this.triangle = new Triangle(vertices);
        aabb = AABB.FromTriangle(this.triangle);
        this.triNumber = triNumber;
    }

    public Intersector(OctreeTransform transform)
    {
        isBound = true;
        this.transform = transform;
        aabb = AABB.FromSide(transform.worldPos, transform.worldScale/2);
    }

    public static bool IntersectionTest(Intersector a, Intersector b)
    {
        Intersector meshTriangle = a.isBound ? b : a;
        AABB bound        = a.isBound ? a.aabb : b.aabb;

        bound.center = Vector3.zero;
        if (bound.BoundBoundIntersection(meshTriangle.aabb))
        {
            if (meshTriangle.triangle.PlaneAABBIntersection(bound))
            {
                if (FinalIntersection(bound, meshTriangle.triangle))
                {
                    a.intersectionInfo = "OK";
                    b.intersectionInfo = "OK";
                    return true;
                }
                else
                {
                    a.intersectionInfo = "FinalIntersection test reject";
                    b.intersectionInfo = "FinalIntersection test reject";
                    return false;
                }
            }
            else
            {
                a.intersectionInfo = "PlaneAABBIntersection test reject";
                b.intersectionInfo = "PlaneAABBIntersection test reject";
                return false;
            }
        }
        else
        {
            a.intersectionInfo = "BoundBoundIntersection test reject";
            b.intersectionInfo = "BoundBoundIntersection test reject";
            return false;
        }
    }

    private static bool FinalIntersection(AABB bound, Triangle t)
    {
        Vector3[] axis = bound.GetAxis(t.GetEdges());
        float[] projectedPoints;

        Vector3[] vertices = t.vertices;

        int[] nonZeroIndices = new int[2];
        float r;

        for (int i = 0; i < axis.Length; i++)
        {
            projectedPoints = new float[3];
            for (int j = 0; j < 3; j++)
            {
                projectedPoints[j] = Vector3.Dot(axis[i], vertices[j]);
            }
            r = bound.halfSide.x * Mathf.Abs(axis[i].x) +
                bound.halfSide.y * Mathf.Abs(axis[i].y) +
                bound.halfSide.z * Mathf.Abs(axis[i].z);

            if (Mathf.Min(projectedPoints) > r || Mathf.Max(projectedPoints) < -r)
                return false;
        }
        return true;
    }
}
