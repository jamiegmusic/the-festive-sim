using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class IncidentDashboard : MonoBehaviour
{
    [Header("UI References")]
    public Transform incidentListContainer;
    public GameObject incidentItemPrefab;
    public Text totalIncidentsText;
    public Text criticalIncidentsText;

    [Header("Filters")]
    public Toggle showSafetyToggle;
    public Toggle showMedicalToggle;
    public Toggle showWeatherToggle;

    private List<GameObject> incidentUIItems = new List<GameObject>();

    private void Start()
    {
        InvokeRepeating(nameof(RefreshIncidentList), 0f, 2f);
    }

    public void RefreshIncidentList()
    {
        ClearIncidentList();

        if (GameManager.Instance == null) return;

        int totalIncidents = 0;
        int criticalIncidents = 0;

        // Safety incidents
        if (showSafetyToggle == null || showSafetyToggle.isOn)
        {
            if (GameManager.Instance.safetyManager != null)
            {
                totalIncidents += GameManager.Instance.safetyManager.incidentCount;

                if (GameManager.Instance.safetyManager.emergencyShutdownActive)
                {
                    CreateIncidentItem("CRITICAL: Lightning Emergency Shutdown", "Safety", true);
                    criticalIncidents++;
                }

                if (!GameManager.Instance.safetyManager.barriersIntact)
                {
                    CreateIncidentItem("WARNING: Barrier Overload Detected", "Safety", true);
                    criticalIncidents++;
                }

                if (GameManager.Instance.safetyManager.lostChildren.Count > 0)
                {
                    foreach (var child in GameManager.Instance.safetyManager.lostChildren)
                    {
                        CreateIncidentItem($"Lost Child: {child.childName}", "Safety", false);
                    }
                }
            }
        }

        // Medical incidents
        if (showMedicalToggle == null || showMedicalToggle.isOn)
        {
            if (GameManager.Instance.medicalManager != null)
            {
                totalIncidents += GameManager.Instance.medicalManager.totalTreatments;

                if (GameManager.Instance.medicalManager.GetCapacityPercentage() > 80f)
                {
                    CreateIncidentItem("WARNING: Medical capacity critical", "Medical", true);
                    criticalIncidents++;
                }

                foreach (var treatment in GameManager.Instance.medicalManager.activeTreatments)
                {
                    if (treatment.severity == MedicalSeverity.Critical)
                    {
                        CreateIncidentItem($"Critical: {treatment.condition}", "Medical", true);
                        criticalIncidents++;
                    }
                }
            }
        }

        // Weather incidents
        if (showWeatherToggle == null || showWeatherToggle.isOn)
        {
            if (GameManager.Instance.weatherManager != null)
            {
                if (GameManager.Instance.weatherManager.currentCondition == WeatherCondition.Storm)
                {
                    CreateIncidentItem("CRITICAL: Storm conditions", "Weather", true);
                    criticalIncidents++;
                }
            }
        }

        // Update totals
        if (totalIncidentsText != null)
            totalIncidentsText.text = $"Total Incidents: {totalIncidents}";

        if (criticalIncidentsText != null)
            criticalIncidentsText.text = $"Critical: {criticalIncidents}";
    }

    private void CreateIncidentItem(string description, string category, bool isCritical)
    {
        if (incidentItemPrefab == null || incidentListContainer == null) return;

        GameObject item = Instantiate(incidentItemPrefab, incidentListContainer);
        incidentUIItems.Add(item);

        Text itemText = item.GetComponentInChildren<Text>();
        if (itemText != null)
        {
            itemText.text = $"[{category}] {description}";
            itemText.color = isCritical ? Color.red : Color.yellow;
        }
    }

    private void ClearIncidentList()
    {
        foreach (GameObject item in incidentUIItems)
        {
            Destroy(item);
        }
        incidentUIItems.Clear();
    }
}
