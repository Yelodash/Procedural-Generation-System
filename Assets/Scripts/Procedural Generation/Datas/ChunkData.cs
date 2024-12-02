using UnityEngine;
/// <summary>
/// Data structure representing the chunk data.
/// </summary>
public class ChunkData
{
    
    /// <summary>
    /// The size of the chunk along each axis.
    /// </summary>
    public int chunkSize = 0;
    
    /// <summary>
    /// Array storing voxel types for the chunk.
    /// </summary>
    public VoxelType[] voxels;
    

    /// <summary>
    /// The height of the chunk.
    /// </summary>
    public int chunkHeight = 0;

    /// <summary>
    /// Reference to the world containing the chunk.
    /// </summary>
    public World worldReference;

    /// <summary>
    /// The world position of the chunk.
    /// </summary>
    public Vector3Int worldPosition;
    

    /// <summary>
    /// Data related to trees within the chunk.
    /// </summary>
    public TreeData treeData;

    /// <summary>
    /// Initializes a new instance of the ChunkData class with specified parameters.
    /// </summary>
    /// <param name="chunkSize">The size of the chunk along each axis.</param>
    /// <param name="chunkHeight">The height of the chunk.</param>
    /// <param name="world">Reference to the world containing the chunk.</param>
    /// <param name="worldPosition">The world position of the chunk.</param>
    public ChunkData(int chunkSize, int chunkHeight, World world, Vector3Int worldPosition)
    {
        this.chunkHeight = chunkHeight;
        this.chunkSize = chunkSize;
        this.worldReference = world;
        this.worldPosition = worldPosition;
        voxels = new VoxelType[chunkSize * chunkHeight * chunkSize];
    }
}
