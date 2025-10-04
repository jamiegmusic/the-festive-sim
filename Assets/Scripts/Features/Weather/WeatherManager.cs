using UnityEngine;
using System.Collections;

public enum WeatherCondition
{
    Clear,
    PartlyCloudy,
    Cloudy,
    LightRain,
    HeavyRain,
    Storm
}

public class WeatherManager : MonoBehaviour
{
    [Header("Current Weather")]
    public WeatherCondition currentCondition = WeatherCondition.Clear;
    public float temperature = 22f; // Celsius
    public float windSpeed = 5f; // km/h
    public float precipitation = 0f; // mm/hour
    public float humidity = 60f; // percentage

    [Header("Lightning Detection")]
    public bool lightningDetected = false;
    public float lightningDistance = 50f; // km
    public float lightningUpdateInterval = 60f; // seconds
    private float lightningTimer = 0f;

    [Header("Weather Forecast")]
    public WeatherCondition[] forecast = new WeatherCondition[24]; // Next 24 hours
    public float[] forecastTemperatures = new float[24];

    [Header("Weather Effects")]
    public bool coverRequired = false;
    public bool stageShutdownRequired = false;
    public float visibilityDistance = 1000f; // meters

    [Header("Weather Impact")]
    public float weatherImpactOnMood = 0f; // -50 to +50
    public float weatherImpactOnSafety = 0f; // -50 to +50

    private float weatherChangeTimer = 0f;
    private float weatherChangeInterval = 600f; // 10 minutes

    private void Start()
    {
        GenerateForecast();
    }

    public void UpdateWeather()
    {
        weatherChangeTimer += Time.deltaTime;
        lightningTimer += Time.deltaTime;

        if (weatherChangeTimer >= weatherChangeInterval)
        {
            weatherChangeTimer = 0f;
            UpdateWeatherCondition();
        }

        if (lightningTimer >= lightningUpdateInterval)
        {
            lightningTimer = 0f;
            UpdateLightningDetection();
        }

        ApplyWeatherEffects();
        UpdateWeatherImpact();
    }

    private void UpdateWeatherCondition()
    {
        // Simulate weather progression
        float changeChance = Random.value;

        if (changeChance < 0.3f) // 30% chance to change
        {
            int currentIndex = (int)currentCondition;
            int direction = Random.value > 0.5f ? 1 : -1;

            currentIndex = Mathf.Clamp(currentIndex + direction, 0, 5);
            currentCondition = (WeatherCondition)currentIndex;

            Debug.Log($"Weather changed to: {currentCondition}");
        }

        // Update temperature based on weather
        switch (currentCondition)
        {
            case WeatherCondition.Clear:
                temperature = Random.Range(20f, 28f);
                precipitation = 0f;
                break;
            case WeatherCondition.PartlyCloudy:
                temperature = Random.Range(18f, 25f);
                precipitation = 0f;
                break;
            case WeatherCondition.Cloudy:
                temperature = Random.Range(15f, 22f);
                precipitation = 0f;
                break;
            case WeatherCondition.LightRain:
                temperature = Random.Range(12f, 18f);
                precipitation = Random.Range(1f, 5f);
                break;
            case WeatherCondition.HeavyRain:
                temperature = Random.Range(10f, 16f);
                precipitation = Random.Range(5f, 15f);
                break;
            case WeatherCondition.Storm:
                temperature = Random.Range(8f, 14f);
                precipitation = Random.Range(15f, 30f);
                break;
        }

        // Update wind
        windSpeed = currentCondition == WeatherCondition.Storm
            ? Random.Range(40f, 80f)
            : Random.Range(5f, 25f);
    }

    private void UpdateLightningDetection()
    {
        if (currentCondition == WeatherCondition.Storm)
        {
            // High chance of lightning during storm
            if (Random.value < 0.8f)
            {
                lightningDetected = true;
                lightningDistance = Random.Range(1f, 30f);

                Debug.Log($"Lightning detected at {lightningDistance:F1} km");

                // Alert safety manager
                if (GameManager.Instance?.safetyManager != null)
                {
                    GameManager.Instance.safetyManager.lightningProximityKm = lightningDistance;

                    if (lightningDistance < 10f)
                    {
                        GameManager.Instance.safetyManager.TriggerLightningShutdown();
                    }
                }
            }
        }
        else if (currentCondition == WeatherCondition.HeavyRain)
        {
            // Lower chance during heavy rain
            if (Random.value < 0.3f)
            {
                lightningDetected = true;
                lightningDistance = Random.Range(20f, 50f);
            }
        }
        else
        {
            lightningDetected = false;
        }
    }

    private void ApplyWeatherEffects()
    {
        // Determine if cover is required
        coverRequired = currentCondition == WeatherCondition.HeavyRain ||
                       currentCondition == WeatherCondition.Storm;

        // Determine if stage shutdown is required
        stageShutdownRequired = currentCondition == WeatherCondition.Storm &&
                               (windSpeed > 60f || lightningDistance < 10f);

        // Update visibility
        switch (currentCondition)
        {
            case WeatherCondition.Clear:
            case WeatherCondition.PartlyCloudy:
                visibilityDistance = 10000f;
                break;
            case WeatherCondition.Cloudy:
                visibilityDistance = 5000f;
                break;
            case WeatherCondition.LightRain:
                visibilityDistance = 2000f;
                break;
            case WeatherCondition.HeavyRain:
                visibilityDistance = 500f;
                break;
            case WeatherCondition.Storm:
                visibilityDistance = 200f;
                break;
        }

        // Update slip risk in safety manager
        if (GameManager.Instance?.safetyManager != null)
        {
            GameManager.Instance.safetyManager.wetConditions =
                currentCondition == WeatherCondition.LightRain ||
                currentCondition == WeatherCondition.HeavyRain ||
                currentCondition == WeatherCondition.Storm;
        }
    }

    private void UpdateWeatherImpact()
    {
        // Impact on crowd mood
        switch (currentCondition)
        {
            case WeatherCondition.Clear:
                weatherImpactOnMood = 20f;
                weatherImpactOnSafety = 10f;
                break;
            case WeatherCondition.PartlyCloudy:
                weatherImpactOnMood = 10f;
                weatherImpactOnSafety = 5f;
                break;
            case WeatherCondition.Cloudy:
                weatherImpactOnMood = 0f;
                weatherImpactOnSafety = 0f;
                break;
            case WeatherCondition.LightRain:
                weatherImpactOnMood = -15f;
                weatherImpactOnSafety = -10f;
                break;
            case WeatherCondition.HeavyRain:
                weatherImpactOnMood = -30f;
                weatherImpactOnSafety = -25f;
                break;
            case WeatherCondition.Storm:
                weatherImpactOnMood = -50f;
                weatherImpactOnSafety = -40f;
                break;
        }

        // Apply to crowd manager
        if (GameManager.Instance?.crowdManager != null)
        {
            GameManager.Instance.crowdManager.crowdMood =
                Mathf.Clamp(GameManager.Instance.crowdManager.crowdMood + (weatherImpactOnMood * Time.deltaTime / 100f), 0f, 100f);
        }
    }

    private void GenerateForecast()
    {
        // Generate a realistic 24-hour forecast
        WeatherCondition current = currentCondition;

        for (int i = 0; i < 24; i++)
        {
            // Weather tends to persist with gradual changes
            if (Random.value < 0.3f)
            {
                int change = Random.value > 0.5f ? 1 : -1;
                int newCondition = Mathf.Clamp((int)current + change, 0, 5);
                current = (WeatherCondition)newCondition;
            }

            forecast[i] = current;

            // Temperature varies by hour
            forecastTemperatures[i] = temperature + Random.Range(-5f, 5f);
        }

        Debug.Log("24-hour weather forecast generated");
    }

    public string GetWeatherDescription()
    {
        string desc = $"{currentCondition}\n";
        desc += $"Temp: {temperature:F1}°C\n";
        desc += $"Wind: {windSpeed:F1} km/h\n";

        if (precipitation > 0)
        {
            desc += $"Rain: {precipitation:F1} mm/h\n";
        }

        if (lightningDetected)
        {
            desc += $"⚡ Lightning: {lightningDistance:F1} km\n";
        }

        return desc;
    }

    public void ForceWeatherChange(WeatherCondition condition)
    {
        currentCondition = condition;
        Debug.Log($"Weather manually set to: {condition}");
        UpdateWeatherCondition();
    }
}
