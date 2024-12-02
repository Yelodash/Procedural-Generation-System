using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the generation of underground layers within chunk data.
/// </summary>
public class UndergroundLayerHandler : VoxelLayerHandler
{
    /// <summary>
    /// The type of voxel to be generated underground.
    /// </summary>
    public VoxelType undergroundVoxelType;

    /// <summary>
    /// Tries to handle underground voxel generation within the chunk data.
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
        if (y < surfaceHeightNoise)
        {
            // Adjust the position relative to the chunk's world position
            Vector3Int pos = new Vector3Int(x, y - chunkData.worldPosition.y, z);

            // Set the voxel at the current position to the specified underground block type
            Chunk.SetVoxel(chunkData, pos, undergroundVoxelType);
            return true;
        }
        return false;
    }
}