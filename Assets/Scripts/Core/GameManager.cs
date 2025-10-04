using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Festival Settings")]
    public string festivalName = "Summer Music Festival";
    public int maxAttendees = 50000;
    public int currentAttendees = 0;
    public float festivalBudget = 1000000f;
    public float currentBudget;

    [Header("Systems")]
    public SafetyManager safetyManager;
    public MedicalManager medicalManager;
    public CrowdManager crowdManager;
    public WeatherManager weatherManager;
    public LogisticsManager logisticsManager;

    [Header("Game State")]
    public bool festivalActive = false;
    public float gameTime = 0f; // In hours
    public float timeScale = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentBudget = festivalBudget;
        InitializeSystems();
    }

    private void Update()
    {
        if (festivalActive)
        {
            gameTime += Time.deltaTime * timeScale / 3600f; // Convert to hours
            UpdateSystems();
        }
    }

    private void InitializeSystems()
    {
        if (safetyManager == null) safetyManager = gameObject.AddComponent<SafetyManager>();
        if (medicalManager == null) medicalManager = gameObject.AddComponent<MedicalManager>();
        if (crowdManager == null) crowdManager = gameObject.AddComponent<CrowdManager>();
        if (weatherManager == null) weatherManager = gameObject.AddComponent<WeatherManager>();
        if (logisticsManager == null) logisticsManager = gameObject.AddComponent<LogisticsManager>();
    }

    private void UpdateSystems()
    {
        safetyManager?.UpdateSafety();
        medicalManager?.UpdateMedical();
        crowdManager?.UpdateCrowds();
        weatherManager?.UpdateWeather();
        logisticsManager?.UpdateLogistics();
    }

    public void StartFestival()
    {
        festivalActive = true;
        Debug.Log($"{festivalName} has started!");
    }

    public void StopFestival()
    {
        festivalActive = false;
        Debug.Log($"{festivalName} has ended!");
    }

    public bool SpendBudget(float amount)
    {
        if (currentBudget >= amount)
        {
            currentBudget -= amount;
            return true;
        }
        Debug.LogWarning("Insufficient budget!");
        return false;
    }

    public void AddRevenue(float amount)
    {
        currentBudget += amount;
    }
}
