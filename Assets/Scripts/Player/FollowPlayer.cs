using UnityEngine;

/// <summary>
/// Follows the player and updates its position.
/// </summary>
public class FollowPlayer : MonoBehaviour
{
    /// <summary>
    /// The transform of the player to follow.
    /// </summary>
    public Transform player;

    /// <summary>
    /// The offset between this object and the player.
    /// </summary>
    public Vector3 offset;

    /// <summary>
    /// Reference to the game manager.
    /// </summary>
    public GameManager gameManager;

    private void Start()
    {
        // Subscribe to the player generation event
        ObservablePlayerGeneration.Instance.OnPlayerGenerated += OnPlayerGenerated;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the player generation event
        ObservablePlayerGeneration.Instance.OnPlayerGenerated -= OnPlayerGenerated;
    }

    /// <summary>
    /// Called when the player is generated.
    /// </summary>
    /// <param name="player">The generated player object.</param>
    private void OnPlayerGenerated(GameObject player)
    {
        // Update the player reference when generated
        this.player = player.transform;
    }

    private void LateUpdate()
    {
        if (player == null)
        {
            // Log a warning if the player transform is not set
            Debug.LogWarning("Player transform not set for FollowPlayer script.");
            return;
        }

        // Calculate the target position based on the player's position and offset
        Vector3 targetPosition = player.position + offset;
        transform.position = targetPosition;
    }
}
