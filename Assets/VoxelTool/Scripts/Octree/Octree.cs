using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OctreeIndex
{
    BottomLeftBack = 0b000, //0
    BottomLeftFront = 0b001, //1
    BottomRightBack = 0b010, //2
    BottomRightFront = 0b011, //3
    TopLeftBack = 0b100, //4
    TopLeftFront = 0b101, //5
    TopRightBack = 0b110, //6
    TopRightFront = 0b111 //7
}

public class Octree
{

    public OctreeTransform rootTransform;
    public OctreeNode root;
    public int depth;

    public static bool IsBaked = false;

    public Octree()
    {
        //this.transform = transform;
        
    }

    public void Create(int depth, Transform transform)
    {
        this.depth = depth;
        rootTransform = new OctreeTransform(transform);
        root = new OctreeNode(rootTransform);
        IncreaseUntil(depth);
        
    }
    public void Destroy()
    {

    }

    public void IncreaseUntil(int maxDepth)
    {
        root.IncreaseUntil(maxDepth-1);
    }

    internal void Bake(List<Triangle> triangles)
    {
        IsBaked = true;
        root.Bake(triangles);
    }

    public void Display(int showedSubdivision)
    {

        root.Display(showedSubdivision);
    }

    public void Refresh(Transform transform)
    {
        root.transform = new OctreeTransform(transform);
        root.Refresh();
    }

    public void Flood(Vector3 pos)
    {

        Stack<Vector3> stack = new Stack<Vector3>();
        stack.Push(pos);
        while (stack.Count > 0)
        {
            Vector3 currentPos = stack.Pop();
            if (GetNode(currentPos, depth).Flood())
            {
                Vector3 posTransformer = rootTransform.worldScale / Mathf.Pow(2, depth);
                stack.Push(new Vector3(currentPos.x + posTransformer.x, currentPos.y, currentPos.z));
                stack.Push(new Vector3(currentPos.x - posTransformer.x, currentPos.y, currentPos.z));
                stack.Push(new Vector3(currentPos.x, currentPos.y + posTransformer.y, currentPos.z));
                stack.Push(new Vector3(currentPos.x, currentPos.y - posTransformer.y, currentPos.z));
                stack.Push(new Vector3(currentPos.x, currentPos.y, currentPos.z + posTransformer.z));
                stack.Push(new Vector3(currentPos.x, currentPos.y, currentPos.z - posTransformer.z));
            }
        }
    }

    public OctreeNode[,,] GetKNodes(int k)
    {
        int maxNodesInAxis = (int)Mathf.Pow(2, k);
        OctreeNode[,,] nodes = new OctreeNode[maxNodesInAxis, maxNodesInAxis, maxNodesInAxis];
        for (int x = 0; x < maxNodesInAxis; x++)
        {
            for (int y = 0; y < maxNodesInAxis; y++)
            {
                for (int z = 0; z < maxNodesInAxis; z++)
                {
                    nodes[x, y, z] = GetNode(new Vector3(
                                                         Mathf.Lerp(-rootTransform.worldScale.x/2, rootTransform.worldScale.x / 2, 
                                                            Mathf.InverseLerp(0, maxNodesInAxis-1, x)),
                                                         Mathf.Lerp(-rootTransform.worldScale.y / 2, rootTransform.worldScale.y / 2,
                                                            Mathf.InverseLerp(0, maxNodesInAxis-1, y)),
                                                         Mathf.Lerp(-rootTransform.worldScale.z / 2, rootTransform.worldScale.z / 2,
                                                            Mathf.InverseLerp(0, maxNodesInAxis-1, z))
                                                        ),
                                                k
                                            );
                }
            }
        }
        return nodes;
    }

    public static int GetIndex(Vector3 position)
    {
        int index = 0;

        index |= position.z > Vector3.zero.z ? 1 : 0;
        index |= position.x > Vector3.zero.x ? 2 : 0;
        index |= position.y > Vector3.zero.y ? 4 : 0;

        return index;
    }

    public OctreeNode GetNode(Vector3 worldPos, int maxK) //World pos
    {
        return GetNode(Morton.WorldToMortonIntPos(worldPos, root.transform), maxK);
    }

    public OctreeNode GetNode(Vector3Int pos, int maxK) //Morton position
    {
        return GetNode(Morton.MortonPosToCode(pos), maxK);
    }

    public OctreeNode GetNode(Morton morton, int maxK)
    {
        return root.GetNode(morton, maxK);
    }
}
