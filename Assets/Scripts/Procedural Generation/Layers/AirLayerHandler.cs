using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the generation of air voxels above the surface within chunk data.
/// </summary>
public class AirLayerHandler : VoxelLayerHandler
{
    /// <summary>
    /// Tries to handle air voxel generation within the chunk data.
    /// </summary>
    /// <param name="chunkData">The chunk data to modify.</param>
    /// <param name="x">The X-coordinate within the chunk.</param>
    /// <param name="y">The Y-coordinate within the chunk.</param>
    /// <param name="z">The Z-coordinate within the chunk.</param>
    /// <param name="surfaceHeightNoise">The noise value representing surface height.</param>
    /// <param name="mapSeedOffset">The offset for the map seed.</param>
    /// <returns>True if handling is successful, otherwise false.</returns>
    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        // Check if the current position is above the surface
        if (y > surfaceHeightNoise)
        {
            // Set the voxel at the current position to air
            Vector3Int pos = new Vector3Int(x, y, z);
            Chunk.SetVoxel(chunkData, pos, VoxelType.Air);
            return true;
        }
        return false;
    }
}