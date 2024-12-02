using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the generation of water layers within chunk data.
/// </summary>
public class WaterLayerHandler : VoxelLayerHandler
{
    /// <summary>
    /// The level at which water should be generated.
    /// </summary>
    public int waterLevel = 1;

    /// <summary>
    /// Tries to handle water generation within the chunk data.
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
        if (y > surfaceHeightNoise && y <= waterLevel)
        {
            Vector3Int pos = new Vector3Int(x, y, z);

            // Set the voxel at the current position to water
            Chunk.SetVoxel(chunkData, pos, VoxelType.Water);

            // If the water surface is at one voxel above the ground, set the voxel below to sand
            if (y == surfaceHeightNoise + 1)
            {
                pos.y = surfaceHeightNoise;
                Chunk.SetVoxel(chunkData, pos, VoxelType.Sand);
            }
            return true;
        }
        return false;
    }
}