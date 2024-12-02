using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
///Manages the input for the player.
/// </summary>
public class InputManager : MonoBehaviour
{
    /// <summary>
    ///Singleton instance of the InputManager.
    /// </summary>
    private static InputManager _instance;
    
    /// <summary>
    /// Gets the singleton instance of the InputManager.
    /// </summary>
    public static InputManager instance => _instance;

    /// <summary>
    /// Reference to the player controls.
    /// </summary>
    public PlayerControls playerControls;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
        playerControls = new PlayerControls();
        Cursor.visible = false;
    }

    /// <summary>
    /// This function is called when the object is enabled + active.
    /// </summary>
    private void OnEnable()
    {
        playerControls.Enable();
    }
    
    
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        if (gameObject != null)
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }

    /// <summary>
    /// Gets the player movement input.
    /// </summary>
    /// <returns></returns>
    public Vector2 GetPlayerMovement()
    {
        return playerControls.Player.Movement.ReadValue<Vector2>();
    }
    
    /// <summary>
    /// Gets the player jump input.
    /// </summary>
    /// <returns></returns>
    public bool PlayerJumpedThisFrame()
    {
        return playerControls.Player.Jump.triggered;
    }
    
    /// <summary>
    /// Gets the player crouch input.
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMouseDelta()
    {
        return playerControls.Player.Look.ReadValue<Vector2>();
    }
}
