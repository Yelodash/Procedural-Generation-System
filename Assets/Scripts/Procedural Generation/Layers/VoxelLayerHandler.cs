using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Abstract class for handling voxel layers within chunk data.
/// </summary>
public abstract class VoxelLayerHandler : MonoBehaviour
{
    /// <summary>
    /// Reference to the next voxel layer handler in the chain.
    /// </summary>
    [SerializeField]
    private VoxelLayerHandler Next;

    /// <summary>
    /// Handles voxel generation within the chunk data.
    /// </summary>
    /// <param name="chunkData">The chunk data to modify.</param>
    /// <param name="x">The X-coordinate within the chunk.</param>
    /// <param name="y">The Y-coordinate within the chunk.</param>
    /// <param name="z">The Z-coordinate within the chunk.</param>
    /// <param name="surfaceHeightNoise">The noise value representing surface height.</param>
    /// <param name="mapSeedOffset">The offset for the map seed.</param>
    /// <returns>True if handling is successful, otherwise false.</returns>
    public bool Handle(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        // Attempt to handle voxel generation with the current handler
        if (TryHandling(chunkData, x, y, z, surfaceHeightNoise, mapSeedOffset))
            return true;

        // If handling is unsuccessful and there is a next handler, pass the handling to it
        if (Next != null)
            return Next.Handle(chunkData, x, y, z, surfaceHeightNoise, mapSeedOffset);

        // If there is no next handler and handling is unsuccessful, return false
        return false;
    }

    /// <summary>
    /// Abstract method for attempting voxel generation within the chunk data.
    /// </summary>
    /// <param name="chunkData">The chunk data to modify.</param>
    /// <param name="x">The X-coordinate within the chunk.</param>
    /// <param name="y">The Y-coordinate within the chunk.</param>
    /// <param name="z">The Z-coordinate within the chunk.</param>
    /// <param name="surfaceHeightNoise">The noise value representing surface height.</param>
    /// <param name="mapSeedOffset">The offset for the map seed.</param>
    /// <returns>True if handling is successful, otherwise false.</returns>
    protected abstract bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset);
}