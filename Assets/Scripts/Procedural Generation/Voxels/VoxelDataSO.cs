using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ScriptableObject containing data for different types of voxels.
/// </summary>
[CreateAssetMenu(fileName = "Voxel Data", menuName = "Data/Voxel Data")]
public class VoxelDataSO : ScriptableObject
{
    /// <summary>
    /// Size of textures on the X-axis.
    /// </summary>
    public float textureSizeX;

    /// <summary>
    /// Size of textures on the Y-axis.
    /// </summary>
    public float textureSizeY;

    /// <summary>
    /// List of texture data for different voxel types.
    /// </summary>
    public List<TextureData> textureDataList;
}

/// <summary>
/// Data structure representing texture information for a voxel type.
/// </summary>
[Serializable]
public class TextureData
{
    /// <summary>
    /// Type of the voxel.
    /// </summary>
    public VoxelType voxelType;

    /// <summary>
    /// Texture coordinates for the top face of the voxel.
    /// </summary>
    public Vector2Int up;

    /// <summary>
    /// Texture coordinates for the bottom face of the voxel.
    /// </summary>
    public Vector2Int down;

    /// <summary>
    /// Texture coordinates for the side faces of the voxel.
    /// </summary>
    public Vector2Int side;

    /// <summary>
    /// Indicates whether the voxel is solid.
    /// </summary>
    public bool isSolid = true;

    /// <summary>
    /// Indicates whether the voxel generates collider.
    /// </summary>
    public bool generatesCollider = true;
}