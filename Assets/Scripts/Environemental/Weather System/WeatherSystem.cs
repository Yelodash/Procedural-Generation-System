using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Manages the weather system in the game.
/// </summary>
public class WeatherSystem : MonoBehaviour, IObserver
{
    /// <summary>
    /// Settings for the Rain.
    /// </summary>
    [Header("Rain Settings")]
    
    
    [Range(0f, 1f)] public float chanceToRainSpring = 0.3f;
    
    /// <summary>
    ///Chance of rain in the summer season.
    /// </summary>
    [Range(0f, 1f)] public float chanceToRainSummer = 0.0f;
    
    /// <summary>
    ///Chance of rain in the fall season.
    /// </summary>
    [Range(0f, 1f)] public float chanceToRainFall = 0.4f;
    
    /// <summary>
    ///Chance of rain in Winter.
    /// </summary>
    [Range(0f, 1f)] public float chanceToRainWinter = 0.7f;
    
    /// <summary>
    ///Reference to the rain effect game object.
    /// </summary>
    public GameObject rainEffect;
    
    /// <summary>
    ///Reference to the rain skybox material.
    /// </summary>
    public Material rainSkyBox;

    /// <summary>
    /// Reference to the audio source for the rain sound.
    /// </summary>
    public AudioSource rainChannel;
    
    /// <summary>
    /// Rain sound effect.
    /// </summary>
    public AudioClip rainSound;

    /// <summary>
    /// Indicates whether special weather is active.
    /// </summary>
    public bool isSpecialWeather;

    /// <summary>
    /// Enumeration of weather conditions.
    /// </summary>
    public enum WeatherCondition { Rainy }

    /// <summary>
    /// Current weather condition.
    /// </summary>
    public WeatherCondition CurrentWeather { get; private set; } = WeatherCondition.Rainy;

    /// <summary>
    /// Duration of each weather type in hours.
    /// </summary>
    private float weatherDuration = 5.0f;
    
    /// <summary>
    ///  Duration of the current weather.
    /// </summary>
    public  float currentWeatherTimer = 0.0f;
    
    /// <summary>
    ///Default skybox material.
    /// </summary>
    private Material defaultSkybox;

    /// <summary>
    /// Initializes the weather system.
    /// </summary>
    private void Start()
    {
        TimeManager.instance.AddObserver(this); // Subscribe to TimeMana events
        currentWeatherTimer = weatherDuration;
        defaultSkybox = RenderSettings.skybox;
    }

    /// <summary>
    /// Updates the weather system.
    /// </summary>
    private void Update()
    {
        if (isSpecialWeather)
        {
            currentWeatherTimer -= Time.deltaTime;
            if (currentWeatherTimer <= 0)
            {
                RenderSettings.skybox = defaultSkybox;
                isSpecialWeather = false;
                currentWeatherTimer = weatherDuration;
                DeactivateWeatherEffects();
            }
        }
    }

    /// <summary>
    /// Generates random weather based on the current season.
    /// </summary>
    private void GenerateRandomWeather()
    {
        TimeManager.Season currentSeason = TimeManager.instance.currentSeason;
        float chanceToRain = GetChanceToRain(currentSeason);

        float randomValue = Random.value;
        if (randomValue <= chanceToRain)
        {
            CurrentWeather = WeatherCondition.Rainy;
            isSpecialWeather = true;
            StartRain();
        }
        else
        {
            CurrentWeather = WeatherCondition.Rainy; // Set the current weather condition to Rainy in all cases
            isSpecialWeather = false;
            StopRain(); // Stop rain if it's ongoing
        }
    }

    /// <summary>
    /// Starts the rainy weather.
    /// </summary>
    private void StartRain()
    {
        if (!rainChannel.isPlaying)
        {
            rainChannel.clip = rainSound;
            rainChannel.loop = true;
            rainChannel.Play();
            rainEffect.SetActive(true);
        }
        RenderSettings.skybox = rainSkyBox;
        currentWeatherTimer = weatherDuration;
    }

    /// <summary>
    /// Stops the rainy weather.
    /// </summary>
    private void StopRain()
    {
        if (rainChannel.isPlaying)
        {
            rainChannel.Stop();
        }
        rainEffect.SetActive(false);
    }

    /// <summary>
    /// Deactivates all weather effects.
    /// </summary>
    private void DeactivateWeatherEffects()
    {
        StopRain();
    }

    /// <summary>
    /// Gets the chance of rain based on the current season.
    /// </summary>
    private float GetChanceToRain(TimeManager.Season season)
    {
        switch (season)
        {
            case TimeManager.Season.Spring: return chanceToRainSpring;
            case TimeManager.Season.Summer: return chanceToRainSummer;
            case TimeManager.Season.Fall: return chanceToRainFall;
            case TimeManager.Season.Winter: return chanceToRainWinter;
            default: return 0f;
        }
    }

    /// <summary>
    ///Handles the day pass event.
    /// </summary>
    public void OnDayPass()
    {
        GenerateRandomWeather();
    }
}
