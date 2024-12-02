using UnityEngine;
using System.Collections;
using Cinemachine;
/// <summary>
/// Manages the game  and player interactions.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Prefab for the player character.
    /// </summary>
    public GameObject playerPrefab;
    
    /// <summary>
    /// Reference to the spawned player object.
    /// </summary>
    private GameObject player;

    /// <summary>
    /// Reference to the world manager.
    /// </summary>
    public World world;

    /// <summary>
    /// Time interval for detecting player movement.
    /// </summary>
    public float detectionTime = 1;

    /// <summary>
    /// Reference to the Cinemachine virtual camera.
    /// </summary>
    public CinemachineVirtualCamera camera_VM;

    /// <summary>
    /// Reference to the player generation observable.
    /// </summary>
    public ObservablePlayerGeneration playerGenerationObservable;

    /// <summary>
    /// Current position of the player chunk.
    /// </summary>
    public Vector3Int currentPlayerChunkPosition;

    /// <summary>
    /// Center position of the current chunk.
    /// </summary>
    private Vector3Int currentChunkCenter = Vector3Int.zero;

    /// <summary>
    /// Spawns the player character in the world.
    /// </summary>
    public void SpawnPlayer()
    {
        if (player != null)
            return;

        Vector3Int raycastStartPosition = new Vector3Int(world.chunkSize / 2, 100, world.chunkSize / 2);
        RaycastHit hit;

        if (Physics.Raycast(raycastStartPosition, Vector3.down, out hit, 120))
        {
            player = Instantiate(playerPrefab, hit.point + Vector3Int.up, Quaternion.identity);
            camera_VM.Follow = player.transform.GetChild(0);
            StartCheckingTheMap();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            playerGenerationObservable.InvokePlayerGenerated(player); // Notify player generation event
        }
    }

    /// <summary>
    /// Starts checking if the player has moved to a new chunk.
    /// </summary>
    public void StartCheckingTheMap()
    {
        SetCurrentChunkCoordinates();
        StopAllCoroutines();
        StartCoroutine(CheckIfShouldLoadNextPosition());
    }

    /// <summary>
    /// Coroutine to check if the player has moved to a new chunk.
    /// </summary>
    IEnumerator CheckIfShouldLoadNextPosition()
    {
        yield return new WaitForSeconds(detectionTime);

        if (
            Mathf.Abs(currentChunkCenter.x - player.transform.position.x) > world.chunkSize ||
            Mathf.Abs(currentChunkCenter.z - player.transform.position.z) > world.chunkSize ||
            (Mathf.Abs(currentPlayerChunkPosition.y - player.transform.position.y) > world.chunkHeight)
            )
        {
            world.LoadAdditionalChunksRequest(player);
        }
        else
        {
            StartCoroutine(CheckIfShouldLoadNextPosition());
        }
    }

    /// <summary>
    /// Sets the current chunk coordinates based on player position.
    /// </summary>
    private void SetCurrentChunkCoordinates()
    {
        currentPlayerChunkPosition = WorldDataHelper.ChunkPositionFromVoxelCoordinates(world, Vector3Int.RoundToInt(player.transform.position));
        currentChunkCenter.x = currentPlayerChunkPosition.x + world.chunkSize / 2;
        currentChunkCenter.z = currentPlayerChunkPosition.z + world.chunkSize / 2;
    }
}
