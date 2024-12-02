using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages block data including textures and texture offsets.
/// </summary>
public class VoxelDataManager : MonoBehaviour
{
    /// <summary>
    /// Offset for texture mapping to prevent texture bleeding.
    /// </summary>
    public static float textureOffset = 0.000f;

    /// <summary>
    /// Size of textures on the X-axis.
    /// </summary>
    public static float tileSizeX;

    /// <summary>
    /// Size of textures on the Y-axis.
    /// </summary>
    public static float tileSizeY;

    /// <summary>
    /// Dictionary containing texture data for each voxel type.
    /// </summary>
    public static Dictionary<VoxelType, TextureData> voxelTextureDataDictionary = new Dictionary<VoxelType, TextureData>();

    /// <summary>
    /// Scriptable object containing the voxel texture data.
    /// </summary>
    public VoxelDataSO textureData;

    private void Awake()
    {
        InitializeVoxelTextureData();
    } 

    /// <summary>
    /// Initializes the voxel  texture data dictionary and sets texture tile sizes.
    /// </summary>
    private void InitializeVoxelTextureData()
    {
        foreach (var item in textureData.textureDataList)
        {
            if (!voxelTextureDataDictionary.ContainsKey(item.voxelType))
            {
                voxelTextureDataDictionary.Add(item.voxelType, item);
            }
        }
        tileSizeX = textureData.textureSizeX;
        tileSizeY = textureData.textureSizeY;
    }
}