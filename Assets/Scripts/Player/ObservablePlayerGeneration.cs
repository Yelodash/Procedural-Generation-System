using UnityEngine;
using System;
/// <summary>
/// Observes the player generation and notifies subscribers when a player is generated.
/// </summary>
public class ObservablePlayerGeneration : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the PlayerGenerationObservable.
    /// </summary>
    public static ObservablePlayerGeneration Instance;

    /// <summary>
    /// Event triggered when a player is generated.
    /// </summary>
    public event Action<GameObject> OnPlayerGenerated;

    /// <summary>
    /// Initializes the singleton instance.
    /// </summary>
    private void Awake()
    {
      
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Invokes the player generated event.
    /// </summary>
    /// <param name="player">The generated player GameObject.</param>
    public void InvokePlayerGenerated(GameObject player)
    {
        // Trigger the player generated event
        OnPlayerGenerated?.Invoke(player);
    }
}
