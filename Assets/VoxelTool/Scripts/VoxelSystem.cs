using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EditorCoroutines;

[ExecuteInEditMode]
public class VoxelSystem : MonoBehaviour
{
    [Range(0,10)]
    public int _Depth;
    public GameObject _Mesh;
    public bool _DebugDisplay = false;
    public Color _Color;
    //public Transform transform;

    private int showedSubdivision;
    int prevShowedSubdivision;
    private Octree octree;

    Mesh debug_Mesh;
    bool debug_Drawing = false;
    Vector3Int debugAux = new Vector3Int();

    public Transform indexCheckerTransform; 

    public void Update()
    {
        if(octree == null)
        {
            octree = new Octree();
            
        }
        if (octree.root != null && transform.hasChanged)
        {
            print("Refreshing");
            //octree.Refresh(transform);
        }
        if (octree.root != null)
        {
            //CheckCube();
        }
        
    }

    private void OnValidate()
    {
        showedSubdivision = _Depth;
        //GenerateMesh();
        //nodes = GetKNodes(showedSubdivision);
    }

    private void OnDrawGizmos()
    {
        if (octree != null && octree.root != null) 
        {
            if(_DebugDisplay) octree.Display(showedSubdivision);
        }
    }

    public void Create()
    {
        showedSubdivision = _Depth;
        prevShowedSubdivision = showedSubdivision;
        debugAux = Vector3Int.zero;
        octree.Create(_Depth, transform);
    }

    public void Bake()
    {
        double bakingTimeCounter = EditorApplication.timeSinceStartup;
        octree.Bake(GetMeshTriangles(_Mesh));
        print(EditorApplication.timeSinceStartup - bakingTimeCounter);
    }

    public void FloodFill()
    {
        Vector3 pos = indexCheckerTransform.position;
        octree.Flood(pos);
    }

    public void DebugDraw()
    {
        debug_Mesh = _Mesh.GetComponent<MeshFilter>().sharedMesh;
    }

    public void GenerateMesh()
    {
        OctreeNode[,,] nodes = GetKNodes(showedSubdivision);
        print("Node legnth: " + nodes.Length);
        MeshFilter filter = GetComponent<MeshFilter>();
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> scales = new List<Vector3>();
        List<int> indices = new List<int>();
        for (int y = 0; y < nodes.GetLength(1); y++)
        {
            for (int x = 0; x < nodes.GetLength(0); x++)
            {
                for (int z = 0; z < nodes.GetLength(2); z++)
                {
                    if (nodes[x, y, z].isBorder) print("IsBorder = true");
                    if (nodes[x, y, z].isBorder || nodes[x, y, z].isInner){
                        vertices.Add(nodes[x, y, z].transform.worldPos);
                        scales.Add(nodes[x, y, z].transform.worldScale);
                        indices.Add(vertices.Count - 1);
                    }
                }
            }
        }

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.SetVertices(vertices);
        print("Vertices[" + mesh.vertices.Length + "]");
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

    public void CheckCube()
    {
        octree.GetNode(indexCheckerTransform.position, showedSubdivision).isSelected = true;
    }
}