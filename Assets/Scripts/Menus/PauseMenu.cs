using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Added for Input System support
/// <summary>
/// Manages the pause menu functionality.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    /// <summary>
    /// The GameObject representing the pause menu UI.
    /// </summary>
    public GameObject pauseMenu;

    /// <summary>
    /// Indicates whether the game is currently paused.
    /// </summary>
    public static bool isPaused;

    /// <summary>
    /// Indicates whether the cursor was locked before entering the pause menu.
    /// </summary>
    private CursorLockMode previousLockMode;

    /// <summary>
    /// if pause menu is active, then the game is paused.
    /// </summary>
    void Start()
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        else
        {
            Debug.LogWarning("PauseMenu object is not assigned.");
        }
    }

    /// <summary>
    /// if keyboard escape key is pressed, then pause or resume the game.
    /// </summary>
    void Update()
    {
        // Checking for escape key press using Input System
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("Escape key pressed.");
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    /// <summary>
    /// Pauses the game and activates the pause menu.
    /// </summary>
    public void PauseGame()
    {
        if (pauseMenu != null)
        {
            Debug.Log("Game Paused");
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;

            // Save the previous lock mode and unlock the cursor
            previousLockMode = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Debug.LogWarning("PauseMenu object is not assigned.");
        }
    }

    /// <summary>
    /// Resumes the game and deactivates the pause menu.
    /// </summary>
    public void ResumeGame()
    {
        if (pauseMenu != null)
        {
            Debug.Log("Game Resumed");
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;

            // Restore the previous lock mode
            Cursor.lockState = previousLockMode;
            Cursor.visible = false;
        }
        else
        {
            Debug.LogWarning("PauseMenu object is not assigned.");
        }
    }

   
    /// <summary>
    /// Quits the game.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }

    /// <summary>
    /// Logs button click events for debugging.
    /// </summary>
    /// <param name="buttonName">The name of the clicked button.</param>
    public void OnButtonClicked(string buttonName)
    {
        Debug.Log("Button clicked: " + buttonName);
    }
}
