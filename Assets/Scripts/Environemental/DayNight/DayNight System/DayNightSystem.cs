using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// Manages the day and night cycle in the game.
/// </summary>
public class DayNightSystem : MonoBehaviour
{
    
    private static DayNightSystem instance;

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static DayNightSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DayNightSystem>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("DayNightSystemSingleton");
                    instance = singletonObject.AddComponent<DayNightSystem>();
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// Reference to the directional light in the scene.
    /// </summary>
    public Light directionalLight;

    /// <summary>
    /// Duration of a day in seconds.
    /// </summary>
    public float dayDurationInSeconds = 24.0f;

    /// <summary>
    /// Current hour of the day.
    /// </summary>
    public int CurrentHour { get; private set; }

    /// <summary>
    /// Current time of day represented as a normalized value between 0 and 1.
    /// </summary>
    private float currentTimeOfDay = 0.40f;

    /// <summary>
    /// List of mappings between hours and corresponding skybox materials.
    /// </summary>
    public List<SkyboxTimeMapping> timeMappings;

    /// <summary>
    /// Blended value for transitioning between skybox materials.
    /// </summary>
    private float blendedValue = 0.0f;

    /// <summary>
    /// Current skybox material.
    /// </summary>
    private Material currentSkybox;

    /// <summary>
    /// Indicates whether the trigger for the next day is locked.
    /// </summary>
    private bool lockNextDayTrigger = false;

    /// <summary>
    /// Reference to the UI element displaying the current time.
    /// </summary>
    public TextMeshProUGUI timeUI;

    /// <summary>
    /// Reference to the weather system.
    /// </summary>
    public WeatherSystem weatherSystem;

    /// <summary>
    /// Initializes the reference to the weather system.
    /// </summary>
    private void Start()
    {
        weatherSystem = FindObjectOfType<WeatherSystem>();
    }

    /// <summary>
    /// Updates the day and night cycle.
    /// </summary>
    private void Update()
    {
        currentTimeOfDay += Time.deltaTime / dayDurationInSeconds;
        currentTimeOfDay = currentTimeOfDay % 1;

        CurrentHour = Mathf.FloorToInt(currentTimeOfDay * 24);

        UpdateTimeUI();

        UpdateDirectionalLightRotation();

        // Check if special weather is not active before updating skybox
        if (!weatherSystem.isSpecialWeather)
        {
            UpdateSkybox();
        }

        // Trigger next day when hour is 0
        HandleNextDayTrigger();
    }

    /// <summary>
    /// Updates the UI element displaying the current time.
    /// </summary>
    private void UpdateTimeUI()
    {
        timeUI.text = $"{CurrentHour}:00";
    }

    /// <summary>
    /// Updates the rotation of the directional light based on the current time of day.
    /// </summary>
    /// <summary>
    /// Updates the rotation and intensity of the directional light based on the current time of day.
    /// </summary>
    private void UpdateDirectionalLightRotation()
    {
        // Calculate the light intensity based on the current time of day
        float intensity = Mathf.Lerp(0.2f, 1f, Mathf.Clamp01(currentTimeOfDay * 2)); // Assuming night starts at 0.5 (0.5 * 2 = 1)

        // Update the light rotation
        directionalLight.transform.rotation = Quaternion.Euler(new Vector3((currentTimeOfDay * 360) - 90, 170, 0));

        // Set the light intensity
        directionalLight.intensity = intensity;
    }

    /// <summary>
    /// Handles the trigger for the next day.
    /// </summary>
    private void HandleNextDayTrigger()
    {
        if (CurrentHour == 0 && !lockNextDayTrigger)
        {
            lockNextDayTrigger = true;
            Debug.Log("Triggering next day...");
            TimeManager.instance.TriggerNextDay();
        }

        if (CurrentHour != 0)
        {
            lockNextDayTrigger = false;
        }
    }

    /// <summary>
    /// Updates the skybox based on the current hour.
    /// </summary>
    private void UpdateSkybox()
    {
        foreach (SkyboxTimeMapping mapping in timeMappings)
        {
            if (CurrentHour == mapping.hour)
            {
                currentSkybox = mapping.skyboxMaterial;

                if (currentSkybox.shader.name == "Custom/SkyboxTransition")
                {
                    blendedValue += Time.deltaTime;
                    blendedValue = Mathf.Clamp01(blendedValue);
                    currentSkybox.SetFloat("_TransitionFactor", blendedValue);
                }
                else
                {
                    blendedValue = 0.0f;
                }

                break;
            }
        }

        if (currentSkybox != null)
        {
            RenderSettings.skybox = currentSkybox;
        }
    }
}

/// <summary>
/// Represents a mapping between hour of the day and skybox material.
/// </summary>
[System.Serializable]
public class SkyboxTimeMapping
{
    /// <summary>
    /// Name of the phase (optional).
    /// </summary>
    public string phaseName;

    /// <summary>
    /// Hour of the day.
    /// </summary>
    public int hour;

    /// <summary>
    /// Skybox material corresponding to the hour.
    /// </summary>
    public Material skyboxMaterial;
}
