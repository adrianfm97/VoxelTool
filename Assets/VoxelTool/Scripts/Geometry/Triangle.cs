using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Triangle
{
    public Vector3[] vertices;
    public Vector3 normal;
    public Vector3 Center
    {
        get
        {
            return ((vertices[0] + vertices[1] + vertices[2]) / 3);
        }
    }

    public Triangle(Vector3[] vertices)
    {
        this.vertices = vertices;

        normal = Vector3.Cross(vertices[1]-vertices[0], vertices[2]-vertices[0]);
    }

    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        this.vertices = new Vector3[3]{ a,b,c};

        normal = Vector3.Cross(b - a, c - a);
    }

    public void Draw()
    {
        Draw(Color.red);
    }

    public void Draw(Color color)
    {
        Gizmos.color = color;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (i == 0)
            {
                continue;
            }
            Gizmos.DrawLine(vertices[i - 1], vertices[i]);
        }
        Gizmos.DrawLine(vertices[vertices.Length - 1], vertices[0]);
    }

    public void DebugDraw(Color color)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            if (i == 0)
            {
                continue;
            }
            Debug.DrawLine(vertices[i - 1], vertices[i], color, 10);
        }
        Debug.DrawLine(vertices[vertices.Length - 1], vertices[0], color, 10);
    }

    public Plane GetPlane()
    {
        return new Plane(vertices[0], vertices[1], vertices[2]);
    }

    public Vector3[] GetEdges()
    {
        Vector3[] edges = {vertices[1] - vertices[0],
                            vertices[2] - vertices[1],
                            vertices[0] - vertices[2] };
        return edges;
    }

    public bool PlaneAABBIntersection(AABB aabb)
    {
        Vector3[] boundVertices = aabb.GetBoundVertices();
        bool previousSign = true;
        Plane p = GetPlane();
        for (int i = 0; i < boundVertices.Length; i++)
        {
            if (i == 0)
            {
                previousSign = p.GetSide(boundVertices[i]);
                continue;
            }
            if (p.GetSide(boundVertices[i]) != previousSign)
            {
                return true;
            }
        }
        return false;
    }



    public Triangle Sum(Vector3 sum)
    {
        Vector3[] vert = vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vert[i] += sum;
        }
        return new Triangle(vert);
    }
}
