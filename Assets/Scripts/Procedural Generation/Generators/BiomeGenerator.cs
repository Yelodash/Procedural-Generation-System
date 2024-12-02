using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Generates and processes biome-related data for chunks.
/// </summary>
public class BiomeGenerator : MonoBehaviour
{
    /// <summary>
    /// Threshold for considering water in the biome.
    /// </summary>
    public int waterThreshold = 0;

    /// <summary>
    /// Settings for generating the biome noise.
    /// </summary>
    public NoiseSettings biomeNoiseSettings;

    /// <summary>
    /// Domain warping utility for generating domain noise.
    /// </summary>
    public DomainWarping domainWarping;

    /// <summary>
    /// Indicates whether to use domain warping for noise generation.
    /// </summary>
    public bool useDomainWarping = true;

    /// <summary>
    /// Handler for the starting layer of voxels.
    /// </summary>
    public VoxelLayerHandler startLayerHandler;

    /// <summary>
    /// Generator for trees within the biome.
    /// </summary>
    public TreeGenerator treeGenerator;

    /// <summary>
    /// Gets the tree data for the specified chunk.
    /// </summary>
    /// <param name="data">Chunk data.</param>
    /// <param name="mapSeedOffset">Offset for the map seed.</param>
    /// <returns>Tree data for the chunk.</returns>
    internal TreeData ObtainTreeData(ChunkData data, Vector2Int mapSeedOffset)
    {
        if (treeGenerator == null)
            return new TreeData();
        return treeGenerator.GenerateTreeData(data, mapSeedOffset);
    }

    /// <summary>
    /// List of additional voxel layer handlers.
    /// </summary>
    public List<VoxelLayerHandler> additionalLayerHandlers;

    /// <summary>
    /// Processes a chunk column based on biome data.
    /// </summary>
    /// <param name="data">Chunk data.</param>
    /// <param name="x">X-coordinate of the chunk.</param>
    /// <param name="z">Z-coordinate of the chunk.</param>
    /// <param name="mapSeedOffset">Offset for the map seed.</param>
    /// <param name="terrainHeightNoise">Optional terrain height noise.</param>
    /// <returns>Processed chunk data.</returns>
    public ChunkData ProcessChunkColumn(ChunkData data, int x, int z, Vector2Int mapSeedOffset, int? terrainHeightNoise)
    {
        biomeNoiseSettings.worldOffset = mapSeedOffset;

        int groundPosition;
        if (terrainHeightNoise.HasValue == false)
            groundPosition = ObtainSurfaceHeightNoise(data.worldPosition.x + x, data.worldPosition.z + z, data.chunkHeight);
        else
            groundPosition = terrainHeightNoise.Value;

        for (int y = data.worldPosition.y; y < data.worldPosition.y + data.chunkHeight; y++)
        {
            startLayerHandler.Handle(data, x, y, z, groundPosition, mapSeedOffset);
        }

        foreach (var layer in additionalLayerHandlers)
        {
            layer.Handle(data, x, data.worldPosition.y, z, groundPosition, mapSeedOffset);
        }
        return data;
    }

    /// <summary>
    /// Calculates surface height using biome noise.
    /// </summary>
    /// <param name="x">X-coordinate.</param>
    /// <param name="z">Z-coordinate.</param>
    /// <param name="chunkHeight">Chunk height.</param>
    /// <returns>Surface height.</returns>
    public int ObtainSurfaceHeightNoise(int x, int z, int chunkHeight)
    {
        float terrainHeight;
        if(useDomainWarping == false)
        {
            terrainHeight = Noise.OctavePerlin(x, z, biomeNoiseSettings);
        }
        else
        {
            terrainHeight = domainWarping.GenerateDomainNoise(x, z, biomeNoiseSettings);
        }

        terrainHeight = Noise.Redistribution(terrainHeight, biomeNoiseSettings);
        int surfaceHeight = Noise.RemapValue01ToInt(terrainHeight, 0, chunkHeight);
        return surfaceHeight;
    }
}
