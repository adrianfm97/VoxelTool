using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AABB
{
    public Vector3 center;
    public Vector3 halfSide;
    public Vector3 fromCenter;

    public AABB(Vector3 min, Vector3 max)
    {
        halfSide = (max - min) / 2f;
        center = min + halfSide;
        fromCenter = -center;
    }

    public static AABB FromVertices(Vector3[] vertices)
    {
        return new AABB(minCorner(vertices), maxCorner(vertices));
    }
    public static AABB FromTriangle(Triangle t)
    {
        return new AABB(minCorner(t.vertices), maxCorner(t.vertices));
    }

    public static AABB FromSide(Vector3 position, Vector3 halfSide)
    {
        return new AABB(position - halfSide, position + halfSide);
    }

    public void Draw(Color c)
    {
        Gizmos.color = c;
        Gizmos.DrawWireCube(center, 2*halfSide);
    }

    public void SetAABB(Vector3 min, Vector3 max)
    {
        halfSide = (max - min) / 2f;
  
        center = min + halfSide;
        fromCenter = -center;
    }

    public bool BoundBoundIntersection(AABB aabb)
    {
        Vector3 dist = aabb.center - this.center;
        return !(Mathf.Abs(dist.x) >= aabb.halfSide.x + halfSide.x || 
                Mathf.Abs(dist.y) >= aabb.halfSide.y + halfSide.y || 
                Mathf.Abs(dist.z) >= aabb.halfSide.z + halfSide.z);
    }

    public Vector3[] GetBoundVertices()
    {
        Vector3[] boundVertices = new Vector3[8];
        for (int y = 0; y < 2; y++)
        {
            for (int z = 0; z < 2; z++)
            {
                for (int x = 0; x < 2; x++)
                {
                    Vector3 localVector = new Vector3(x*2-1, y*2-1, z*2-1);
                    boundVertices[y * 4 + z * 2 + x] = new Vector3( center.x + localVector.x * halfSide.x, 
                                                                    center.y + localVector.y * halfSide.y, 
                                                                    center.z + localVector.z * halfSide.z
                                                                    );
                    //Debug.Log(boundVertices[y * 4 + z * 2 + x]);
                }
            }
        }
        return boundVertices;
    }

    public Vector3[] GetAxis(Vector3[] edges)
    {
        Vector3[] axis = new Vector3[9];
        for (int i = 0; i < 3; i++)
        {
            axis[i*3] = Vector3.Cross(new Vector3(halfSide.x*2, 0, 0), edges[i]);
            axis[i * 3 + 1] = Vector3.Cross(new Vector3(0, halfSide.y * 2, 0), edges[i]);
            axis[i * 3 + 2] = Vector3.Cross(new Vector3(0, 0, halfSide.z * 2), edges[i]);
        }
        return axis;
    }

    public static Vector3 minCorner(Vector3[] vertices)
    {
        Vector3 v = new Vector3();

        for (int i = 0; i < vertices.Length; i++)
        {
            if (i == 0)
            {
                v = vertices[0];
            }
            else
            {
                v = Vector3.Min(v, vertices[i]);
            }
        }
        return v;
    }

    public static Vector3 maxCorner(Vector3[] vertices)
    {
        Vector3 v = new Vector3();

        for (int i = 0; i < vertices.Length; i++)
        {
            if (i == 0)
            {
                v = vertices[0];
            }
            else
            {
                v = Vector3.Max(v, vertices[i]);
            }
        }
        return v;
    }

    public bool isOneDimension()
    {
        int c = 0;
        if(this.halfSide.x == 0)
        {
            c++;
        }
        if(this.halfSide.y == 0)
        {
            c++;
        }
        if(this.halfSide.z == 0)
        {
            c++;
        }

        if (c >= 2) return true;
        else return false;
    }

    public AABB Centered()
    {
        AABB centered = new AABB();
        centered.center = Vector3.zero;
        centered.fromCenter = fromCenter;
        return centered;
    }
}

