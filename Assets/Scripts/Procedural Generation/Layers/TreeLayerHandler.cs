using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the generation of tree layers within chunk data.
/// </summary>
public class TreeLayerHandler : VoxelLayerHandler
{
    /// <summary>
    /// Maximum terrain height limit for generating trees.
    /// </summary>
    public float terrainHeightLimit = 25;

    /// <summary>
    /// Static layout of tree leaves.
    /// </summary>
    public static List<Vector3Int> treeLeafesStaticLayout = new List<Vector3Int>
    {
        // Coordinates for tree leaf positions relative to the tree trunk
        new Vector3Int(-1, 1, -1),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(-1, 1, 1),
        new Vector3Int(0, 1, -1),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, 1, 1),
        new Vector3Int(1, 1, -1),
        new Vector3Int(1, 1, 0),
        new Vector3Int(1, 1, 1),
        new Vector3Int(0, 2, 0)
    };

    /// <summary>
    /// Attempts to handle tree generation within the chunk data.
    /// </summary>
    /// <param name="chunkData">The chunk data to modify.</param>
    /// <param name="x">The X-coordinate within the chunk.</param>
    /// <param name="y">The Y-coordinate within the chunk.</param>
    /// <param name="z">The Z-coordinate within the chunk.</param>
    /// <param name="surfaceHeightNoise">The noise value representing surface height.</param>
    /// <param name="mapSeedOffset">The offset for the map seed.</param>
    /// <returns>True if handling is successful, otherwise false.</returns>
    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        // Skip handling if the chunk data's Y position is below ground level
        if (chunkData.worldPosition.y < 0)
            return false;

        // Check if the surface height is within the tree generation limit and if the current position is a tree location
        if (surfaceHeightNoise < terrainHeightLimit && chunkData.treeData.treePositions.Contains(new Vector2Int(chunkData.worldPosition.x + x, chunkData.worldPosition.z + z)))
        {
            Vector3Int chunkCoordinates = new Vector3Int(x, surfaceHeightNoise, z);
            VoxelType type = Chunk.ObtainVoxelFromChunkCoordinates(chunkData, chunkCoordinates);

            // Check if the current position is grass/dirt voxel type
            if (type == VoxelType.Grass_Dirt)
            {
                // Set the voxel below the tree trunk to dirt
                Chunk.SetVoxel(chunkData, chunkCoordinates, VoxelType.Dirt);

                // Place the tree trunk voxels
                for (int i = 1; i < 5; i++)
                {
                    chunkCoordinates.y = surfaceHeightNoise + i;
                    Chunk.SetVoxel(chunkData, chunkCoordinates, VoxelType.TreeTrunk);
                }

                // Calculate the position of leaves relative to the trunk and add them to tree data
                foreach (Vector3Int leafOffset in treeLeafesStaticLayout)
                {
                    Vector3Int leafPosition = chunkCoordinates + leafOffset;
                    chunkData.treeData.treeLeafesSolid.Add(leafPosition);
                }
            }
        }
        return false;
    }
}
