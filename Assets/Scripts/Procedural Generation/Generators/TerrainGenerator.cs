using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Generates terrain chunks based on selected biomes.
/// </summary>
public class TerrainGenerator : MonoBehaviour

{
    /// <summary>
    /// Reference to the biome generator.
    /// </summary>
    public BiomeGenerator biomeGenerator;

    /// <summary>
    /// The biome centers.
    /// </summary>
    [SerializeField]
    private List<Vector3Int> biomeCenters = new List<Vector3Int>();
    
    /// <summary>
    /// Biome noise.
    /// </summary>
    private List<float> biomeNoise = new List<float>();

    [SerializeField]
    private NoiseSettings biomeNoiseSettings;

    /// <summary>
    /// Domain warping for biomes.
    /// </summary>
    public DomainWarping biomeDomainWarping;

    /// <summary>
    ///Biome data for biome generators.
    /// </summary>
    [SerializeField]
    private List<BiomeData> biomeGeneratorsData = new List<BiomeData>();

    /// <summary>
    /// Generates chunk data for terrain.
    /// </summary>
    /// <param name="data">The chunk data.</param>
    /// <param name="mapSeedOffset">The offset for the map seed.</param>
    /// <returns>The generated chunk data.</returns>
    
    public interface ITerrainGeneration
    {
        ChunkData GenerateChunkData(ChunkData data, Vector2Int mapSeedOffset);
        
    }
    
    /// <summary>
    /// Represents the selected biome generator and associated terrain surface noise.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="mapSeedOffset"></param>
    /// <returns></returns>
    public ChunkData GenerateChunkData(ChunkData data, Vector2Int mapSeedOffset)
    {
        
        BiomeGeneratorSelection biomeSelection = SelectBiomeGenerator(data.worldPosition, data, false);
        data.treeData = biomeSelection.biomeGenerator.ObtainTreeData(data, mapSeedOffset);
        for (int x = 0; x < data.chunkSize; x++)
        {
            for (int z = 0; z < data.chunkSize; z++)
            {
                biomeSelection = SelectBiomeGenerator(new Vector3Int(data.worldPosition.x + x, 0, data.worldPosition.z + z), data);
                data = biomeSelection.biomeGenerator.ProcessChunkColumn(data, x, z, mapSeedOffset, biomeSelection.terrainSurfaceNoise);
            }
        }
        return data;
    }

    /// <summary>
    /// Selects the biome generator based on the world position.
    /// </summary>
    /// <param name="worldPosition">The world position.</param>
    /// <param name="data">The chunk data.</param>
    /// <param name="useDomainWarping">Flag to indicate whether to use domain warping.</param>
    /// <returns>The selected biome generator.</returns>
    private BiomeGeneratorSelection SelectBiomeGenerator(Vector3Int worldPosition, ChunkData data, bool useDomainWarping = true)
    {
        if (useDomainWarping)
        {
            Vector2Int domainOffset = Vector2Int.RoundToInt(biomeDomainWarping.GenerateDomainOffset(worldPosition.x, worldPosition.z));
            worldPosition += new Vector3Int(domainOffset.x, 0, domainOffset.y);
        }

        List<BiomeSelectionHelper> biomeSelectionHelpers = ObtainBiomeGeneratorSelectionHelpers(worldPosition);
        BiomeGenerator generator_1 = SelectBiome(biomeSelectionHelpers[0].Index);
        BiomeGenerator generator_2 = SelectBiome(biomeSelectionHelpers[1].Index);

        float distance = Vector3.Distance(biomeCenters[biomeSelectionHelpers[0].Index], biomeCenters[biomeSelectionHelpers[1].Index]);
        float weight_0 = biomeSelectionHelpers[0].Distance / distance;
        float weight_1 = 1 - weight_0;
        int terrainHeightNoise_0 = generator_1.ObtainSurfaceHeightNoise(worldPosition.x, worldPosition.z, data.chunkHeight);
        int terrainHeightNoise_1 = generator_2.ObtainSurfaceHeightNoise(worldPosition.x, worldPosition.z, data.chunkHeight);
        return new BiomeGeneratorSelection(generator_1, Mathf.RoundToInt(terrainHeightNoise_0 * weight_0 + terrainHeightNoise_1 * weight_1));
    }

    /// <summary>
    /// Selects the biome generator based on index.
    /// </summary>
    /// <param name="index">The index of the biome generator.</param>
    /// <returns>The selected biome generator.</returns>
    private BiomeGenerator SelectBiome(int index)
    {
        float temp = biomeNoise[index];
        foreach (var data in biomeGeneratorsData)
        {
            if (temp >= data.temperatureStartThreshold && temp < data.temperatureEndThreshold)
                return data.biomeTerrainGenerator;
        }
        return biomeGeneratorsData[0].biomeTerrainGenerator;
    }

    /// <summary>
    /// Retrieves biome generator selection helpers based on position.
    /// </summary>
    /// <param name="position">The position in the world.</param>
    /// <returns>List of biome generator selection helpers.</returns>
    private List<BiomeSelectionHelper> ObtainBiomeGeneratorSelectionHelpers(Vector3Int position)
    {
        position.y = 0;
        return GetClosestBiomeIndex(position);
    }

    /// <summary>
    /// Retrieves closest biome index based on the position.
    /// </summary>
    /// <param name="position">The position in the world.</param>
    /// <returns>List of biome selection helpers.</returns>
    private List<BiomeSelectionHelper> GetClosestBiomeIndex(Vector3Int position)
    {
        return biomeCenters.Select((center, index) =>
        new BiomeSelectionHelper
        {
            Index = index,
            Distance = Vector3.Distance(center, position)
        }).OrderBy(helper => helper.Distance).Take(4).ToList();
    }

    /// <summary>
    /// Generates biome points based on the player position.
    /// </summary>
    /// <param name="playerPosition">The player position.</param>
    /// <param name="drawRange">The draw range.</param>
    /// <param name="mapSize">The size of the map.</param>
    /// <param name="mapSeedOffset">The offset for the map seed.</param>
    public void GenerateBiomePoints(Vector3 playerPosition, int drawRange, int mapSize, Vector2Int mapSeedOffset)
    {
        biomeCenters = BiomeCenterFinder.CalculateBiomeCenters(playerPosition, drawRange, mapSize);

        for (int i = 0; i < biomeCenters.Count; i++)
        {
            Vector2Int domainWarpingOffset = biomeDomainWarping.GenerateDomainOffsetInt(biomeCenters[i].x, biomeCenters[i].y);
            biomeCenters[i] += new Vector3Int(domainWarpingOffset.x, 0, domainWarpingOffset.y);
        }
        biomeNoise = CalculateBiomeNoise(biomeCenters, mapSeedOffset);
    }

    /// <summary>
    /// Calculates biome noise based on centers and map seed offset.
    /// </summary>
    /// <param name="biomeCenters">The centers of biomes.</param>
    /// <param name="mapSeedOffset">The offset for the map seed.</param>
    /// <returns>List of biome noises.</returns>
    private List<float> CalculateBiomeNoise(List<Vector3Int> biomeCenters, Vector2Int mapSeedOffset)
    {
        biomeNoiseSettings.worldOffset = mapSeedOffset;
        return biomeCenters.Select(center => Noise.OctavePerlin(center.x, center.z, biomeNoiseSettings)).ToList();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        foreach (var biomCenterPoint in biomeCenters)
        {
            Gizmos.DrawLine(biomCenterPoint, biomCenterPoint + Vector3.up * 255);
        }
    }
}


