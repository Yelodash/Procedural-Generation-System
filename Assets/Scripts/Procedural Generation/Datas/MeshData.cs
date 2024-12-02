using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// data structure representing mesh data.
/// </summary>
public class MeshData
{
    /// <summary>
    /// List of vertices.
    /// </summary>
    public List<Vector3> vertices = new List<Vector3>();

    /// <summary>
    /// List of triangles.
    /// </summary>
    public List<int> triangles = new List<int>();

    /// <summary>
    /// List of UV coordinates.
    /// </summary>
    public List<Vector2> uv = new List<Vector2>();

    /// <summary>
    /// List of vertices for collider mesh.
    /// </summary>
    public List<Vector3> colliderVertices = new List<Vector3>();

    /// <summary>
    /// List of triangles for collider mesh.
    /// </summary>
    public List<int> colliderTriangles = new List<int>();

    /// <summary>
    /// Water mesh data.
    /// </summary>
    public MeshData waterMesh;

    /// <summary>
    /// Flag indicating if it is in the main mesh.
    /// </summary>
    private bool IsItMainMesh;

    /// <summary>
    /// Initializes a new instance of the MeshData class.
    /// </summary>
    /// <param name="isMainMesh">Flag indicating if it's the main mesh.</param>
    public MeshData(bool isMainMesh)
    {
        this.IsItMainMesh = isMainMesh;
        if (isMainMesh)
        {
            waterMesh = new MeshData(false);
        }
    }

    /// <summary>
    /// Adds a vertex to the mesh data.
    /// </summary>
    /// <param name="vertex">The vertex to add.</param>
    /// <param name="vertexGeneratesCollider">Flag indicating if the vertex generates collider.</param>
    public void AddVertex(Vector3 vertex, bool vertexGeneratesCollider)
    {
        vertices.Add(vertex);
        if (vertexGeneratesCollider)
        {
            colliderVertices.Add(vertex);
        }
    }

    /// <summary>
    /// Adds quad triangles to the mesh data.
    /// </summary>
    /// <param name="quadGeneratesCollider">Flag indicating if the quad generates collider.</param>
    public void AddQuadTriangles(bool quadGeneratesCollider)
    {
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);

        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);

        if (quadGeneratesCollider)
        {
            colliderTriangles.Add(colliderVertices.Count - 4);
            colliderTriangles.Add(colliderVertices.Count - 3);
            colliderTriangles.Add(colliderVertices.Count - 2);
            colliderTriangles.Add(colliderVertices.Count - 4);
            colliderTriangles.Add(colliderVertices.Count - 2);
            colliderTriangles.Add(colliderVertices.Count - 1);
        }
    }
}
