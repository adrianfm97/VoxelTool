using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EditorCoroutines;

[ExecuteInEditMode]
public class VoxelSystem : MonoBehaviour
{
    [Range(0,8)]
    public int _Depth;
    public GameObject _Mesh;
    public bool _DebugDisplay = false;
    public Color _Color;

    private int showedSubdivision;
    private Octree octree;
    [HideInInspector]
    public static OctreeNode[,,] kNodes;
    [HideInInspector]
    public bool voxelizationDone;
    
    MeshFilter filter;
    MeshRenderer renderer;

    public void Update()
    {
        if(octree == null)
        {
            octree = new Octree();
            
        }
    }

    private void OnValidate()
    {
        showedSubdivision = _Depth;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        if (octree != null && octree.root != null) 
        {
            if(_DebugDisplay) octree.Display(showedSubdivision);
        }
    }

    public void Create()
    {
        octree = null;
        System.GC.Collect();
        showedSubdivision = _Depth;
        octree = new Octree();
        octree.Create(_Depth, transform);
        kNodes = null;
        transform.parent.GetComponent<MeshFilter>().sharedMesh = null;
    }

    public void Bake()
    {
        double bakingTimeCounter = EditorApplication.timeSinceStartup;
        octree.Bake(GetMeshTriangles(_Mesh));
        print("BakeTime: " + (EditorApplication.timeSinceStartup - bakingTimeCounter));
    }

    public void FloodFill()
    {
        Filler[] fillers = GetFillers();
        if (fillers.Length == 0)
        {
            Debug.LogWarning("You don't have any filler GameObject as children of Fillers.");
            return;
        }
        for (int i = 0; i < fillers.Length; i++)
        {
            if (fillers[i].isFullFill)
            {
                FloodFill(fillers[i].transform.position);
            }
        }
        UpdateMesh();
    }

    public void FloodFill(Vector3 position)
    {
        octree.FloodFill(position);
    }

    public void FloodErase(Vector3 pos, int range)
    {
        octree.FloodErase(pos, range);
        GenerateMesh();
    }

    public void FloodAtRange(Vector3 pos, int range)
    {
        octree.Flood(pos, range);
        GenerateMesh();
    }

    public Filler[] GetFillers()
    {
        Transform[] go = transform.parent.GetComponentsInChildren<Transform>();
        Filler[] fillers = new Filler[0];
        for (int i = 0; i < go.Length; i++)
        {
            if (go[i].name == "Fillers")
            {
                fillers = go[i].gameObject.GetComponentsInChildren<Filler>();
            }
        }
        return fillers;
    }

    public void GenerateMesh()
    {
        kNodes = GetKNodes(showedSubdivision);
        Mesh mesh = new Mesh();

        MeshFilter filter = transform.parent.GetComponent<MeshFilter>();
        MeshRenderer renderer = transform.parent.GetComponent<MeshRenderer>();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> scales = new List<Vector3>();
        List<int> indices = new List<int>();
        for (int y = 0; y < kNodes.GetLength(1); y++)
        {
            for (int x = 0; x < kNodes.GetLength(0); x++)
            {
                for (int z = 0; z < kNodes.GetLength(2); z++)
                {
                    //if (nodes[x, y, z].isBorder) print("IsBorder = true");
                    if (kNodes[x, y, z].isBorder /*|| kNodes[x, y, z].isInner*/){
                        vertices.Add(kNodes[x, y, z].transform.worldPos);
                        scales.Add(kNodes[x, y, z].transform.worldScale);
                        indices.Add(vertices.Count - 1);
                    }
                }
            }
        }

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.SetVertices(vertices);
        mesh.SetNormals(scales);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);
        filter.sharedMesh = mesh;

        Material mat = new Material(Shader.Find("Voxel/NewVoxelShader"));
        mat.SetColor("_MainColor", _Color);
        renderer.sharedMaterial = mat;
    }

    public void UpdateMesh()
    {
        Mesh mesh = new Mesh();

        MeshFilter filter = transform.parent.GetComponent<MeshFilter>();
        MeshRenderer renderer = transform.parent.GetComponent<MeshRenderer>();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> scales = new List<Vector3>();
        List<int> indices = new List<int>();
        for (int y = 0; y < kNodes.GetLength(1); y++)
        {
            for (int x = 0; x < kNodes.GetLength(0); x++)
            {
                for (int z = 0; z < kNodes.GetLength(2); z++)
                {
                    //if (nodes[x, y, z].isBorder) print("IsBorder = true");
                    if (kNodes[x, y, z].isBorder /*|| kNodes[x, y, z].isInner*/)
                    {
                        vertices.Add(kNodes[x, y, z].transform.worldPos);
                        scales.Add(kNodes[x, y, z].transform.worldScale);
                        indices.Add(vertices.Count - 1);
                    }
                }
            }
        }

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.SetVertices(vertices);
        mesh.SetNormals(scales);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);
        filter.sharedMesh = mesh;

        Material mat = new Material(Shader.Find("Voxel/NewVoxelShader"));
        mat.SetColor("_MainColor", _Color);
        renderer.sharedMaterial = mat;
    }

    OctreeNode[,,] GetKNodes(int k)
    {
        return octree.GetKNodes(k);
    }

    public void DrawTriangles(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        int[] indices = mesh.triangles;
        for (int i = 0; i < indices.Length; i += 3)
        {
            Vector3[] tV = new Vector3[3];
            for (int j = 0; j < 3; j++)
            {
                tV[j] = _Mesh.transform.TransformPoint(vertices[indices[i + j]]);
            }
            Gizmos.color = Color.red;
            Gizmos.DrawLine(tV[0], tV[1]);
            Gizmos.DrawLine(tV[1], tV[2]);
            Gizmos.DrawLine(tV[2], tV[0]);
            AABB.FromVertices(tV).Draw(Color.blue);
        }

    }

    List<Triangle> GetMeshTriangles(GameObject meshGO)
    {
        Mesh mesh = meshGO.GetComponent<MeshFilter>().sharedMesh;
        List<Triangle> triangles = new List<Triangle>();
        Vector3[] vertices = mesh.vertices;
        int[] indices = mesh.triangles;
        for (int i = 0; i < indices.Length; i += 3)
        {
            Vector3[] tV = new Vector3[3];
            for (int j = 0; j < 3; j++)
            {
                tV[j] = _Mesh.transform.TransformPoint(vertices[indices[i + j]]);
            }

            triangles.Add(new Triangle(tV));
        }
        return triangles;
    }
}