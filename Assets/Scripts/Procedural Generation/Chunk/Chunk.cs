using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The Chunk class which works the with chunks.
/// </summary>
public static class Chunk
{
    /// <summary>
    /// Loops through all the Voxels in the chunk and performs the specified action on each voxel.
    /// </summary>
    /// <param name="chunkData">The chunk data containing the voxels.</param>t
    /// <param name="actionToPerform">The action to perform on each voxel, taking x, y, and z coordinates as parameters.</param>
    public static void LoopThroughTheVoxels(ChunkData chunkData, Action<int, int, int> actionToPerform)
    {
        for (int index = 0; index < chunkData.voxels.Length; index++)
        {
            var position = ObtainVoxelPosititionFromIndex(chunkData, index);
            actionToPerform(position.x, position.y, position.z);
        }
    }


    /// <summary>
    /// Gets the voxel position from the index.
    /// </summary>
    /// <param name="chunkData"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private static Vector3Int ObtainVoxelPosititionFromIndex(ChunkData chunkData, int index)
    {
        int x = index % chunkData.chunkSize;
        int y = (index / chunkData.chunkSize) % chunkData.chunkHeight;
        int z = index / (chunkData.chunkSize * chunkData.chunkHeight);
        return new Vector3Int(x, y, z);
    }

    
    /// <summary>
    /// Checks if the given coordinates are within the range of the chunk.
    /// </summary>
    /// <param name="chunkData"></param>
    /// <param name="axisCoordinate"></param>
    /// <returns></returns>
    private static bool InRange(ChunkData chunkData, int axisCoordinate)
    {
        if (axisCoordinate < 0 || axisCoordinate >= chunkData.chunkSize)
            return false;

        return true;
    }

    /// <summary>
    /// Checks if the given y coordinate is within the height of the chunk.
    /// </summary>
    /// <param name="chunkData"></param>
    /// <param name="ycoordinate"></param>
    /// <returns></returns>
    private static bool InRangeHeight(ChunkData chunkData, int ycoordinate)
    {
        if (ycoordinate < 0 || ycoordinate >= chunkData.chunkHeight)
            return false;

        return true;
    }

    /// <summary>
    /// Gets the voxel type at the specified chunk coordinates.
    /// </summary>
    /// <param name="chunkData">The chunk data.</param>
    /// <param name="chunkCoordinates">The chunk coordinates of the voxel.</param>
    /// <returns>The voxel type at the specified chunk coordinates.</returns>
    public static VoxelType ObtainVoxelFromChunkCoordinates(ChunkData chunkData, Vector3Int chunkCoordinates)
    {
        return ObtainVoxelFromChunkCoordinates(chunkData, chunkCoordinates.x, chunkCoordinates.y, chunkCoordinates.z);
    }

    /// <summary>
    /// Obtain the voxel type at the specified chunk coordinates.
    /// </summary>
    /// <param name="chunkData">The chunk data.</param>
    /// <param name="x">The x-coordinate in chunk space.</param>
    /// <param name="y">The y-coordinate in chunk space.</param>
    /// <param name="z">The z-coordinate in chunk space.</param>
    /// <returns>The voxel type at the specified chunk coordinates.</returns>
    public static VoxelType ObtainVoxelFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        if (InRange(chunkData, x) && InRangeHeight(chunkData, y) && InRange(chunkData, z))
        {
            int index = ObtainIndexFromPosition(chunkData, x, y, z);
            return chunkData.voxels[index];
        }

        return chunkData.worldReference.ObtainVoxelFromChunkCoordinates(chunkData, chunkData.worldPosition.x + x,
            chunkData.worldPosition.y + y, chunkData.worldPosition.z + z);
    }


    /// <summary>
    /// Sets the voxel at the specified local position within the chunk.
    /// </summary>
    /// <param name="chunkData">The chunk data.</param>
    /// <param name="localPosition">The local position within the chunk.</param>
    /// <param name="voxel">The voxel type to set.</param>
    public static void SetVoxel(ChunkData chunkData, Vector3Int localPosition, VoxelType voxel)
    {
        if (InRange(chunkData, localPosition.x) && InRangeHeight(chunkData, localPosition.y) &&
            InRange(chunkData, localPosition.z))
        {
            int index = ObtainIndexFromPosition(chunkData, localPosition.x, localPosition.y, localPosition.z);
            chunkData.voxels[index] = voxel;
        }
        else
        {
            WorldDataHelper.SetVoxel(chunkData.worldReference, localPosition + chunkData.worldPosition, voxel);
        }
    }

    /// <summary>
    /// Calculates the index of a voxel in the chunk data array based on its position in chunk space.
    /// </summary>
    /// <param name="chunkData">The chunk data.</param>
    /// <param name="x">The x-coordinate in chunk space.</param>
    /// <param name="y">The y-coordinate in chunk space.</param>
    /// <param name="z">The z-coordinate in chunk space.</param>
    /// <returns>The index of the voxel in the chunk data array.</returns>
    private static int ObtainIndexFromPosition(ChunkData chunkData, int x, int y, int z)
    {
        return x + chunkData.chunkSize * y + chunkData.chunkSize * chunkData.chunkHeight * z;
    }

    /// <summary>
    /// Converts a position from world space to chunk space.
    /// </summary>
    /// <param name="chunkData">The chunk data.</param>
    /// <param name="pos">The position in world space.</param>
    /// <returns>The position in chunk space.</returns>
    public static Vector3Int ObtainVoxelInChunkCoordinates(ChunkData chunkData, Vector3Int pos)
    {
        return new Vector3Int
        {
            x = pos.x - chunkData.worldPosition.x,
            y = pos.y - chunkData.worldPosition.y,
            z = pos.z - chunkData.worldPosition.z
        };
    }

    /// <summary>
    /// Generates mesh data for the entire chunk.
    /// </summary>
    /// <param name="chunkData">The chunk data.</param>
    /// <returns>The generated mesh data.</returns>
    public static MeshData ObtainChunkMeshData(ChunkData chunkData)
    {
        MeshData meshData = new MeshData(true);

        LoopThroughTheVoxels(chunkData,
            (x, y, z) => meshData = VoxelHelper.GetMeshData(chunkData, x, y, z, meshData,
                chunkData.voxels[ObtainIndexFromPosition(chunkData, x, y, z)]));

        return meshData;
    }

    /// <summary>
    /// Converts voxel coordinates to chunk coordinates.
    /// </summary>
    /// <param name="world">The world reference.</param>
    /// <param name="x">The x-coordinate in voxel space.</param>
    /// <param name="y">The y-coordinate in voxel space.</param>
    /// <param name="z">The z-coordinate in voxel space.</param>
    /// <returns>The corresponding chunk coordinates.</returns>
    internal static Vector3Int ChunkPositionFromVoxelCoordinates(World world, int x, int y, int z)
    {
        Vector3Int pos = new Vector3Int
        {
            x = Mathf.FloorToInt(x / (float)world.chunkSize) * world.chunkSize,
            y = Mathf.FloorToInt(y / (float)world.chunkHeight) * world.chunkHeight,
            z = Mathf.FloorToInt(z / (float)world.chunkSize) * world.chunkSize
        };
        return pos;
    }

    /// <summary>
    /// Gets the edge neighboring chunks of the specified chunk.
    /// </summary>
    /// <param name="chunkData">The chunk data.</param>
    /// <param name="worldPosition">The world position.</param>
    /// <returns>The list of edge neighboring chunks.</returns>
    internal static List<ChunkData> ObtainEdgeNeighbourChunk(ChunkData chunkData, Vector3Int worldPosition)
    {
        Vector3Int chunkPosition = ObtainVoxelInChunkCoordinates(chunkData, worldPosition);
        List<ChunkData> neighboursToUpdate = new List<ChunkData>();
        if (chunkPosition.x == 0)
        {
            neighboursToUpdate.Add(WorldDataHelper.GetChunkData(chunkData.worldReference,
                worldPosition - Vector3Int.right));
        }

        if (chunkPosition.x == chunkData.chunkSize - 1)
        {
            neighboursToUpdate.Add(WorldDataHelper.GetChunkData(chunkData.worldReference,
                worldPosition + Vector3Int.right));
        }

        if (chunkPosition.y == 0)
        {
            neighboursToUpdate.Add(
                WorldDataHelper.GetChunkData(chunkData.worldReference, worldPosition - Vector3Int.up));
        }

        if (chunkPosition.y == chunkData.chunkHeight - 1)
        {
            neighboursToUpdate.Add(
                WorldDataHelper.GetChunkData(chunkData.worldReference, worldPosition + Vector3Int.up));
        }

        if (chunkPosition.z == 0)
        {
            neighboursToUpdate.Add(WorldDataHelper.GetChunkData(chunkData.worldReference,
                worldPosition - Vector3Int.forward));
        }

        if (chunkPosition.z == chunkData.chunkSize - 1)
        {
            neighboursToUpdate.Add(WorldDataHelper.GetChunkData(chunkData.worldReference,
                worldPosition + Vector3Int.forward));
        }

        return neighboursToUpdate;
    }

    /// <summary>
    /// Checks if the specified world position is on the edge of the chunk.
    /// </summary>
    /// <param name="chunkData">The chunk data.</param>
    /// <param name="worldPosition">The world position.</param>
    /// <returns>True if the position is on the edge of the chunk, otherwise false.</returns>
    internal static bool IsItOnEdge(ChunkData chunkData, Vector3Int worldPosition)
    {
        Vector3Int chunkPosition = ObtainVoxelInChunkCoordinates(chunkData, worldPosition);
        return chunkPosition.x == 0 || chunkPosition.x == chunkData.chunkSize - 1 ||
               chunkPosition.y == 0 || chunkPosition.y == chunkData.chunkHeight - 1 ||
               chunkPosition.z == 0 || chunkPosition.z == chunkData.chunkSize - 1;
    }

}
