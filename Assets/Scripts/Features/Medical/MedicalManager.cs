using UnityEngine;
using System.Collections.Generic;

public enum MedicalSeverity { Minor, Moderate, Severe, Critical }

public class MedicalManager : MonoBehaviour
{
    [Header("Medical Resources")]
    public int medicalStaffCount = 10;
    public int requiredMedicalStaff = 20;
    public int firstAidKits = 50;
    public int naloxoneKits = 20;
    public int ambulancesAvailable = 3;

    [Header("Medical Facilities")]
    public int medicalTentsCount = 5;
    public int bedsPerTent = 4;
    public int occupiedBeds = 0;

    [Header("Treatments & Incidents")]
    public List<MedicalTreatment> activeTreatments = new List<MedicalTreatment>();
    public int totalTreatments = 0;
    public int naloxoneAdministered = 0;
    public int ambulanceCallouts = 0;

    [Header("Certifications")]
    public int certifiedFirstAiders = 15;
    public int certifiedParamedics = 5;

    [Header("Food Safety")]
    public List<FoodSafetyLog> foodSafetyLogs = new List<FoodSafetyLog>();
    public int foodSafetyViolations = 0;

    private float inspectionTimer = 0f;
    private float inspectionInterval = 300f; // 5 minutes

    public void UpdateMedical()
    {
        UpdateStaffingRequirements();
        ProcessTreatments();
        RunFoodSafetyInspections();
    }

    private void UpdateStaffingRequirements()
    {
        int attendees = GameManager.Instance != null ? GameManager.Instance.currentAttendees : 0;
        requiredMedicalStaff = Mathf.CeilToInt(attendees / 1000f); // 1 medical staff per 1000 attendees
    }

    private void ProcessTreatments()
    {
        for (int i = activeTreatments.Count - 1; i >= 0; i--)
        {
            MedicalTreatment treatment = activeTreatments[i];
            treatment.timeElapsed += Time.deltaTime;

            // Complete treatment after certain time
            if (treatment.timeElapsed >= treatment.treatmentDuration)
            {
                CompleteTreatment(i);
            }
        }
    }

    private void RunFoodSafetyInspections()
    {
        inspectionTimer += Time.deltaTime;

        if (inspectionTimer >= inspectionInterval)
        {
            inspectionTimer = 0f;
            PerformFoodSafetyInspection();
        }
    }

    public void RegisterTreatment(string patientName, string condition, MedicalSeverity severity)
    {
        if (occupiedBeds >= medicalTentsCount * bedsPerTent)
        {
            Debug.LogWarning("Medical facilities at capacity! Calling ambulance.");
            CallAmbulance(patientName, condition);
            return;
        }

        MedicalTreatment treatment = new MedicalTreatment
        {
            patientName = patientName,
            condition = condition,
            severity = severity,
            timeStarted = GameManager.Instance != null ? GameManager.Instance.gameTime : 0f,
            treatmentDuration = GetTreatmentDuration(severity),
            timeElapsed = 0f
        };

        activeTreatments.Add(treatment);
        occupiedBeds++;
        totalTreatments++;

        Debug.Log($"Treatment started: {patientName} - {condition} ({severity})");
    }

    private float GetTreatmentDuration(MedicalSeverity severity)
    {
        switch (severity)
        {
            case MedicalSeverity.Minor: return 300f; // 5 minutes
            case MedicalSeverity.Moderate: return 900f; // 15 minutes
            case MedicalSeverity.Severe: return 1800f; // 30 minutes
            case MedicalSeverity.Critical: return 3600f; // 1 hour
            default: return 600f;
        }
    }

    private void CompleteTreatment(int index)
    {
        MedicalTreatment treatment = activeTreatments[index];
        Debug.Log($"Treatment completed: {treatment.patientName} - {treatment.condition}");

        activeTreatments.RemoveAt(index);
        occupiedBeds--;
    }

    public void AdministerNaloxone(string patientName, string situation)
    {
        if (naloxoneKits > 0)
        {
            naloxoneKits--;
            naloxoneAdministered++;

            Debug.Log($"Naloxone administered to {patientName}. Situation: {situation}");
            Debug.Log($"Naloxone kits remaining: {naloxoneKits}");

            RegisterTreatment(patientName, "Overdose - Naloxone administered", MedicalSeverity.Critical);
        }
        else
        {
            Debug.LogError("No naloxone kits available! Emergency medical response required.");
            CallAmbulance(patientName, "Overdose - No naloxone available");
        }
    }

    public void CallAmbulance(string patientName, string condition)
    {
        if (ambulancesAvailable > 0)
        {
            ambulanceCallouts++;
            Debug.LogWarning($"Ambulance called for {patientName}: {condition}");

            // Ambulance will be available again after some time
            StartCoroutine(ReturnAmbulance());
        }
        else
        {
            Debug.LogError("No ambulances available! Critical situation!");
        }
    }

    private System.Collections.IEnumerator ReturnAmbulance()
    {
        ambulancesAvailable--;
        yield return new WaitForSeconds(1800f); // 30 minute round trip
        ambulancesAvailable++;
        Debug.Log("Ambulance returned and available.");
    }

    public void UseFirstAidKit(string purpose)
    {
        if (firstAidKits > 0)
        {
            firstAidKits--;
            Debug.Log($"First aid kit used: {purpose}. Remaining: {firstAidKits}");

            if (firstAidKits < 10)
            {
                Debug.LogWarning("First aid kit supplies running low!");
            }
        }
        else
        {
            Debug.LogError("No first aid kits available!");
        }
    }

    public void PerformFoodSafetyInspection()
    {
        // Simulate random inspection
        bool passed = Random.value > 0.2f; // 80% pass rate

        FoodSafetyLog log = new FoodSafetyLog
        {
            inspectionTime = GameManager.Instance != null ? GameManager.Instance.gameTime : 0f,
            passed = passed,
            vendorID = Random.Range(1, 20),
            notes = passed ? "All standards met" : "Violations found"
        };

        foodSafetyLogs.Add(log);

        if (!passed)
        {
            foodSafetyViolations++;
            Debug.LogWarning($"Food safety violation at vendor {log.vendorID}");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SpendBudget(5000f); // Fine
            }
        }
    }

    public float GetCapacityPercentage()
    {
        int totalBeds = medicalTentsCount * bedsPerTent;
        return totalBeds > 0 ? (occupiedBeds / (float)totalBeds) * 100f : 0f;
    }

    public void HireMedicalStaff(int count, bool isParamedic = false)
    {
        float costPerStaff = isParamedic ? 1000f : 600f;
        float totalCost = count * costPerStaff;

        if (GameManager.Instance != null && GameManager.Instance.SpendBudget(totalCost))
        {
            medicalStaffCount += count;

            if (isParamedic)
                certifiedParamedics += count;
            else
                certifiedFirstAiders += count;

            Debug.Log($"Hired {count} {(isParamedic ? "paramedics" : "first aiders")}. Total staff: {medicalStaffCount}");
        }
    }

    public void RestockSupplies(string itemType, int quantity)
    {
        float costPerItem = 0f;

        switch (itemType.ToLower())
        {
            case "firstaid":
                firstAidKits += quantity;
                costPerItem = 50f;
                Debug.Log($"Restocked {quantity} first aid kits. Total: {firstAidKits}");
                break;

            case "naloxone":
                naloxoneKits += quantity;
                costPerItem = 200f;
                Debug.Log($"Restocked {quantity} naloxone kits. Total: {naloxoneKits}");
                break;

            default:
                Debug.LogWarning($"Unknown supply type: {itemType}");
                return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SpendBudget(quantity * costPerItem);
        }
    }
}

[System.Serializable]
public class MedicalTreatment
{
    public string patientName;
    public string condition;
    public MedicalSeverity severity;
    public float timeStarted;
    public float treatmentDuration;
    public float timeElapsed;
}

[System.Serializable]
public class FoodSafetyLog
{
    public float inspectionTime;
    public int vendorID;
    public bool passed;
    public string notes;
}
