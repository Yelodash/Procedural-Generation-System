using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
/// <summary>
/// Renders chunk for the terrain.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    Mesh mesh;

    /// <summary>
    /// Determines whether to show gizmos or not.
    /// </summary>
    public bool showGizmo = false;

    /// <summary>
    /// The chunk data associated with this renderer.
    /// </summary>
    public ChunkData ChunkData { get; private set; }


    /// <summary>
    /// Indicates if the chunk has been modified by the player.
    /// </summary>
    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        mesh = meshFilter.mesh;
    }

    /// <summary>
    /// Initializes the chunk with provided data.
    /// </summary>
    /// <param name="data">The chunk data.</param>
    public void InitializeChunk(ChunkData data)
    {
        ChunkData = data;
    }

    /// <summary>
    /// Renders the mesh.
    /// </summary>
    /// <param name="meshData"></param>
    private void RenderMesh(MeshData meshData)
    {
        mesh.Clear();

        mesh.subMeshCount = 2;
        mesh.vertices = meshData.vertices.Concat(meshData.waterMesh.vertices).ToArray();

        mesh.SetTriangles(meshData.triangles.ToArray(), 0);
        mesh.SetTriangles(meshData.waterMesh.triangles.Select(val => val + meshData.vertices.Count).ToArray(), 1);

        mesh.uv = meshData.uv.Concat(meshData.waterMesh.uv).ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshCollider.sharedMesh = null;
        Mesh collisionMesh = new Mesh();
        collisionMesh.vertices = meshData.colliderVertices.ToArray();
        collisionMesh.triangles = meshData.colliderTriangles.ToArray();
        collisionMesh.RecalculateNormals();
        collisionMesh.RecalculateBounds();
        meshCollider.sharedMesh = collisionMesh;
    }

    /// <summary>
    /// Updates the chunk.
    /// </summary>
    public void UpdateChunk()
    {
        RenderMesh(Chunk.ObtainChunkMeshData(ChunkData));
    }

    /// <summary>
    /// Updates the chunk with provided mesh data.
    /// </summary>
    /// <param name="data">The mesh data.</param>
    public void UpdateChunk(MeshData data)
    {
        RenderMesh(data);
    }
}