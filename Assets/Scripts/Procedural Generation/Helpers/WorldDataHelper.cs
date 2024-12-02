using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Helper class for the world data.
/// </summary>
public static class WorldDataHelper
{
    /// <summary>
    ///  Get the chunk position from the world voxel position.
    /// </summary>
    /// <param name="world"></param>
    /// <param name="worldVoxelPosition"></param>
    /// <returns></returns>
    public static Vector3Int ChunkPositionFromVoxelCoordinates(World world, Vector3Int worlVoxelPosition)
    {
        return new Vector3Int
        {
            x = Mathf.FloorToInt(worlVoxelPosition.x / (float)world.chunkSize) * world.chunkSize,
            y = Mathf.FloorToInt(worlVoxelPosition.y / (float)world.chunkHeight) * world.chunkHeight,
            z = Mathf.FloorToInt(worlVoxelPosition.z / (float)world.chunkSize) * world.chunkSize
        };
    }

    /// <summary>
    ///  Get the chunk positions around the player.
    /// </summary>
    /// <param name="world"></param>
    /// <param name="playerPosition"></param>
    /// <returns></returns>
    internal static List<Vector3Int> ObtainChunkPositionsAroundPlayer(World world, Vector3Int playerPosition)
    {
        int startX = playerPosition.x - (world.chunkDrawingRange) * world.chunkSize;
        int startZ = playerPosition.z - (world.chunkDrawingRange) * world.chunkSize;
        int endX = playerPosition.x + (world.chunkDrawingRange) * world.chunkSize;
        int endZ = playerPosition.z + (world.chunkDrawingRange) * world.chunkSize;

        List<Vector3Int> chunkPositionsToCreate = new List<Vector3Int>();
        for (int x = startX; x <= endX; x += world.chunkSize)
        {
            for (int z = startZ; z <= endZ; z += world.chunkSize)
            {
                Vector3Int chunkPos = ChunkPositionFromVoxelCoordinates(world, new Vector3Int(x, 0, z));
                chunkPositionsToCreate.Add(chunkPos);
                if (x >= playerPosition.x - world.chunkSize
                    && x <= playerPosition.x + world.chunkSize
                    && z >= playerPosition.z - world.chunkSize
                    && z <= playerPosition.z + world.chunkSize)
                {
                    for (int y = -world.chunkHeight;
                         y >= playerPosition.y - world.chunkHeight * 2;
                         y -= world.chunkHeight)
                    {
                        chunkPos = ChunkPositionFromVoxelCoordinates(world, new Vector3Int(x, y, z));
                        chunkPositionsToCreate.Add(chunkPos);
                    }
                }
            }
        }

        return chunkPositionsToCreate;
    }

    /// <summary>
    ///  Remove the chunk data from the world.
    /// </summary>
    /// <param name="world"></param>
    /// <param name="pos"></param>
    internal static void RemoveChunkData(World world, Vector3Int pos)
    {
        world.worldData.chunkDataDictionary.Remove(pos);
    }

    /// <summary>
    ///  Remove the chunk 
    /// </summary>
    /// <param name="world"></param>
    /// <param name="pos"></param>
    internal static void RemoveChunk(World world, Vector3Int pos)
    {
        ChunkRenderer chunk = null;
        if (world.worldData.chunkDictionary.TryGetValue(pos, out chunk))
        {
            world.worldRenderer.RemoveChunk(chunk);
            world.worldData.chunkDictionary.Remove(pos);
        }
    }

    /// <summary>
    ///  Get the data positions around the player.
    /// </summary>
    /// <param name="world"></param>
    /// <param name="playerPosition"></param>
    /// <returns></returns>
    internal static List<Vector3Int> ObtainTheDataPositionsAroundPlayer(World world, Vector3Int playerPosition)
    {
        int startX = playerPosition.x - (world.chunkDrawingRange + 1) * world.chunkSize;
        int startZ = playerPosition.z - (world.chunkDrawingRange + 1) * world.chunkSize;
        int endX = playerPosition.x + (world.chunkDrawingRange + 1) * world.chunkSize;
        int endZ = playerPosition.z + (world.chunkDrawingRange + 1) * world.chunkSize;

        List<Vector3Int> chunkDataPositionsToCreate = new List<Vector3Int>();
        for (int x = startX; x <= endX; x += world.chunkSize)
        {
            for (int z = startZ; z <= endZ; z += world.chunkSize)
            {
                Vector3Int chunkPos = ChunkPositionFromVoxelCoordinates(world, new Vector3Int(x, 0, z));
                chunkDataPositionsToCreate.Add(chunkPos);
                if (x >= playerPosition.x - world.chunkSize
                    && x <= playerPosition.x + world.chunkSize
                    && z >= playerPosition.z - world.chunkSize
                    && z <= playerPosition.z + world.chunkSize)
                {
                    for (int y = -world.chunkHeight;
                         y >= playerPosition.y - world.chunkHeight * 2;
                         y -= world.chunkHeight)
                    {
                        chunkPos = ChunkPositionFromVoxelCoordinates(world, new Vector3Int(x, y, z));
                        chunkDataPositionsToCreate.Add(chunkPos);
                    }
                }
            }
        }

        return chunkDataPositionsToCreate;
    }

    /// <summary>
    ///  Get the chunk from the world.
    /// </summary>
    /// <param name="worldReference"></param>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    internal static ChunkRenderer ObtainChunk(World worldReference, Vector3Int worldPosition)
    {
        if (worldReference.worldData.chunkDictionary.ContainsKey(worldPosition))
            return worldReference.worldData.chunkDictionary[worldPosition];
        return null;
    }

    /// <summary>
    /// Sets the voxel type at the specified world voxel position.
    /// </summary>
    /// <param name="worldReference">Reference to the world.</param>
    /// <param name="worldVoxelsPosition">Position of the voxel in world coordinates.</param>
    /// <param name="voxelType">Type of the voxel to set.</param>
    internal static void SetVoxel(World worldReference, Vector3Int worldVoxelPosition, VoxelType voxelType)
    {
        ChunkData chunkData = GetChunkData(worldReference, worldVoxelPosition);
        if (chunkData != null)
        {
            Vector3Int localPosition = Chunk.ObtainVoxelInChunkCoordinates(chunkData, worldVoxelPosition);
            Chunk.SetVoxel(chunkData, localPosition, voxelType);
        }
    }

    /// <summary>
    /// Retrieves the chunk data corresponding to the specified world voxel position.
    /// </summary>
    /// <param name="worldReference">Reference to the world.</param>
    /// <param name="worldVoxelPosition">Position of the voxel in world coordinates.</param>
    /// <returns>The chunk data containing the specified voxel, or null if the chunk is not loaded.</returns>
    public static ChunkData GetChunkData(World worldReference, Vector3Int worldVoxelPosition)
    {
        // Calculate the chunk position based on the world voxel position
        Vector3Int chunkPosition = ChunkPositionFromVoxelCoordinates(worldReference, worldVoxelPosition);

        // Retrieve the chunk data from the world data dictionary
        ChunkData containerChunk = null;
        worldReference.worldData.chunkDataDictionary.TryGetValue(chunkPosition, out containerChunk);

        // Return the chunk data containing the specified voxel
        return containerChunk;
    }

    /// <summary>
    /// Retrieves the list of unneeded chunk data positions based on the provided list of needed positions.
    /// </summary>
    /// <param name="worldData">The world data containing the chunk data.</param>
    /// <param name="neededPositions">The list of needed chunk data positions.</param>
    /// <returns>The list of unneeded chunk data positions.</returns>
    public static List<Vector3Int> ObtainNotNecessaryData(WorldData worldData, List<Vector3Int> neededPositions)
    {
        return worldData.chunkDataDictionary.Keys.Except(neededPositions).ToList();
    }

    /// <summary>
    /// Selects the positions from the list to create.
    /// </summary>
    /// <param name="worldData"></param>
    /// <param name="allPositions"></param>
    /// <param name="playerPosition"></param>
    /// <returns></returns>
    public static List<Vector3Int> SelectDataPositionsToCreate(WorldData worldData,
        List<Vector3Int> allPositions, Vector3Int playerPosition)
    {
        List<Vector3Int> positionsToCreate = new List<Vector3Int>();

        foreach (Vector3Int pos in allPositions)
        {
            if (!worldData.chunkDataDictionary.ContainsKey(pos) &&
                Mathf.Abs(playerPosition.y - pos.y) <= worldData.chunkHeight * 2)
            {
                positionsToCreate.Add(pos);
            }
        }

        return positionsToCreate;
    }
    
    /// <summary>
    /// Selects the positions from the list to create.
    /// </summary>
    /// <param name="worldData"></param>
    /// <param name="allPositions"></param>
    /// <param name="playerPosition"></param>
    /// <returns></returns>
    public static List<Vector3Int> SelectPositionsToCreate(WorldData worldData,
        List<Vector3Int> allPositions, Vector3Int playerPosition)
    {
        List<Vector3Int> positionsToCreate = new List<Vector3Int>();

        foreach (Vector3Int pos in allPositions)
        {
            if (!worldData.chunkDictionary.ContainsKey(pos) &&
                Mathf.Abs(playerPosition.y - pos.y) <= worldData.chunkHeight * 2)
            {
                positionsToCreate.Add(pos);
            }
        }

        return positionsToCreate;
    }

    /// <summary>
    ///  Get the unneeded chunks.
    /// </summary>
    /// <param name="worldData"></param>
    /// <param name="allChunkPositionsNeeded"></param>
    /// <returns></returns>
    public static List<Vector3Int> ObtainNotNecessaryChunks(WorldData worldData, List<Vector3Int> allChunkPositionsNeeded)
    {
        return worldData.chunkDictionary.Keys.Except(allChunkPositionsNeeded).ToList();
    }
}
