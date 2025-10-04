using UnityEngine;
using System.Collections.Generic;

public class LogisticsManager : MonoBehaviour
{
    [Header("Waste Management")]
    public int binCount = 100;
    public float totalWasteCollected = 0f; // kg
    public float wastePerAttendeePerHour = 0.5f; // kg
    public float binCapacity = 50f; // kg per bin
    public List<WasteBin> bins = new List<WasteBin>();

    [Header("Water & Sanitation")]
    public int toiletCount = 80;
    public int waterStations = 20;
    public float waterConsumptionPerPerson = 2f; // liters per hour
    public float totalWaterSupply = 100000f; // liters
    public float waterUsed = 0f;
    public int toiletCleaningStaff = 10;
    public float toiletCleaningInterval = 60f; // minutes
    private float toiletCleaningTimer = 0f;

    [Header("Power & Infrastructure")]
    public int generatorsCount = 8;
    public float totalPowerCapacity = 500f; // kW
    public float currentPowerUsage = 0f;
    public bool powerOutage = false;
    public List<Generator> generators = new List<Generator>();

    [Header("Food & Beverage")]
    public int foodVendors = 15;
    public int drinkVendors = 20;
    public float foodStockLevel = 100f; // percentage
    public float drinkStockLevel = 100f; // percentage
    public float revenueFromFood = 0f;
    public float revenueFromDrinks = 0f;

    [Header("Parking & Transport")]
    public int parkingSpaces = 5000;
    public int occupiedSpaces = 0;
    public int shuttleBuses = 10;
    public int bikeRacks = 200;
    public int occupiedBikeRacks = 0;

    private void Start()
    {
        InitializeLogistics();
    }

    public void UpdateLogistics()
    {
        UpdateWasteManagement();
        UpdateWaterSupply();
        UpdatePowerSystems();
        UpdateFoodBeverage();
        UpdateTransport();
        CleanToilets();
    }

    private void InitializeLogistics()
    {
        // Initialize waste bins
        for (int i = 0; i < binCount; i++)
        {
            bins.Add(new WasteBin
            {
                binID = i,
                capacity = binCapacity,
                currentLoad = 0f,
                isFull = false,
                binType = i % 3 == 0 ? "Recycling" : "General"
            });
        }

        // Initialize generators
        for (int i = 0; i < generatorsCount; i++)
        {
            generators.Add(new Generator
            {
                generatorID = i,
                capacity = totalPowerCapacity / generatorsCount,
                isOperational = true,
                fuelLevel = 100f
            });
        }
    }

    private void UpdateWasteManagement()
    {
        if (GameManager.Instance == null) return;

        float wasteGenerated = GameManager.Instance.currentAttendees * wastePerAttendeePerHour * Time.deltaTime / 3600f;
        totalWasteCollected += wasteGenerated;

        // Distribute waste to bins
        float wastePerBin = wasteGenerated / binCount;
        foreach (WasteBin bin in bins)
        {
            if (!bin.isFull)
            {
                bin.currentLoad += wastePerBin;

                if (bin.currentLoad >= bin.capacity * 0.9f)
                {
                    bin.isFull = true;
                    Debug.LogWarning($"Waste bin {bin.binID} ({bin.binType}) is full!");
                }
            }
        }

        // Check if too many bins are full
        int fullBins = bins.FindAll(b => b.isFull).Count;
        if (fullBins > binCount * 0.7f)
        {
            Debug.LogError("Critical: 70% of waste bins are full! Waste management crisis!");
        }
    }

    private void UpdateWaterSupply()
    {
        if (GameManager.Instance == null) return;

        float waterConsumed = GameManager.Instance.currentAttendees * waterConsumptionPerPerson * Time.deltaTime / 3600f;
        waterUsed += waterConsumed;

        float waterRemaining = totalWaterSupply - waterUsed;
        float waterPercentage = (waterRemaining / totalWaterSupply) * 100f;

        if (waterPercentage < 20f)
        {
            Debug.LogWarning($"Water supply critical: {waterPercentage:F1}% remaining");
        }

        if (waterRemaining <= 0)
        {
            Debug.LogError("Water supply depleted! Emergency water delivery required!");
        }
    }

    private void UpdatePowerSystems()
    {
        currentPowerUsage = 0f;
        int operationalGenerators = 0;

        foreach (Generator gen in generators)
        {
            if (gen.isOperational)
            {
                operationalGenerators++;
                currentPowerUsage += gen.capacity * 0.7f; // Assume 70% load

                // Consume fuel
                gen.fuelLevel -= 0.5f * Time.deltaTime;

                if (gen.fuelLevel <= 0)
                {
                    gen.isOperational = false;
                    Debug.LogWarning($"Generator {gen.generatorID} out of fuel!");
                }
            }
        }

        // Check for power outage
        if (currentPowerUsage > totalPowerCapacity * 0.95f)
        {
            powerOutage = true;
            Debug.LogError("Power system overload! Outage risk!");
        }
        else
        {
            powerOutage = false;
        }
    }

    private void UpdateFoodBeverage()
    {
        if (GameManager.Instance == null) return;

        // Simulate stock depletion
        float attendees = GameManager.Instance.currentAttendees;
        foodStockLevel -= (attendees / 10000f) * Time.deltaTime;
        drinkStockLevel -= (attendees / 5000f) * Time.deltaTime;

        foodStockLevel = Mathf.Clamp(foodStockLevel, 0f, 100f);
        drinkStockLevel = Mathf.Clamp(drinkStockLevel, 0f, 100f);

        if (foodStockLevel < 20f)
        {
            Debug.LogWarning($"Food stock low: {foodStockLevel:F1}%");
        }

        if (drinkStockLevel < 20f)
        {
            Debug.LogWarning($"Drink stock low: {drinkStockLevel:F1}%");
        }

        // Generate revenue
        float foodRevenue = attendees * 0.1f * Time.deltaTime / 3600f; // $0.1 per person per hour
        float drinkRevenue = attendees * 0.15f * Time.deltaTime / 3600f;

        revenueFromFood += foodRevenue;
        revenueFromDrinks += drinkRevenue;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddRevenue(foodRevenue + drinkRevenue);
        }
    }

    private void UpdateTransport()
    {
        // Simulate parking usage
        if (GameManager.Instance != null)
        {
            float parkingDemand = GameManager.Instance.currentAttendees * 0.3f; // 30% drive
            occupiedSpaces = Mathf.Min(Mathf.FloorToInt(parkingDemand), parkingSpaces);

            if (occupiedSpaces >= parkingSpaces * 0.9f)
            {
                Debug.LogWarning("Parking nearly full!");
            }
        }
    }

    private void CleanToilets()
    {
        toiletCleaningTimer += Time.deltaTime;

        if (toiletCleaningTimer >= toiletCleaningInterval * 60f)
        {
            toiletCleaningTimer = 0f;

            Debug.Log($"Toilets cleaned by {toiletCleaningStaff} staff members");

            // Cost of cleaning
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SpendBudget(toiletCleaningStaff * 50f);
            }
        }
    }

    public void EmptyBin(int binID)
    {
        if (binID >= 0 && binID < bins.Count)
        {
            bins[binID].currentLoad = 0f;
            bins[binID].isFull = false;
            Debug.Log($"Bin {binID} emptied");

            // Cost of waste removal
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SpendBudget(100f);
            }
        }
    }

    public void RefuelGenerator(int genID)
    {
        if (genID >= 0 && genID < generators.Count)
        {
            float cost = 1000f;

            if (GameManager.Instance != null && GameManager.Instance.SpendBudget(cost))
            {
                generators[genID].fuelLevel = 100f;
                generators[genID].isOperational = true;
                Debug.Log($"Generator {genID} refueled");
            }
        }
    }

    public void RestockFood(float percentage)
    {
        float cost = percentage * 100f;

        if (GameManager.Instance != null && GameManager.Instance.SpendBudget(cost))
        {
            foodStockLevel = Mathf.Min(100f, foodStockLevel + percentage);
            Debug.Log($"Food restocked to {foodStockLevel:F1}%");
        }
    }

    public void RestockDrinks(float percentage)
    {
        float cost = percentage * 80f;

        if (GameManager.Instance != null && GameManager.Instance.SpendBudget(cost))
        {
            drinkStockLevel = Mathf.Min(100f, drinkStockLevel + percentage);
            Debug.Log($"Drinks restocked to {drinkStockLevel:F1}%");
        }
    }

    public void AddWaterSupply(float liters)
    {
        float cost = liters * 0.5f;

        if (GameManager.Instance != null && GameManager.Instance.SpendBudget(cost))
        {
            totalWaterSupply += liters;
            Debug.Log($"Water supply increased by {liters} liters");
        }
    }

    public void HireCleaningStaff(int count)
    {
        float cost = count * 400f;

        if (GameManager.Instance != null && GameManager.Instance.SpendBudget(cost))
        {
            toiletCleaningStaff += count;
            Debug.Log($"Hired {count} cleaning staff. Total: {toiletCleaningStaff}");
        }
    }
}

[System.Serializable]
public class WasteBin
{
    public int binID;
    public string binType;
    public float capacity;
    public float currentLoad;
    public bool isFull;
}

[System.Serializable]
public class Generator
{
    public int generatorID;
    public float capacity; // kW
    public bool isOperational;
    public float fuelLevel; // percentage
}
