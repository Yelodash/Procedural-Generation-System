using UnityEngine;
/// <summary>
/// Utility class for generating noise.
/// </summary>
public static class Noise
{
    /// <summary>
    /// Remaps a value from one range to another.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="initialMin"></param>
    /// <param name="initialMax"></param>
    /// <param name="outputMin"></param>
    /// <param name="outputMax"></param>
    /// <returns></returns>
    public static float RemapValue(float value, float initialMin, float initialMax, float outputMin, float outputMax)
    {
        return outputMin + (value - initialMin) * (outputMax - outputMin) / (initialMax - initialMin);
    }
    
    /// <summary>
    /// Remaps a value from 0 to 1 to a new range.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="outputMin"></param>
    /// <param name="outputMax"></param>
    /// <returns></returns>
    public static float RemapValue01(float value, float outputMin, float outputMax)
    {
        return outputMin + (value - 0) * (outputMax - outputMin) / (1 - 0);
    }

    /// <summary>
    /// Remaps a value from 0 to 1 to a new range and returns an integer.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="outputMin"></param>
    /// <param name="outputMax"></param>
    /// <returns></returns>
    public static int RemapValue01ToInt(float value, float outputMin, float outputMax)
    {
        return (int)RemapValue01(value, outputMin, outputMax);
    }

    /// <summary>
    ///Redistributes noise values.
    /// </summary>
    /// <param name="noise"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public static float Redistribution(float noise, NoiseSettings settings)
    {
        return Mathf.Pow(noise * settings.redistributionModifier, settings.exponent);
    }

    /// <summary>
    /// Generates Perlin noise.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public static float OctavePerlin(float x, float z, NoiseSettings settings)
    {
        x *= settings.noiseZoom;
        z *= settings.noiseZoom;
        x += settings.noiseZoom;
        z += settings.noiseZoom;

        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float amplitudeSum = 0;  
        for (int i = 0; i < settings.octaves; i++)
        {
            total += Mathf.PerlinNoise((settings.offest.x + settings.worldOffset.x + x) * frequency, (settings.offest.y + settings.worldOffset.y + z) * frequency) * amplitude;

            amplitudeSum += amplitude;

            amplitude *= settings.persistance;
            frequency *= 2;
        }

        return total / amplitudeSum;
    }
}
/// <summary>
/// Enumeration for noise types.
/// </summary>
public enum NoiseType
{
    Perlin,
    
}
