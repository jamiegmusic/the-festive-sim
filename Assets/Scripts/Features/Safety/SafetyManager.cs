using UnityEngine;
using System.Collections.Generic;

public class SafetyManager : MonoBehaviour
{
    [Header("Safety Metrics")]
    public float overallSafetyRating = 100f; // 0-100
    public int securityStaffCount = 0;
    public int requiredSecurityStaff = 50;
    public int incidentCount = 0;

    [Header("Barriers & Crowd Control")]
    public List<Barrier> barriers = new List<Barrier>();
    public float barrierLoadCapacity = 1000f; // kg per barrier
    public bool barriersIntact = true;

    [Header("Weather Safety")]
    public bool lightningDetected = false;
    public float lightningProximityKm = 15f;
    public float lightningShutdownThreshold = 10f;
    public bool emergencyShutdownActive = false;

    [Header("Slip Risk")]
    public float slipRiskIndex = 0f; // 0-100
    public bool wetConditions = false;

    [Header("Emergency Systems")]
    public List<EmergencyRoute> emergencyRoutes = new List<EmergencyRoute>();
    public int emergencyExits = 8;
    public bool emergencyLightingActive = true;

    [Header("Confiscation & Amnesty")]
    public List<ConfiscatedItem> confiscatedItems = new List<ConfiscatedItem>();
    public int amnestyBinsCount = 10;

    [Header("Lost Children")]
    public List<LostChild> lostChildren = new List<LostChild>();
    public int lostChildrenReunited = 0;

    private void Start()
    {
        InitializeSafety();
    }

    public void UpdateSafety()
    {
        CalculateSafetyRating();
        CheckSecurityStaffing();
        MonitorBarriers();
        UpdateSlipRisk();
        CheckLightningProximity();
    }

    private void InitializeSafety()
    {
        // Initialize emergency routes
        for (int i = 0; i < emergencyExits; i++)
        {
            emergencyRoutes.Add(new EmergencyRoute
            {
                routeID = i,
                isBlocked = false,
                capacity = 500
            });
        }
    }

    private void CalculateSafetyRating()
    {
        float rating = 100f;

        // Reduce rating based on various factors
        if (securityStaffCount < requiredSecurityStaff)
        {
            rating -= 20f;
        }

        if (!barriersIntact)
        {
            rating -= 30f;
        }

        if (slipRiskIndex > 50f)
        {
            rating -= 15f;
        }

        if (incidentCount > 5)
        {
            rating -= incidentCount * 2f;
        }

        if (lostChildren.Count > 0)
        {
            rating -= lostChildren.Count * 5f;
        }

        overallSafetyRating = Mathf.Clamp(rating, 0f, 100f);
    }

    private void CheckSecurityStaffing()
    {
        int attendees = GameManager.Instance != null ? GameManager.Instance.currentAttendees : 0;
        requiredSecurityStaff = Mathf.CeilToInt(attendees / 100f); // 1 security per 100 attendees
    }

    private void MonitorBarriers()
    {
        foreach (Barrier barrier in barriers)
        {
            if (barrier.currentLoad > barrierLoadCapacity)
            {
                barrier.isOverloaded = true;
                barriersIntact = false;
                LogIncident("Barrier Overload", $"Barrier {barrier.barrierID} exceeded load capacity");
            }
        }
    }

    private void UpdateSlipRisk()
    {
        if (wetConditions)
        {
            slipRiskIndex = Mathf.Clamp(slipRiskIndex + 5f * Time.deltaTime, 0f, 100f);
        }
        else
        {
            slipRiskIndex = Mathf.Clamp(slipRiskIndex - 2f * Time.deltaTime, 0f, 100f);
        }
    }

    private void CheckLightningProximity()
    {
        if (lightningProximityKm <= lightningShutdownThreshold && !emergencyShutdownActive)
        {
            TriggerLightningShutdown();
        }
    }

    public void TriggerLightningShutdown()
    {
        emergencyShutdownActive = true;
        lightningDetected = true;
        Debug.LogWarning("EMERGENCY: Lightning detected within safe distance. Initiating shutdown protocol.");

        // Shutdown outdoor stages
        BroadcastEmergencyPA("Lightning detected. All outdoor activities suspended. Seek indoor shelter immediately.");

        if (GameManager.Instance != null)
        {
            // Can add more shutdown logic here
        }
    }

    public void BroadcastEmergencyPA(string message)
    {
        Debug.Log($"[PA SYSTEM - EMERGENCY]: {message}");
        // This would trigger actual audio system in full implementation
    }

    public void LogIncident(string type, string description)
    {
        incidentCount++;
        Debug.LogWarning($"[INCIDENT {incidentCount}] {type}: {description}");
    }

    public void ReportLostChild(string childName, string description, string parentContact)
    {
        LostChild child = new LostChild
        {
            childName = childName,
            description = description,
            parentContact = parentContact,
            reportTime = GameManager.Instance != null ? GameManager.Instance.gameTime : 0f
        };

        lostChildren.Add(child);
        BroadcastEmergencyPA($"Lost child alert: {childName}. {description}. Please report to nearest information point.");
    }

    public void ReuniteLostChild(int childIndex)
    {
        if (childIndex >= 0 && childIndex < lostChildren.Count)
        {
            lostChildren.RemoveAt(childIndex);
            lostChildrenReunited++;
            Debug.Log("Lost child successfully reunited with parents.");
        }
    }

    public void ConfiscateItem(string itemType, string reason)
    {
        ConfiscatedItem item = new ConfiscatedItem
        {
            itemType = itemType,
            reason = reason,
            confiscationTime = GameManager.Instance != null ? GameManager.Instance.gameTime : 0f
        };

        confiscatedItems.Add(item);
    }

    public void HireSecurityStaff(int count)
    {
        float cost = count * 500f; // $500 per security staff
        if (GameManager.Instance != null && GameManager.Instance.SpendBudget(cost))
        {
            securityStaffCount += count;
            Debug.Log($"Hired {count} security staff. Total: {securityStaffCount}");
        }
    }
}

[System.Serializable]
public class Barrier
{
    public int barrierID;
    public Vector3 position;
    public float currentLoad;
    public bool isOverloaded;
}

[System.Serializable]
public class EmergencyRoute
{
    public int routeID;
    public bool isBlocked;
    public int capacity;
}

[System.Serializable]
public class ConfiscatedItem
{
    public string itemType;
    public string reason;
    public float confiscationTime;
}

[System.Serializable]
public class LostChild
{
    public string childName;
    public string description;
    public string parentContact;
    public float reportTime;
}
