using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// data structure representing world generation data.
/// </summary>
public class WorldGenerationData
{
    /// <summary>
    /// chunk positions to create.
    /// </summary>
    public List<Vector3Int> chunkPositionsToCreate = new List<Vector3Int>();
    
    /// <summary>
    /// chunk data positions to create.
    /// </summary>
    public List<Vector3Int> chunkDataPositionsToCreate = new List<Vector3Int>();
    
    /// <summary>
    /// chunk positions to remove.
    /// </summary>
    public List<Vector3Int> chunkPositionsToRemove = new List<Vector3Int>();
    
    
    /// <summary>
    ///  chunk data to remove.
    /// </summary>
    public List<Vector3Int> chunkDataToRemove = new List<Vector3Int>();
    
    
    /// <summary>
    ///chunk positions to update.
    /// </summary>
    public List<Vector3Int> chunkPositionsToUpdate = new List<Vector3Int>();
}

