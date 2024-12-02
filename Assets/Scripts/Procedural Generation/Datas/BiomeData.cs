using UnityEngine;
/// <summary>
/// Data structure representing biome data.
/// </summary>
[System.Serializable]
public struct BiomeData
{
    /// <summary>
    /// Start Threshold of the biome.
    /// </summary>
    [Range(0f, 1f)]
    public float temperatureStartThreshold;

    /// <summary>
    /// End Threshold of the biome.
    /// </summary>
    [Range(0f, 1f)]
    public float temperatureEndThreshold;
    
    /// <summary>
    ///  Biome generator for the terrain.
    /// </summary>
    public BiomeGenerator biomeTerrainGenerator;
}
