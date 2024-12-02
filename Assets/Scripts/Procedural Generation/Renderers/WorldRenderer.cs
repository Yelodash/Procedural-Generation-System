using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Renders and manages chunks in the world.
/// </summary>
public class WorldRenderer : MonoBehaviour
{
    /// <summary>
    /// Prefab for individual chunks.
    /// </summary>
    public GameObject chunkPrefab;

    /// <summary>
    /// Queue to manage chunk pool for reusability.
    /// </summary>
    public Queue<ChunkRenderer> chunkPool = new Queue<ChunkRenderer>();

    /// <summary>
    /// Clears the world by destroying all chunk game objects and clearing the chunk pool.
    /// </summary>
    /// <param name="worldData">Data representing the world.</param>
    public void Clear(WorldData worldData)
    {
        foreach (var item in worldData.chunkDictionary.Values)
        {
            Destroy(item.gameObject);
        }
        chunkPool.Clear();
    }

    /// <summary>
    /// Renders a chunk at the specified position with given mesh data.
    /// </summary>
    /// <param name="worldData">Data representing the world.</param>
    /// <param name="position">Position to render the chunk.</param>
    /// <param name="meshData">Mesh data for the chunk.</param>
    /// <returns>The rendered chunk.</returns>
    internal ChunkRenderer RenderChunk(WorldData worldData, Vector3Int position, MeshData meshData)
    {
        ChunkRenderer newChunk = ObtainOrCreateChunk(position);
        newChunk.InitializeChunk(worldData.chunkDataDictionary[position]);
        newChunk.UpdateChunk(meshData);
        newChunk.gameObject.SetActive(true);
        return newChunk;
    }

    /// <summary>
    /// Removes a chunk from the world and adds it back to the chunk pool for reusability.
    /// </summary>
    /// <param name="chunk">Chunk to be removed.</param>
    public void RemoveChunk(ChunkRenderer chunk)
    {
        chunk.gameObject.SetActive(false);
        chunkPool.Enqueue(chunk);
    }

    /// <summary>
    /// Gets an available chunk from the pool or instantiates a new one if pool is empty.
    /// </summary>
    /// <param name="position">Position to place the chunk.</param>
    /// <returns>An available chunk.</returns>
    private ChunkRenderer ObtainOrCreateChunk(Vector3Int position)
    {
        ChunkRenderer newChunk = null;
        if (chunkPool.Count > 0)
        {
            newChunk = chunkPool.Dequeue();
            newChunk.transform.position = position;
        }
        else
        {
            GameObject chunkObject = Instantiate(chunkPrefab, position, Quaternion.identity);
            newChunk = chunkObject.GetComponent<ChunkRenderer>();
        }
        return newChunk;
    }
}
