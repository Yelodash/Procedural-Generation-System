using UnityEngine;
/// <summary>
/// Handles domain warping for noise generation.
/// </summary>
public class DomainWarping : MonoBehaviour
{
    /// <summary>
    /// Noise settings for domain warping in the X direction.
    /// </summary>
    public NoiseSettings noiseDomainX;

    /// <summary>
    /// Noise settings for domain warping in the Y direction.
    /// </summary>
    public NoiseSettings noiseDomainY;

    /// <summary>
    /// Amplitude of domain warping in the X direction.
    /// </summary>
    public int amplitudeX = 20;

    /// <summary>
    /// Amplitude of domain warping in the Y direction.
    /// </summary>
    public int amplitudeY = 20;

    /// <summary>
    /// Generates domain noise at a given position.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="z">The z coordinate.</param>
    /// <param name="defaultNoiseSettings">Default noise settings.</param>
    /// <returns>The generated domain noise.</returns>
    public float GenerateDomainNoise(int x, int z, NoiseSettings defaultNoiseSettings)
    {
        Vector2 domainOffset = GenerateDomainOffset(x, z);
        return Noise.OctavePerlin(x + domainOffset.x, z + domainOffset.y, defaultNoiseSettings);
    }

    /// <summary>
    /// Generates domain offset at a given position.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="z">The z coordinate.</param>
    /// <returns>The generated domain offset.</returns>
    public Vector2 GenerateDomainOffset(int x, int z)
    {
        var noiseX = Noise.OctavePerlin(x, z, noiseDomainX) * amplitudeX;
        var noiseY = Noise.OctavePerlin(x, z, noiseDomainY) * amplitudeY;
        return new Vector2(noiseX, noiseY);
    }

    /// <summary>
    /// Generates domain offset as integer values at a given position.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="z">The z coordinate.</param>
    /// <returns>The generated domain offset as integer values.</returns>
    public Vector2Int GenerateDomainOffsetInt(int x, int z)
    {
        return Vector2Int.RoundToInt(GenerateDomainOffset(x, z));
    }
}
