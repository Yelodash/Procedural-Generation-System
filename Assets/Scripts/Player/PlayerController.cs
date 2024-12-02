using UnityEngine;
/// <summary>
///Manages the player character controller.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Character controller component for the player.
    /// </summary>
    private CharacterController controller;
    
    /// <summary>
    /// Current velocity of the player.
    /// </summary>
    private Vector3 playerVelocity;
    
    /// <summary>
    /// Flag to check if the player is grounded.
    /// </summary>
    private bool groundedPlayer;
    
    /// <summary>
    /// Reference to the input manager.
    /// </summary>
    private InputManager inputManager;
    
    /// <summary>
    /// Speed of the player character.
    /// </summary>
    [SerializeField] private float playerSpeed = 2.0f;
    
    /// <summary>
    /// Height of the player's jump.
    /// </summary>
    [SerializeField] private float jumpHeight = 1.0f;
    
    /// <summary>
    /// Value of gravity applied to the player.q
    /// </summary>
    [SerializeField] private float gravityValue = -9.81f;
    
    /// <summary>
    ///Transform of the camera to move the player relative to the camera's orientation.
    /// </summary>
    [SerializeField] private Transform cameraTransform;

    /// <summary>
    ///  wake is called when the script instance is being loaded.
    /// </summary>
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.instance;
        
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
            else
            {
                Debug.LogError("Main camera not found. Make sure a camera is tagged as 'MainCamera'.");
            }
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

   
    /// <summary>
    /// Updates the player's movement and jumping based on user input.
    /// </summary>
    private void Update()
    {
        // Check if the player is grounded and reset vertical velocity if falling
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Get horizontal movement input
        Vector2 movementInput = inputManager.GetPlayerMovement();
        Vector3 move = new Vector3(movementInput.x, 0f, movementInput.y);
        
        // Move in the direction relative to the camera's orientation
        if (cameraTransform != null)
        {
            move = cameraTransform.TransformDirection(move);
            move.y = 0f;
        }
        
        // Move the player horizontally
        controller.Move(move * Time.deltaTime * playerSpeed);

        // Jump if the player presses the jump button and is grounded
        if (inputManager.PlayerJumpedThisFrame() && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        // Apply gravity to the player
        playerVelocity.y += gravityValue * Time.deltaTime;
        
        // Move the player vertically
        controller.Move(playerVelocity * Time.deltaTime);
    }
}