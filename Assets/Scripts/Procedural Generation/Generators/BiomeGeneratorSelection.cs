using UnityEngine;
/// <summary>
/// Represents the selected biome generator and associated terrain surface noise.
/// </summary>
public class BiomeGeneratorSelection
{
    /// <summary>
    /// The selected biome generator.
    /// </summary>
    public BiomeGenerator biomeGenerator = null;

    /// <summary>
    /// The terrain surface noise.
    /// </summary>
    public int? terrainSurfaceNoise = null;

    /// <summary>
    /// Initializes a new instance of the BiomeGeneratorSelection class.
    /// </summary>
    /// <param name="biomeGeneror">The selected biome generator.</param>
    /// <param name="terrainSurfaceNoise">The terrain surface noise.</param>
    public BiomeGeneratorSelection(BiomeGenerator biomeGeneror, int? terrainSurfaceNoise = null)
    {
        this.biomeGenerator = biomeGeneror;
        this.terrainSurfaceNoise = terrainSurfaceNoise;
    }
}
