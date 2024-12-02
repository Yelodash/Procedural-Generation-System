using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
/// <summary>
///  Interface for classes that observe the TimeManager class.
/// </summary>
public interface IObserver
{
    void OnDayPass();
}

/// <summary>
/// Manages the time in the game.
/// </summary>
public class TimeManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the TimeManager.
    /// </summary>
    public static TimeManager instance { get; private set; }

    /// <summary>
    ///Event that is triggered when a day passes.
    /// </summary>
    public UnityEvent OnDayPass = new UnityEvent();

   /// <summary>
   /// Enumeration of the seasons in the game.
   /// </summary>
    public enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter
    }

   /// <summary>
   /// Enumeration of the days in the week.
   /// </summary>
    public enum DayOfWeek
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    /// <summary>
    ///Current day of the week.
    /// </summary>
    public DayOfWeek currentDay = DayOfWeek.Monday;
    
    /// <summary>
    ///Current season in the game. 
    /// </summary>
    public Season currentSeason = Season.Spring;

    /// <summary>
    ///Number of days in each season.
    /// </summary>
    public  int daysPerSeason = 2;
    
    
    /// <summary>
    ///Number of days that have passed in the current season.
    /// </summary>
    public int daysInCurrentSeason = 1;
    
    /// <summary>
    ///Number of days that have passed in the game.
    /// </summary>
    public int dayInGame = 1;
    
    /// <summary>
    /// Reference to the day UI text.
    /// </summary>
    public TextMeshProUGUI dayUI;
    
    /// <summary>
    /// Year in the game.
    /// </summary>
    public int yearIngame = 1;

    /// <summary>
    ///List of observers that are observing the TimeManager.
    /// </summary>
    private List<IObserver> observers = new List<IObserver>();

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    ///Initializes the TimeManager.
    /// </summary>
    private void Start()
    {
        UpdateUI();
    }

    /// <summary>
    /// Adds an observer to the list of observers.
    /// </summary>
    /// <param name="observer"></param>
    public void AddObserver(IObserver observer)
    {
        observers.Add(observer);
    }

    /// <summary>
    /// Removes an observer from the list of observers.
    /// </summary>
    private void NotifyObservers()
    {
        foreach (var observer in observers)
        {
            observer.OnDayPass();
        }
    }

    /// <summary>
    /// Triggers the next day in the game.
    /// </summary>
    public void TriggerNextDay()
    {
        dayInGame += 1;
        daysInCurrentSeason += 1;

        currentDay = (DayOfWeek)(((int)currentDay + 1) % 7);

        if (daysInCurrentSeason > daysPerSeason)
        {
            daysInCurrentSeason = 1;
            currentSeason = GetNextSeason();
        }

        UpdateUI();

        OnDayPass.Invoke();
        NotifyObservers(); // Notify observers when a day passes
    }

    /// <summary>
    /// Gets the next season in the game.
    /// </summary>
    /// <returns></returns>
    private Season GetNextSeason()
    {
        int currentSeasonIndex = (int)currentSeason;
        int nextSeasonIndex = (currentSeasonIndex + 1) % 4;

        if (nextSeasonIndex == 0)
        {
            yearIngame += 1;
        }

        return (Season)nextSeasonIndex;
    }

    /// <summary>
    /// Updates the UI with the current day and season.
    /// </summary>
    private void UpdateUI()
    {
        dayUI.text = $"{currentDay}, {daysInCurrentSeason}, {currentSeason}";
    }
}
