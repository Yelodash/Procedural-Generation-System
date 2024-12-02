using UnityEngine;
using Cinemachine;

/// <summary>
/// A Cinemachine extension for implementing a first-person POV (Point of View) camera control.
/// </summary>
public class CinemachinePOVExtension : CinemachineExtension
{
    [SerializeField] private float horizontalSpeed = 10f;
    [SerializeField] private float verticalSpeed = 10f;
    [SerializeField] private float clampAngle = 80f;

    private InputManager inputManager;
    private Vector3 startingRotation = Vector3.zero; // Initialise startingRotation à Vector3.zero

    /// <summary>
    /// Initializes the extension by obtaining the InputManager instance.
    /// </summary>
    protected override void Awake()
    {
        inputManager = InputManager.instance;
        base.Awake();
    }

    /// <summary>
    /// Overrides the PostPipelineStageCallback method to handle camera rotation.
    /// </summary>
    /// <param name="vcam">The Cinemachine virtual camera being manipulated.</param>
    /// <param name="stage">The current stage in the Cinemachine pipeline.</param>
    /// <param name="state">The camera state to be modified.</param>
    /// <param name="deltaTime">The time since the last update.</param>
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (vcam.Follow)
        {
            if (stage == CinemachineCore.Stage.Aim)
            {
                UpdateCameraRotation(ref state, deltaTime);
            }
        }
    }

    /// <summary>
    /// Updates the camera rotation based on user input.
    /// </summary>
    /// <param name="state">The camera state to be modified.</param>
    /// <param name="deltaTime">The time since the last update.</param>
    private void UpdateCameraRotation(ref CameraState state, float deltaTime)
    {
        // Vérifie si startingRotation a été initialisé
        if (startingRotation == Vector3.zero)
        {
            startingRotation = transform.localRotation.eulerAngles;
            Debug.Log("Starting rotation initialized: " + startingRotation);
        }

        Vector2 deltaInput = inputManager.GetMouseDelta();
        startingRotation.x += deltaInput.x * verticalSpeed * deltaTime;
        startingRotation.y += deltaInput.y * horizontalSpeed * deltaTime;
        startingRotation.y = Mathf.Clamp(startingRotation.y, -clampAngle, clampAngle);

        state.RawOrientation = Quaternion.Euler(-startingRotation.y, startingRotation.x, 0f);
    }
}
