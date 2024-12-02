using UnityEngine;
/// <summary>
/// Settings for generating noise.
/// </summary>
[CreateAssetMenu(fileName = "noiseSettings", menuName = "Data/NoiseSettings")]
public class NoiseSettings : ScriptableObject
{
    
    /// <summary>
    /// World offset for the noise.
    /// </summary>
    public Vector2Int worldOffset;
    
    
    /// <summary>
    /// Number of octaves for noise generation.
    /// </summary>
    public int octaves;

    /// <summary>
    /// Zoom level of the noise.
    /// </summary>
    public float noiseZoom;
    
    /// <summary>
    /// Offset for the noise.
    /// </summary>
    public Vector2Int offest;
    
    /// <summary>
    /// Exponent for noise.
    /// </summary>
    public float exponent;
    
    /// <summary>
    /// Persistence of noise.
    /// </summary>
    public float persistance;

    /// <summary>
    /// Redistribution modifier for noise.
    /// </summary>
    public float redistributionModifier;

    
}
