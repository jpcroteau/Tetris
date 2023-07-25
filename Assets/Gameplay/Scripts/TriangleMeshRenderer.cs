using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TriangleMeshRenderer : MonoBehaviour
{
    private void Start()
    {
        GetComponent<MeshFilter>().mesh = CreateTriangleMesh();
    }

    private Mesh CreateSquareMesh()
    {
        var mesh = new Mesh();

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0f, 0f, 0f),       // Bottom Left
            new Vector3(1f, 0f, 0f),       // Bottom Right
            new Vector3(1f, 1f, 0f),       // Top Right
            new Vector3(0f, 1f, 0f)        // Top Left
        };
        
        int[] triangles = new int[]
        {
            0, 2, 1,
            0, 3, 2
        };

        Vector2[] uv = new Vector2[]
        {
            new Vector2(0f, 0f),           // UV for Bottom Left vertex
            new Vector2(1f, 0f),           // UV for Bottom Right vertex
            new Vector2(1f, 1f),           // UV for Top Right vertex
            new Vector2(0f, 1f)            // UV for Top Left vertex
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        return mesh;
    }
    
    private Mesh CreateTriangleMesh()
    {
        var mesh = new Mesh();

        Vector3[] vertices = new Vector3[3];
        Vector2[] uv = new Vector2[3];
        int[] triangles = new int[3];
        
        // Bottom Left
        vertices[0] = new Vector3(0f, 0f, 0f);                               
        uv[0] = new Vector2(0f, 0f);
        
        // Bottom Right
        vertices[1] = new Vector3(1f*Screen.width, 0f, 0f);  
        uv[1] = new Vector2(1f, 0f);
        
        // Center
        vertices[2] = new Vector3(0.5f*Screen.width, 0.5f*Screen.height, 0f);
        uv[2] = new Vector2(0.5f, 0.5f);
        
        triangles = new int[] { 0, 2, 1 };
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv; // Assign the UV coordinates to the mesh

        return mesh;
    }
}