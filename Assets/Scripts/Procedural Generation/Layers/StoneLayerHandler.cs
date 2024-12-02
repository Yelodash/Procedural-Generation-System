using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the generation of stone layers within chunk data.
/// </summary>
public class StoneLayerHandler : VoxelLayerHandler
{
    /// <summary>
    /// The threshold value for stone generation.
    /// </summary>
    [Range(0, 1)]
    public float stoneThreshold = 0f;

    /// <summary>
    /// The noise settings used for generating stone noise.
    /// </summary>
    [SerializeField]
    private NoiseSettings stoneNoiseSettings;

    /// <summary>
    /// The domain warping component for generating domain noise.
    /// </summary>
    public DomainWarping domainWarping;

    /// <summary>
    /// Tries to handle stone voxel generation within the chunk data.
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
        if (chunkData.worldPosition.y > surfaceHeightNoise)
            return false;

        // Set the world offset for stone noise
        stoneNoiseSettings.worldOffset = mapSeedOffset;

        // Generate domain noise for stone
        float stoneNoise = domainWarping.GenerateDomainNoise(chunkData.worldPosition.x + x, chunkData.worldPosition.z + z, stoneNoiseSettings);

        // Determine the end position for stone generation
        int endPosition = surfaceHeightNoise;
        if (chunkData.worldPosition.y < 0)
        {
            endPosition = chunkData.worldPosition.y + chunkData.chunkHeight;
        }

        // If stone noise exceeds the threshold, generate stone voxels
        if (stoneNoise > stoneThreshold)
        {
            for (int i = chunkData.worldPosition.y; i <= endPosition; i++)
            {
                Vector3Int pos = new Vector3Int(x, i, z);
                Chunk.SetVoxel(chunkData, pos, VoxelType.Stone);
            }
            return true;
        }
        return false;
    }
}