# Festival Simulator - Quick Start Guide

## 🚀 Get Started in 5 Minutes

### Step 1: Open in Unity

1. Open Unity Hub
2. Click "Add" and select the `FestivalSimUnity` folder
3. Open the project (Unity 2021.3+ required)

### Step 2: Open the Scene

1. In Unity, go to `Assets/Scenes/`
2. Double-click `FestivalMap.unity`

### Step 3: Start the Festival

1. Press the **Play** button in Unity
2. Open the Console window (Window > General > Console)
3. The festival will initialize all systems automatically

### Step 4: Control the Festival

Open the Console and type these commands in a script or use the Inspector:

```csharp
// Start the festival
GameManager.Instance.StartFestival();

// Add attendees to zones
GameManager.Instance.crowdManager.SetZoneOccupancy("Main Stage", 5000);

// Hire staff
GameManager.Instance.safetyManager.HireSecurityStaff(50);
GameManager.Instance.medicalManager.HireMedicalStaff(20);

// Change weather
GameManager.Instance.weatherManager.ForceWeatherChange(WeatherCondition.Storm);

// Trigger events
GameManager.Instance.medicalManager.AdministerNaloxone("Patient", "Emergency");
GameManager.Instance.safetyManager.ReportLostChild("Emma", "5 years old, blue dress", "555-0123");
```

## 🎮 Testing the Simulation

### Create a Test Script

1. Create `Assets/Scripts/TestController.cs`:

```csharp
using UnityEngine;

public class TestController : MonoBehaviour
{
    void Start()
    {
        // Wait 1 second for initialization
        Invoke("StartTest", 1f);
    }

    void StartTest()
    {
        // Start festival
        GameManager.Instance.StartFestival();

        // Add crowds
        GameManager.Instance.crowdManager.SetZoneOccupancy("Main Stage", 15000);
        GameManager.Instance.crowdManager.SetZoneOccupancy("Food Court", 2000);

        // Hire staff
        GameManager.Instance.safetyManager.HireSecurityStaff(150);
        GameManager.Instance.medicalManager.HireMedicalStaff(50);

        // Simulate some incidents
        InvokeRepeating("SimulateEvents", 5f, 10f);
    }

    void SimulateEvents()
    {
        // Random medical incident
        if (Random.value > 0.7f)
        {
            GameManager.Instance.medicalManager.RegisterTreatment(
                "Attendee " + Random.Range(1, 1000),
                "Dehydration",
                MedicalSeverity.Minor
            );
        }

        // Random weather change
        if (Random.value > 0.8f)
        {
            WeatherCondition[] conditions = System.Enum.GetValues(typeof(WeatherCondition)) as WeatherCondition[];
            GameManager.Instance.weatherManager.ForceWeatherChange(conditions[Random.Range(0, conditions.Length)]);
        }
    }

    void Update()
    {
        // Press SPACE to add more attendees
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.crowdManager.SetZoneOccupancy("Main Stage",
                GameManager.Instance.crowdManager.zones[0].currentOccupancy + 1000);
        }

        // Press L to trigger lightning
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameManager.Instance.weatherManager.ForceWeatherChange(WeatherCondition.Storm);
            GameManager.Instance.safetyManager.lightningProximityKm = 5f;
        }

        // Press M for medical emergency
        if (Input.GetKeyDown(KeyCode.M))
        {
            GameManager.Instance.medicalManager.AdministerNaloxone("Emergency Patient", "Overdose");
        }
    }
}
```

2. Attach to GameManager object in the scene
3. Press Play and use keyboard controls!

## 📊 Monitoring the Festival

### Console Output

Watch the Console for real-time updates:
- Safety incidents
- Medical treatments
- Weather changes
- Budget updates
- Staff hiring
- Capacity warnings

### Inspector Values

Select the GameManager object and watch:
- `currentAttendees`
- `currentBudget`
- `gameTime`
- Safety Manager stats
- Medical Manager stats
- Weather conditions

## 🎨 Adding Visual Assets (Optional)

### Free Assets to Try

1. **Kenney Assets** (kenney.nl)
   - Download "City Kit" or "Modular Buildings"
   - Import to `Assets/Models/`

2. **Synty Studios** (Unity Asset Store)
   - Search "POLYGON Concert" or "POLYGON City"
   - Import to project

3. **Terrain**
   - GameObject > 3D Object > Terrain
   - Paint a festival ground

## 🔧 Build Settings

To build a standalone game:

1. File > Build Settings
2. Add `FestivalMap` scene
3. Select platform (Windows/Mac/Linux)
4. Click "Build"

## 📝 Next Steps

1. **Read the Full Docs**
   - `README.md` - Overview
   - `FEATURES.md` - Complete feature list
   - `MODDING.md` - Create mods

2. **Customize Your Festival**
   - Change festival name in GameManager
   - Adjust budget and capacity
   - Create custom scenarios

3. **Create UI**
   - Use `FestivalInfoPanel.cs` as template
   - Design dashboard in Unity UI
   - Connect to game data

4. **Make Mods**
   - Follow MODDING.md guide
   - Share with community

## 🐛 Troubleshooting

### "GameManager.Instance is null"
- Make sure GameManager exists in the scene
- Check DontDestroyOnLoad is working

### "No managers attached"
- Managers auto-create on Start()
- Wait 1 frame before accessing

### "Mods not loading"
- Check `Mods/` folder exists
- Verify DLL compatibility
- Check Console for errors

## 🎯 Example Scenarios to Try

### Scenario 1: Perfect Festival
```csharp
GameManager.Instance.StartFestival();
GameManager.Instance.safetyManager.HireSecurityStaff(200);
GameManager.Instance.medicalManager.HireMedicalStaff(100);
GameManager.Instance.weatherManager.ForceWeatherChange(WeatherCondition.Clear);
GameManager.Instance.crowdManager.SetZoneOccupancy("Main Stage", 20000);
```

### Scenario 2: Emergency Situation
```csharp
GameManager.Instance.StartFestival();
GameManager.Instance.weatherManager.ForceWeatherChange(WeatherCondition.Storm);
GameManager.Instance.safetyManager.lightningProximityKm = 5f;
GameManager.Instance.crowdManager.SetZoneOccupancy("Main Stage", 25000);
GameManager.Instance.medicalManager.RegisterTreatment("John", "Lightning injury", MedicalSeverity.Critical);
```

### Scenario 3: Overcrowding Crisis
```csharp
GameManager.Instance.StartFestival();
GameManager.Instance.crowdManager.SetZoneOccupancy("Main Stage", 30000); // Over capacity!
GameManager.Instance.safetyManager.HireSecurityStaff(10); // Understaffed!
```

## 💡 Pro Tips

1. **Time Scale**: Increase `gameTime` speed for faster testing
2. **Budget**: Adjust `festivalBudget` for different challenges
3. **Auto-Save**: Implement save/load for persistence
4. **Debug Mode**: Add a debug panel to control everything
5. **Analytics**: Log data to CSV for analysis

---

**You're ready to manage your festival! 🎉**

Press Play and watch your festival come to life!
