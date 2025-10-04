using UnityEngine;
using System.Collections.Generic;

public class CrowdManager : MonoBehaviour
{
    [Header("Crowd Metrics")]
    public int totalAttendees = 0;
    public float crowdDensity = 0f; // people per square meter
    public float safeDensityThreshold = 4f; // people per square meter
    public bool crowdCrushRisk = false;

    [Header("Crowd Zones")]
    public List<CrowdZone> zones = new List<CrowdZone>();

    [Header("Entry/Exit Management")]
    public int entranceGates = 6;
    public int exitGates = 8;
    public float averageEntryRate = 100f; // people per minute
    public float averageExitRate = 80f;

    [Header("Queue Management")]
    public List<Queue> queues = new List<Queue>();
    public float maxAcceptableQueueTime = 30f; // minutes

    [Header("Crowd Behavior")]
    public float crowdMood = 75f; // 0-100
    public int complaintCount = 0;
    public int crowdIncidents = 0;

    private void Start()
    {
        InitializeZones();
        InitializeQueues();
    }

    public void UpdateCrowds()
    {
        UpdateAttendeeCount();
        UpdateCrowdDensity();
        CheckCrowdSafety();
        UpdateQueues();
        UpdateCrowdMood();
    }

    private void InitializeZones()
    {
        // Main stage area
        zones.Add(new CrowdZone
        {
            zoneName = "Main Stage",
            capacity = 20000,
            currentOccupancy = 0,
            area = 5000f, // square meters
            isActive = true
        });

        // Second stage
        zones.Add(new CrowdZone
        {
            zoneName = "Second Stage",
            capacity = 10000,
            currentOccupancy = 0,
            area = 2500f,
            isActive = true
        });

        // Food court
        zones.Add(new CrowdZone
        {
            zoneName = "Food Court",
            capacity = 5000,
            currentOccupancy = 0,
            area = 2000f,
            isActive = true
        });

        // VIP area
        zones.Add(new CrowdZone
        {
            zoneName = "VIP Area",
            capacity = 1000,
            currentOccupancy = 0,
            area = 500f,
            isActive = true
        });
    }

    private void InitializeQueues()
    {
        // Bar queues
        for (int i = 0; i < 4; i++)
        {
            queues.Add(new Queue
            {
                queueName = $"Bar {i + 1}",
                peopleInQueue = 0,
                averageWaitTime = 0f,
                serviceRate = 30f // people per minute
            });
        }

        // Food queues
        for (int i = 0; i < 6; i++)
        {
            queues.Add(new Queue
            {
                queueName = $"Food Vendor {i + 1}",
                peopleInQueue = 0,
                averageWaitTime = 0f,
                serviceRate = 20f
            });
        }

        // Toilet queues
        for (int i = 0; i < 8; i++)
        {
            queues.Add(new Queue
            {
                queueName = $"Toilets {i + 1}",
                peopleInQueue = 0,
                averageWaitTime = 0f,
                serviceRate = 40f
            });
        }
    }

    private void UpdateAttendeeCount()
    {
        totalAttendees = 0;
        foreach (CrowdZone zone in zones)
        {
            totalAttendees += zone.currentOccupancy;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentAttendees = totalAttendees;
        }
    }

    private void UpdateCrowdDensity()
    {
        foreach (CrowdZone zone in zones)
        {
            if (zone.area > 0)
            {
                zone.density = zone.currentOccupancy / zone.area;

                if (zone.density > safeDensityThreshold)
                {
                    zone.isOvercrowded = true;
                    Debug.LogWarning($"Zone {zone.zoneName} is overcrowded! Density: {zone.density:F2} people/m²");
                }
                else
                {
                    zone.isOvercrowded = false;
                }
            }
        }
    }

    private void CheckCrowdSafety()
    {
        crowdCrushRisk = false;

        foreach (CrowdZone zone in zones)
        {
            if (zone.density > safeDensityThreshold * 1.5f)
            {
                crowdCrushRisk = true;
                TriggerCrowdAlert(zone.zoneName);
            }

            if (zone.currentOccupancy > zone.capacity)
            {
                CloseZoneEntry(zone);
            }
        }
    }

    private void TriggerCrowdAlert(string zoneName)
    {
        Debug.LogError($"CROWD CRUSH RISK in {zoneName}! Initiating emergency procedures.");

        if (GameManager.Instance?.safetyManager != null)
        {
            GameManager.Instance.safetyManager.BroadcastEmergencyPA(
                $"Attention: {zoneName} has reached capacity. Please move to alternative areas."
            );
        }

        crowdIncidents++;
    }

    private void CloseZoneEntry(CrowdZone zone)
    {
        zone.entryClosed = true;
        Debug.Log($"Entry to {zone.zoneName} closed due to capacity.");
    }

    private void UpdateQueues()
    {
        foreach (Queue queue in queues)
        {
            if (queue.serviceRate > 0)
            {
                queue.averageWaitTime = queue.peopleInQueue / queue.serviceRate;

                if (queue.averageWaitTime > maxAcceptableQueueTime)
                {
                    Debug.LogWarning($"{queue.queueName} has excessive wait time: {queue.averageWaitTime:F1} minutes");
                    complaintCount++;
                    crowdMood -= 1f;
                }
            }

            // Simulate queue processing
            int peopleServed = Mathf.FloorToInt(queue.serviceRate * Time.deltaTime / 60f);
            queue.peopleInQueue = Mathf.Max(0, queue.peopleInQueue - peopleServed);
        }
    }

    private void UpdateCrowdMood()
    {
        // Improve mood slowly over time if conditions are good
        if (!crowdCrushRisk && GetAverageQueueTime() < maxAcceptableQueueTime)
        {
            crowdMood = Mathf.Min(100f, crowdMood + 0.5f * Time.deltaTime);
        }

        // Degrade mood for poor conditions
        if (crowdCrushRisk)
        {
            crowdMood -= 5f * Time.deltaTime;
        }

        crowdMood = Mathf.Clamp(crowdMood, 0f, 100f);
    }

    private float GetAverageQueueTime()
    {
        if (queues.Count == 0) return 0f;

        float total = 0f;
        foreach (Queue queue in queues)
        {
            total += queue.averageWaitTime;
        }
        return total / queues.Count;
    }

    public void SimulateAttendeeMovement()
    {
        // Randomly move attendees between zones
        foreach (CrowdZone zone in zones)
        {
            if (zone.isActive && !zone.entryClosed)
            {
                // Random arrivals
                int arrivals = Random.Range(0, 100);
                int departures = Random.Range(0, 50);

                zone.currentOccupancy += arrivals;
                zone.currentOccupancy = Mathf.Max(0, zone.currentOccupancy - departures);
            }
        }
    }

    public void AddPeopleToQueue(string queueName, int count)
    {
        foreach (Queue queue in queues)
        {
            if (queue.queueName == queueName)
            {
                queue.peopleInQueue += count;
                return;
            }
        }
    }

    public void SetZoneOccupancy(string zoneName, int occupancy)
    {
        foreach (CrowdZone zone in zones)
        {
            if (zone.zoneName == zoneName)
            {
                zone.currentOccupancy = Mathf.Min(occupancy, zone.capacity);
                return;
            }
        }
    }

    public void IncreaseServiceRate(string queueName, float multiplier)
    {
        float cost = 2000f * multiplier;

        if (GameManager.Instance != null && GameManager.Instance.SpendBudget(cost))
        {
            foreach (Queue queue in queues)
            {
                if (queue.queueName == queueName)
                {
                    queue.serviceRate *= multiplier;
                    Debug.Log($"Increased service rate for {queueName} by {multiplier}x");
                    return;
                }
            }
        }
    }
}

[System.Serializable]
public class CrowdZone
{
    public string zoneName;
    public int capacity;
    public int currentOccupancy;
    public float area; // square meters
    public float density; // people per square meter
    public bool isOvercrowded;
    public bool isActive;
    public bool entryClosed;
}

[System.Serializable]
public class Queue
{
    public string queueName;
    public int peopleInQueue;
    public float averageWaitTime; // minutes
    public float serviceRate; // people per minute
}
