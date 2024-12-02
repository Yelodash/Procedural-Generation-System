using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Represents the type of voxel.
/// </summary>
public enum VoxelType
{
    /// <summary>
    /// Represents nothing.
    /// </summary>
    Nothing,
    
    /// <summary>
    /// Represents air.
    /// </summary>
    Air,
    
    /// <summary>
    /// Represents grass.
    /// </summary>
    Grass_Dirt,
    
    /// <summary>
    ///  Represents dirt.
    /// </summary>
    Dirt,
    
    /// <summary>
    /// Represents grass and stone.
    /// </summary>
    Grass_Stone,
    
    /// <summary>
    /// Represents stone.
    /// </summary>
    Stone,
    
    /// <summary>
    /// Represents tree trunk.
    /// </summary>
    TreeTrunk,
    
    /// <summary>
    ///  Represents tree leafes.
    /// </summary>
    TreeLeafesTransparent,
    
    /// <summary>
    /// Represents tree leafes.
    /// </summary>
    TreeLeafsSolid,
    
    /// <summary>
    /// Represents water.
    /// </summary>
    Water,
    
    /// <summary>
    ///  Represents sand.
    /// </summary>
    Sand,
    
    Snow
}