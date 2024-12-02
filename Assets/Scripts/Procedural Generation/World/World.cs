using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents the world and handles world generation.
/// </summary>
public class World : MonoBehaviour
{
    /// <summary>
    /// Represents the instance of the World class.
    /// </summary>
    public static World Instance { get; private set; } // Singleton instance

    /// <summary>
    /// The size of the map in chunks.
    /// </summary>
    public int mapSizeInChunks = 0;
    
    /// <summary>
    /// The size of the chunk.
    /// </summary>
    public int chunkSize = 0, chunkHeight = 0;
    
    /// <summary>
    /// The range of chunks to draw.
    /// </summary>
    public int chunkDrawingRange = 0;

    /// <summary>
    /// The prefab for the chunk.
    /// </summary>
    public GameObject chunkPrefab;
    
    /// <summary>
    /// The world renderer.
    /// </summary>
    public WorldRenderer worldRenderer;

    /// <summary>
    /// The terrain generator.
    /// </summary>
    public TerrainGenerator terrainGenerator;
    
    /// <summary>
    /// The offset for the map seed.
    /// </summary>
    public Vector2Int mapSeedOffset;

    /// <summary>
    /// The token source for the task.
    /// </summary>
    private CancellationTokenSource taskTokenSource = new CancellationTokenSource();

    /// <summary>
    /// The event invoked when the world is created.
    /// </summary>
    public UnityEvent OnWorldCreated, OnNewChunksGenerated;

    /// <summary>
    /// The world data.
    /// </summary>
    public WorldData worldData { get; private set; }

    /// <summary>
    /// Indicates if the world has been created.
    /// </summary>
    public bool IsWorldCreated { get; private set; }

    /// <summary>
    ///  Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make sure the instance persists across scenes
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Ensures that there is only one instance of the class
            return;
        }

        // Instantiate WorldData with the appropriate constructor parameters
        worldData = new WorldData(chunkSize, chunkHeight);
    }

    /// <summary>
    /// Generates the world asynchronously.
    /// </summary>
    public async void GenerateWorld()
    {
        await GenerateWorld(Vector3Int.zero);
    }

    /// <summary>
    /// Generates the world asynchronously starting from the specified position.
    /// </summary>
    /// <param name="position">The position to start world generation from.</param>
    private async Task GenerateWorld(Vector3Int position)
    {
        
        terrainGenerator.GenerateBiomePoints(position, chunkDrawingRange, chunkSize, mapSeedOffset);

       
        WorldGenerationData worldGenerationData =
            await Task.Run(() => GetPositionsThatPlayerSees(position), taskTokenSource.Token);

        
        foreach (Vector3Int pos in worldGenerationData.chunkPositionsToRemove)
        {
            WorldDataHelper.RemoveChunk(this, pos);
        }

        foreach (Vector3Int pos in worldGenerationData.chunkDataToRemove)
        {
            WorldDataHelper.RemoveChunkData(this, pos);
        }

        ConcurrentDictionary<Vector3Int, ChunkData> dataDictionary = null;

        try
        {
            
            dataDictionary = await CalculateWorldChunkData(worldGenerationData.chunkDataPositionsToCreate);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Task canceled");
            return;
        }

        foreach (var calculatedData in dataDictionary)
        {
            worldData.chunkDataDictionary.TryAdd(calculatedData.Key, calculatedData.Value);
        }

        foreach (var chunkData in worldData.chunkDataDictionary.Values)
        {
            AddTreeLeafs(chunkData);
        }

        ConcurrentDictionary<Vector3Int, MeshData>
            meshDataDictionary = new ConcurrentDictionary<Vector3Int, MeshData>();

        
        List<ChunkData> dataToRender = worldData.chunkDataDictionary
            .Where(keyvaluepair => worldGenerationData.chunkPositionsToCreate.Contains(keyvaluepair.Key))
            .Select(keyvalpair => keyvalpair.Value)
            .ToList();

        try
        {
            meshDataDictionary = await CreateMeshDataAsync(dataToRender);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Task canceled");
            return;
        }

        
        StartCoroutine(ChunkCreationCoroutine(meshDataDictionary));
    }

    /// <summary>
    /// Adds tree leaf voxels to the chunk data.
    /// </summary>
    /// <param name="chunkData">The chunk data to add tree leafs to.</param>
    private void AddTreeLeafs(ChunkData chunkData)
    {
        foreach (var treeLeafes in chunkData.treeData.treeLeafesSolid)
        {
            Chunk.SetVoxel(chunkData, treeLeafes, VoxelType.TreeLeafsSolid);
        }
    }

    /// <summary>
    /// Asynchronously creates mesh data for rendering chunks.
    /// </summary>
    /// <param name="dataToRender">The list of chunk data to render.</param>
    /// <returns>A task representing the asynchronous operation with the dictionary of mesh data.</returns>
    private Task<ConcurrentDictionary<Vector3Int, MeshData>> CreateMeshDataAsync(List<ChunkData> dataToRender)
    {
        ConcurrentDictionary<Vector3Int, MeshData> dictionary = new ConcurrentDictionary<Vector3Int, MeshData>();
        return Task.Run(() =>
        {
            foreach (ChunkData data in dataToRender)
            {
                if (taskTokenSource.Token.IsCancellationRequested)
                {
                    taskTokenSource.Token.ThrowIfCancellationRequested();
                }

                MeshData meshData = Chunk.ObtainChunkMeshData(data);
                dictionary.TryAdd(data.worldPosition, meshData);
            }

            return dictionary;
        }, taskTokenSource.Token);
    }

    /// <summary>
    /// Calculates chunk data for the world asynchronously.
    /// </summary>
    /// <param name="chunkDataPositionsToCreate">The list of chunk data positions to create.</param>
    /// <returns>A task representing the asynchronous operation with the dictionary of chunk data.</returns>
    private Task<ConcurrentDictionary<Vector3Int, ChunkData>> CalculateWorldChunkData(
        List<Vector3Int> chunkDataPositionsToCreate)
    {
        ConcurrentDictionary<Vector3Int, ChunkData> dictionary = new ConcurrentDictionary<Vector3Int, ChunkData>();

        return Task.Run(() =>
            {
                foreach (Vector3Int pos in chunkDataPositionsToCreate)
                {
                    if (taskTokenSource.Token.IsCancellationRequested)
                    {
                        taskTokenSource.Token.ThrowIfCancellationRequested();
                    }

                    ChunkData data = new ChunkData(chunkSize, chunkHeight, this, pos);
                    ChunkData newData = terrainGenerator.GenerateChunkData(data, mapSeedOffset);
                    dictionary.TryAdd(pos, newData);
                }

                return dictionary;
            },
            taskTokenSource.Token);
    }

    /// <summary>
    /// Coroutine for creating chunks based on mesh data.
    /// </summary>
    /// <param name="meshDataDictionary">The dictionary containing mesh data for chunks.</param>
    /// <returns>An IEnumerator for the coroutine.</returns>
    private IEnumerator ChunkCreationCoroutine(ConcurrentDictionary<Vector3Int, MeshData> meshDataDictionary)
    {
        foreach (var item in meshDataDictionary)
        {
            if (worldData.chunkDataDictionary.ContainsKey(item.Key))
            {
                CreateChunk(worldData, item.Key, item.Value);
            }
            else
            {
                Debug.LogWarning("Chunk data not found for position: " + item.Key);
            }

            yield return new WaitForEndOfFrame();
        }

        if (IsWorldCreated == false)
        {
            IsWorldCreated = true;
            OnWorldCreated?.Invoke();
        }
    }

    /// <summary>
    /// Creates a chunk at the specified position with the given mesh data.
    /// </summary>
    /// <param name="worldData">The world data.</param>
    /// <param name="position">The position of the chunk.</param>
    /// <param name="meshData">The mesh data for the chunk.</param>
    private void CreateChunk(WorldData worldData, Vector3Int position, MeshData meshData)
    {
        if (worldData.chunkDictionary.ContainsKey(position))
        {
            Debug.LogWarning("Chunk already exists at position: " + position);
            return;
        }

        ChunkRenderer chunkRenderer = worldRenderer.RenderChunk(worldData, position, meshData);
        if (chunkRenderer == null)
        {
            Debug.LogError("Failed to create chunk at position: " + position);
            return;
        }

        worldData.chunkDictionary.Add(position, chunkRenderer);
    }

 
    /// <summary>
    /// Adjusts the voxel position based on the hit normal.
    /// </summary>
    /// <param name="pos">The position coordinate.</param>
    /// <param name="normal">The normal coordinate.</param>
    /// <returns>The adjusted position coordinate.</returns>
    private float GetVoxelPositionIn(float pos, float normal)
    {
        if (Mathf.Abs(pos % 1) == 0.5f)
        {
            pos -= (normal / 2);
        }

        return (float)pos;
    }

    /// <summary>
    /// Retrieves the necessary positions for world generation based on the player's position.
    /// </summary>
    /// <param name="playerPosition">The position of the player.</param>
    /// <returns>A WorldGenerationData object containing necessary positions for world generation.</returns>
    private WorldGenerationData GetPositionsThatPlayerSees(Vector3Int playerPosition)
    {
        List<Vector3Int> allChunkPositionsNeeded = WorldDataHelper.ObtainChunkPositionsAroundPlayer(this, playerPosition);
        List<Vector3Int> allChunkDataPositionsNeeded =
            WorldDataHelper.ObtainTheDataPositionsAroundPlayer(this, playerPosition);

        List<Vector3Int> chunkPositionsToCreate =
            WorldDataHelper.SelectPositionsToCreate(worldData, allChunkPositionsNeeded, playerPosition);
        List<Vector3Int> chunkDataPositionsToCreate =
            WorldDataHelper.SelectPositionsToCreate(worldData, allChunkDataPositionsNeeded, playerPosition);

        List<Vector3Int> chunkPositionsToRemove = WorldDataHelper.ObtainNotNecessaryChunks(worldData, allChunkPositionsNeeded);
        List<Vector3Int> chunkDataToRemove = WorldDataHelper.ObtainNotNecessaryChunks(worldData, allChunkDataPositionsNeeded);

        WorldGenerationData data = new WorldGenerationData
        {
            chunkPositionsToCreate = chunkPositionsToCreate,
            chunkDataPositionsToCreate = chunkDataPositionsToCreate,
            chunkPositionsToRemove = chunkPositionsToRemove,
            chunkDataToRemove = chunkDataToRemove,
            chunkPositionsToUpdate = new List<Vector3Int>()
        };
        return data;
    }

    /// <summary>
    /// Initiates the loading of additional chunks based on the player's position.
    /// </summary>
    /// <param name="player">The player GameObject.</param>
    internal async void LoadAdditionalChunksRequest(GameObject player)
    {
        Debug.Log("Load more chunks");
        await GenerateWorld(Vector3Int.RoundToInt(player.transform.position));
        OnNewChunksGenerated?.Invoke();
    }

    /// <summary>
    /// Retrieves the voxel type from chunk coordinates.
    /// </summary>
    /// <param name="chunkData">The chunk data.</param>
    /// <param name="x">The x-coordinate in chunk space.</param>
    /// <param name="y">The y-coordinate in chunk space.</param>
    /// <param name="z">The z-coordinate in chunk space.</param>
    /// <returns>The voxel type at the specified chunk coordinates.</returns>
    internal VoxelType ObtainVoxelFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        Vector3Int pos = Chunk.ChunkPositionFromVoxelCoordinates(this, x, y, z);
        ChunkData containerChunk = null;

        worldData.chunkDataDictionary.TryGetValue(pos, out containerChunk);

        if (containerChunk == null)
            return VoxelType.Nothing;
        Vector3Int voxelInChunkCoordinates = Chunk.ObtainVoxelInChunkCoordinates(containerChunk, new Vector3Int(x, y, z));
        return Chunk.ObtainVoxelFromChunkCoordinates(containerChunk, voxelInChunkCoordinates);
    }

    /// <summary>
    /// Disables the component and cancels any running tasks.
    /// </summary>
    public void OnDisable()
    {
        taskTokenSource.Cancel();
    }
}

