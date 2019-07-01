using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class OctreeNode
{
    public OctreeTransform transform;
    public byte k;
    public OctreeNode[] children;
    public OctreeNode father;
    public Morton morton;

    public bool isLeaf = true, isBorder = false, isInner = false, isSelected = false;

    public OctreeNode(OctreeTransform transform)
    {
        father = null;
        this.transform = transform;
        k = 0;
        morton = new Morton();
    }

    public OctreeNode(OctreeTransform transform, int k, OctreeNode father, ulong index)
    {
        this.father = father;
        this.transform = transform;
        this.k = (byte)k;
        morton = new Morton(index);
    }

    public void Increase(int targetK)
    {
        if (k == targetK)
        {
            GenerateChildren();
        }
        else
        {
            if (!isLeaf)
            {
                for (int i = 0; i < 8; i++)
                {
                    children[i].Increase(targetK);
                }
            }
        }
    }

    internal void Bake(List<Triangle> triangles)
    {
        for (int i = 0; i < triangles.Count; i++)
        {
            bool intersects = Intersector.IntersectionTest(
                                                           new Intersector(transform),
                                                           new Intersector(triangles[i], -transform.worldPos, i)
                                                           );
            if (intersects)
            {
                isBorder = true;
                if (!isLeaf)
                {
                    List<Triangle> newTris = triangles.GetRange(i, triangles.Count - i);
                    for (int j = 0; j < children.Length; j++)
                    {
                        children[j].Bake(newTris);
                    }
                }
                return;
            }
            
        }
    }

    public void IncreaseUntil(int maxDepth)
    {
        if (k<=maxDepth)
        {
            if (children == null)
            {
                GenerateChildren();
            }
            for (int i = 0; i < children.Length; i++)
            {
                children[i].IncreaseUntil(maxDepth);
            }
        }
    }

    public void Refresh()
    {
        if (isLeaf) return;
        RefreshChildren();
        for (int i = 0; i < children.Length; i++)
        {
            children[i].Refresh();
        }
    }

    public bool Flood(bool isBorder, bool isInner)
    {
        if (this.isBorder||this.isInner) return false;
        this.isBorder = isBorder;
        this.isInner = isInner;
        UpdateUpwards(isBorder, isInner);
        return true;
    }

    public bool Erase(bool isBorder, bool isInner)
    {
        if (!this.isBorder && !this.isInner) return false;
        if (isBorder)
        {
            if(this.isInner) this.isBorder = true;
            return false;
        }
        else this.isBorder = false;
        this.isInner = false;
        return true;
    }

    public void RefreshUpwards()
    {
        if (father != null)
        {
            father.UpdateUpwards(isBorder, isInner);
        }
    }

    void UpdateUpwards(bool isBorder, bool isInner)
    {
        if (father == null) return;
        if (isBorder) this.isBorder = true;
        if (isInner) this.isInner = true;
        father.UpdateUpwards(isBorder, isInner);
    }

    public void Display(int showedSubdivision)
    {
        if (Octree.IsBaked)
        {
            
            if (isSelected)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(transform.worldPos, transform.worldScale);
            }
            else if (isBorder && k == showedSubdivision)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireCube(transform.worldPos, transform.worldScale);
            }
            else if(isInner && k == showedSubdivision)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(transform.worldPos, transform.worldScale);
            }
            else if(!isLeaf)
            {
                for (int i = 0; i < children.Length; i++)
                {
                    children[i].Display(showedSubdivision);
                }
            }
        }
        else
        {
            if (isSelected)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(transform.worldPos, transform.worldScale);
            }
            else if (isLeaf || k == showedSubdivision)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(transform.worldPos, transform.worldScale);
            }
            else
            {
                for (int i = 0; i < children.Length; i++)
                {
                    children[i].Display(showedSubdivision);
                }
            }
        }
    }

    private void GenerateChildren()
    {
        isLeaf = false;
        children = new OctreeNode[8];
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                for (int z = 0; z < 2; z++)
                {
                    Vector3 newLocalPosition = new Vector3(
                                                        (2 * x - 1) / 4f,
                                                        (2 * y - 1) / 4f,
                                                        (2 * z - 1) / 4f
                                                        );
                    Vector3 newWorldPosition = Vector3.Scale(transform.worldScale, newLocalPosition);
                    OctreeTransform newTransform = transform;
                    newTransform.worldPos += newWorldPosition;
                    newTransform.worldScale = transform.worldScale / 2;
                    newTransform.localPos = newLocalPosition;
                    


                    children[Octree.GetIndex(newLocalPosition)] = new OctreeNode(
                                                                        newTransform, 
                                                                        k + 1, 
                                                                        this, 
                                                                        Morton.Insert(morton.mortonCode, (ulong)Octree.GetIndex(newLocalPosition), k)
                                                                    );
                }
            }
        }
    }

    private void RefreshChildren()
    {
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                for (int z = 0; z < 2; z++)
                {
                    Vector3 newLocalPosition = new Vector3(
                                                        (2 * x - 1) / 4f,
                                                        (2 * y - 1) / 4f,
                                                        (2 * z - 1) / 4f
                                                        );
                    Vector3 newWorldPosition = Vector3.Scale(transform.worldScale, newLocalPosition);
                    OctreeTransform newTransform = transform;
                    newTransform.worldPos += newWorldPosition;
                    newTransform.worldScale = transform.worldScale / 2;
                    newTransform.localPos = newLocalPosition;

                    children[Octree.GetIndex(newLocalPosition)].transform = newTransform;
                }
            }
        }
    }

    public OctreeNode GetNode(Morton morton, int maxK)
    {
        if (isLeaf || k == maxK) return this;
        else
        {
            return children[morton.GetChildIndex(k+1)].GetNode(morton, maxK);
        }
    }
}
