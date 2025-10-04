using UnityEngine;
using UnityEngine.UI;

public class FestivalInfoPanel : MonoBehaviour
{
    [Header("UI References")]
    public Text festivalNameText;
    public Text attendeeCountText;
    public Text budgetText;
    public Text gameTimeText;
    public Text safetyRatingText;

    [Header("Status Indicators")]
    public Image safetyIndicator;
    public Image medicalIndicator;
    public Image weatherIndicator;

    [Header("Colors")]
    public Color goodColor = Color.green;
    public Color warningColor = Color.yellow;
    public Color criticalColor = Color.red;

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (GameManager.Instance == null) return;

        // Update basic info
        if (festivalNameText != null)
            festivalNameText.text = GameManager.Instance.festivalName;

        if (attendeeCountText != null)
            attendeeCountText.text = $"Attendees: {GameManager.Instance.currentAttendees:N0} / {GameManager.Instance.maxAttendees:N0}";

        if (budgetText != null)
            budgetText.text = $"Budget: ${GameManager.Instance.currentBudget:N0}";

        if (gameTimeText != null)
        {
            int hours = Mathf.FloorToInt(GameManager.Instance.gameTime);
            int minutes = Mathf.FloorToInt((GameManager.Instance.gameTime - hours) * 60);
            gameTimeText.text = $"Time: {hours:D2}:{minutes:D2}";
        }

        // Update safety rating
        UpdateSafetyRating();

        // Update status indicators
        UpdateStatusIndicators();
    }

    private void UpdateSafetyRating()
    {
        if (GameManager.Instance.safetyManager == null || safetyRatingText == null) return;

        float rating = GameManager.Instance.safetyManager.overallSafetyRating;
        safetyRatingText.text = $"Safety: {rating:F1}%";

        if (safetyRatingText != null)
        {
            if (rating >= 80f)
                safetyRatingText.color = goodColor;
            else if (rating >= 50f)
                safetyRatingText.color = warningColor;
            else
                safetyRatingText.color = criticalColor;
        }
    }

    private void UpdateStatusIndicators()
    {
        // Safety indicator
        if (safetyIndicator != null && GameManager.Instance.safetyManager != null)
        {
            float safetyRating = GameManager.Instance.safetyManager.overallSafetyRating;
            safetyIndicator.color = GetColorForRating(safetyRating);
        }

        // Medical indicator
        if (medicalIndicator != null && GameManager.Instance.medicalManager != null)
        {
            float medicalCapacity = GameManager.Instance.medicalManager.GetCapacityPercentage();
            medicalIndicator.color = GetColorForRating(100f - medicalCapacity);
        }

        // Weather indicator
        if (weatherIndicator != null && GameManager.Instance.weatherManager != null)
        {
            WeatherCondition condition = GameManager.Instance.weatherManager.currentCondition;
            weatherIndicator.color = GetColorForWeather(condition);
        }
    }

    private Color GetColorForRating(float rating)
    {
        if (rating >= 80f)
            return goodColor;
        else if (rating >= 50f)
            return warningColor;
        else
            return criticalColor;
    }

    private Color GetColorForWeather(WeatherCondition condition)
    {
        switch (condition)
        {
            case WeatherCondition.Clear:
            case WeatherCondition.PartlyCloudy:
                return goodColor;
            case WeatherCondition.Cloudy:
            case WeatherCondition.LightRain:
                return warningColor;
            case WeatherCondition.HeavyRain:
            case WeatherCondition.Storm:
                return criticalColor;
            default:
                return Color.gray;
        }
    }
}
