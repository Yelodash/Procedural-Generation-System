using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Generates tree-related data based on noise settings and chunk data.
/// </summary>
public class TreeGenerator : MonoBehaviour
{
    /// <summary>
    /// Settings for generating tree noise.
    /// </summary>
    public NoiseSettings treeNoiseSettings;

    /// <summary>
    /// Domain warping utility for generating domain noise.
    /// </summary>
    public DomainWarping domainWrapping;

    /// <summary>
    /// Generates tree data for the specified chunk.
    /// </summary>
    /// <param name="chunkData">Chunk data.</param>
    /// <param name="mapSeedOffset">Offset for the map seed.</param>
    /// <returns>Tree data for the chunk.</returns>
    public TreeData GenerateTreeData(ChunkData chunkData, Vector2Int mapSeedOffset)
    {
        treeNoiseSettings.worldOffset = mapSeedOffset;
        TreeData treeData = new TreeData();
        float[,] noiseData = GenerateTreeNoise(chunkData);
        treeData.treePositions = DataProcessing.FindLocalMaximum(noiseData, chunkData.worldPosition.x, chunkData.worldPosition.z);
        return treeData;
    }

    /// <summary>
    /// Generates tree noise for the specified chunk.
    /// </summary>
    /// <param name="chunkData">Chunk data.</param>
    /// <returns>2D array representing tree noise.</returns>
    private float[,] GenerateTreeNoise(ChunkData chunkData)
    {
        float[,] noiseMax = new float[chunkData.chunkSize, chunkData.chunkSize];
        int xMax = chunkData.worldPosition.x + chunkData.chunkSize;
        int xMin = chunkData.worldPosition.x;
        int zMax = chunkData.worldPosition.z + chunkData.chunkSize;
        int zMin = chunkData.worldPosition.z;
        int xIndex = 0, zIndex = 0;
        for (int x = xMin; x < xMax; x++)
        {
            for (int z = zMin; z < zMax; z++)
            {
                noiseMax[xIndex, zIndex] = domainWrapping.GenerateDomainNoise(x, z, treeNoiseSettings);
                zIndex++;
            }
            xIndex++;
            zIndex = 0;
        }
        return noiseMax;
    }
}