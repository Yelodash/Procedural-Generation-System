using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///  data structure representing world data.
/// </summary>
public class WorldData
{
    /// <summary>
    /// Dictionary storing chunk data.
    /// </summary>
    public Dictionary<Vector3Int, ChunkData> chunkDataDictionary;
    
    
    /// <summary>
    /// Dictionary storing chunk renderers.
    /// </summary>
    public Dictionary<Vector3Int, ChunkRenderer> chunkDictionary;
    
    
    /// <summary>
    /// The size of the chunk along each axis.
    /// </summary>
    public int chunkSize;
    
    
    /// <summary>
    ///The height of the chunk.
    /// </summary>
    public int chunkHeight;

    /// <summary>
    ///Initializes a new instance of the WorldData class with specified parameters.
    /// </summary>
    /// <param name="size"></param>
    /// <param name="height"></param>
    public WorldData(int size, int height)
    {
        chunkSize = size;
        chunkHeight = height;
        chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>();
        chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>();
    }
}