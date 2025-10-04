# Festival Simulator - Modding Guide

Welcome to the Festival Simulator modding system! This guide will help you create custom content, features, and experiences.

## 📋 Table of Contents

1. [Getting Started](#getting-started)
2. [Mod Types](#mod-types)
3. [Creating DLL Mods](#creating-dll-mods)
4. [Creating JSON Mods](#creating-json-mods)
5. [Mod API Reference](#mod-api-reference)
6. [Best Practices](#best-practices)
7. [Examples](#examples)

## 🚀 Getting Started

### Folder Structure

Place all mods in the `Mods/` folder in your Festival Simulator directory:

```
FestivalSimUnity/
├── Mods/
│   ├── MyCustomMod.dll          # Compiled DLL mods
│   ├── MyJsonMod.json           # JSON configuration mods
│   └── SubFolder/               # Organize in folders
│       └── AnotherMod.dll
```

Mods are automatically discovered and loaded when the game starts.

## 🎨 Mod Types

### 1. DLL Mods (Code-Based)
- Full programmatic control
- Access to Unity API
- Can modify game behavior
- Requires C# programming knowledge

### 2. JSON Mods (Data-Based)
- No coding required
- Define custom buildings, rules, scenarios
- Easy to create and share
- Limited to predefined structures

## 🔧 Creating DLL Mods

### Step 1: Set Up Your Project

Create a new C# Class Library project:

```bash
# Using .NET CLI
dotnet new classlib -n MyFestivalMod
```

### Step 2: Add Unity References

Add references to Unity DLLs from your Unity installation:

- `UnityEngine.dll`
- `UnityEngine.CoreModule.dll`

Also reference the Festival Simulator assembly.

### Step 3: Implement IFestivalMod

```csharp
using UnityEngine;

public class MyCustomMod : IFestivalMod
{
    public string GetModName()
    {
        return "My Awesome Festival Mod";
    }

    public string GetModVersion()
    {
        return "1.0.0";
    }

    public void OnModLoaded()
    {
        Debug.Log($"{GetModName()} v{GetModVersion()} loaded!");

        // Initialize your mod here
        RegisterCustomFeatures();
    }

    public void OnModUnloaded()
    {
        Debug.Log($"{GetModName()} unloaded!");

        // Cleanup here
    }

    private void RegisterCustomFeatures()
    {
        // Your custom code
    }
}
```

### Step 4: Compile and Deploy

```bash
# Build your project
dotnet build -c Release

# Copy the DLL to Mods folder
cp bin/Release/MyFestivalMod.dll <FestivalSimUnity>/Mods/
```

## 📄 Creating JSON Mods

JSON mods use a predefined structure to add content without coding.

### Basic JSON Mod Structure

```json
{
  "modName": "Summer Stages Expansion",
  "version": "1.0",
  "author": "YourName",
  "customBuildings": [
    {
      "buildingName": "Mega Stage",
      "modelPath": "Models/MegaStage",
      "cost": 50000,
      "capacity": 30000
    }
  ],
  "customRules": [
    {
      "ruleName": "No Glass Bottles",
      "description": "Glass bottles are prohibited",
      "penaltyAmount": 1000
    }
  ],
  "customScenarios": [
    {
      "scenarioName": "EDM Festival Challenge",
      "description": "Host a 100k person EDM festival",
      "targetAttendees": 100000,
      "budget": 5000000
    }
  ]
}
```

### Supported JSON Features

#### Custom Buildings

```json
"customBuildings": [
  {
    "buildingName": "VIP Lounge Deluxe",
    "modelPath": "Models/VIPLounge",
    "cost": 25000,
    "capacity": 500
  }
]
```

#### Custom Rules

```json
"customRules": [
  {
    "ruleName": "Mandatory Ear Protection",
    "description": "All attendees must wear ear protection",
    "penaltyAmount": 5000
  }
]
```

#### Custom Scenarios

```json
"customScenarios": [
  {
    "scenarioName": "Rainy Day Challenge",
    "description": "Run a successful festival during a storm",
    "targetAttendees": 20000,
    "budget": 500000
  }
]
```

## 🔌 Mod API Reference

### Accessing Game Managers

```csharp
// Access the main game manager
GameManager gameManager = GameManager.Instance;

// Access specific systems
SafetyManager safety = gameManager.safetyManager;
MedicalManager medical = gameManager.medicalManager;
CrowdManager crowd = gameManager.crowdManager;
WeatherManager weather = gameManager.weatherManager;
LogisticsManager logistics = gameManager.logisticsManager;
```

### Common Mod Operations

#### Modify Budget

```csharp
public void OnModLoaded()
{
    // Give player bonus starting budget
    GameManager.Instance.AddRevenue(100000f);
}
```

#### Add Custom Events

```csharp
public void OnModLoaded()
{
    // Subscribe to update loop
    GameManager.Instance.StartCoroutine(CustomUpdate());
}

private IEnumerator CustomUpdate()
{
    while (true)
    {
        // Your custom logic every frame
        CheckCustomConditions();
        yield return null;
    }
}

private void CheckCustomConditions()
{
    if (GameManager.Instance.currentAttendees > 50000)
    {
        Debug.Log("Mega crowd bonus activated!");
        GameManager.Instance.AddRevenue(10000f);
    }
}
```

#### Create Custom Incidents

```csharp
public void TriggerCustomIncident()
{
    var safety = GameManager.Instance.safetyManager;
    safety.LogIncident("Custom Event", "A special event occurred!");
}
```

#### Modify Weather

```csharp
public void MakeItRain()
{
    var weather = GameManager.Instance.weatherManager;
    weather.ForceWeatherChange(WeatherCondition.HeavyRain);
}
```

#### Add Medical Cases

```csharp
public void AddMedicalEmergency()
{
    var medical = GameManager.Instance.medicalManager;
    medical.RegisterTreatment(
        "John Doe",
        "Heat Exhaustion",
        MedicalSeverity.Moderate
    );
}
```

#### Manage Crowds

```csharp
public void SetCrowdCapacity(string zoneName, int newCapacity)
{
    var crowd = GameManager.Instance.crowdManager;
    foreach (var zone in crowd.zones)
    {
        if (zone.zoneName == zoneName)
        {
            zone.capacity = newCapacity;
            Debug.Log($"Zone {zoneName} capacity set to {newCapacity}");
            break;
        }
    }
}
```

## ✅ Best Practices

### Performance

1. **Avoid Update-Heavy Logic**: Don't put expensive operations in frequently-called methods
2. **Use Coroutines**: For delayed or periodic actions
3. **Cache References**: Store manager references instead of repeated lookups

```csharp
// Good
private GameManager gameManager;
public void OnModLoaded()
{
    gameManager = GameManager.Instance;
}

// Bad
public void SomeMethod()
{
    GameManager.Instance.DoSomething(); // Repeated lookup
}
```

### Compatibility

1. **Check for Null**: Always verify managers exist
2. **Version Checking**: Check game version if using version-specific features
3. **Graceful Degradation**: Handle missing features elegantly

```csharp
public void OnModLoaded()
{
    if (GameManager.Instance == null)
    {
        Debug.LogError("GameManager not found! Mod disabled.");
        return;
    }

    if (GameManager.Instance.safetyManager != null)
    {
        // Use safety features
    }
    else
    {
        Debug.LogWarning("Safety manager not available");
    }
}
```

### User Experience

1. **Clear Logging**: Use Debug.Log for important events
2. **Configuration**: Allow users to customize your mod
3. **Documentation**: Include a README with your mod

## 📚 Examples

### Example 1: Budget Booster Mod

```csharp
using UnityEngine;

public class BudgetBoosterMod : IFestivalMod
{
    public string GetModName() => "Budget Booster";
    public string GetModVersion() => "1.0";

    private float bonusAmount = 50000f;

    public void OnModLoaded()
    {
        Debug.Log($"Budget Booster loaded! Adding ${bonusAmount}");
        GameManager.Instance.AddRevenue(bonusAmount);
    }

    public void OnModUnloaded()
    {
        Debug.Log("Budget Booster unloaded");
    }
}
```

### Example 2: Weather Controller Mod

```csharp
using UnityEngine;
using System.Collections;

public class WeatherControllerMod : IFestivalMod
{
    public string GetModName() => "Weather Controller";
    public string GetModVersion() => "1.0";

    public void OnModLoaded()
    {
        Debug.Log("Weather Controller loaded!");
        GameManager.Instance.StartCoroutine(WeatherCycle());
    }

    public void OnModUnloaded()
    {
        Debug.Log("Weather Controller unloaded");
    }

    private IEnumerator WeatherCycle()
    {
        while (GameManager.Instance.festivalActive)
        {
            // Cycle through all weather conditions
            foreach (WeatherCondition condition in System.Enum.GetValues(typeof(WeatherCondition)))
            {
                GameManager.Instance.weatherManager.ForceWeatherChange(condition);
                Debug.Log($"Weather changed to: {condition}");
                yield return new WaitForSeconds(300f); // 5 minutes each
            }
        }
    }
}
```

### Example 3: Auto-Safety Mod

```csharp
using UnityEngine;
using System.Collections;

public class AutoSafetyMod : IFestivalMod
{
    public string GetModName() => "Auto Safety Manager";
    public string GetModVersion() => "1.0";

    public void OnModLoaded()
    {
        Debug.Log("Auto Safety Manager loaded!");
        GameManager.Instance.StartCoroutine(AutoManageSafety());
    }

    public void OnModUnloaded()
    {
        Debug.Log("Auto Safety Manager unloaded");
    }

    private IEnumerator AutoManageSafety()
    {
        while (true)
        {
            var safety = GameManager.Instance.safetyManager;

            // Auto-hire security if understaffed
            if (safety.securityStaffCount < safety.requiredSecurityStaff)
            {
                int needed = safety.requiredSecurityStaff - safety.securityStaffCount;
                safety.HireSecurityStaff(needed);
                Debug.Log($"Auto-hired {needed} security staff");
            }

            yield return new WaitForSeconds(60f); // Check every minute
        }
    }
}
```

### Example 4: JSON Mod - Festival Expansion Pack

```json
{
  "modName": "Festival Expansion Pack",
  "version": "2.0",
  "author": "ModCreator123",
  "customBuildings": [
    {
      "buildingName": "Acoustic Stage",
      "modelPath": "Models/AcousticStage",
      "cost": 30000,
      "capacity": 5000
    },
    {
      "buildingName": "Silent Disco Dome",
      "modelPath": "Models/SilentDisco",
      "cost": 45000,
      "capacity": 3000
    },
    {
      "buildingName": "Chill Zone Tent",
      "modelPath": "Models/ChillZone",
      "cost": 15000,
      "capacity": 1000
    }
  ],
  "customRules": [
    {
      "ruleName": "Quiet Hours",
      "description": "No amplified music after midnight",
      "penaltyAmount": 25000
    },
    {
      "ruleName": "Green Festival",
      "description": "All waste must be separated for recycling",
      "penaltyAmount": 5000
    }
  ],
  "customScenarios": [
    {
      "scenarioName": "Eco Festival",
      "description": "Run a carbon-neutral festival with 30k attendees",
      "targetAttendees": 30000,
      "budget": 800000
    },
    {
      "scenarioName": "Multi-Genre Fest",
      "description": "Satisfy diverse musical tastes with 5 stages",
      "targetAttendees": 75000,
      "budget": 3000000
    }
  ]
}
```

## 🐛 Debugging Mods

### Enable Debug Logs

```csharp
public void OnModLoaded()
{
    Debug.Log($"[{GetModName()}] Loading...");

    // Add detailed logging
    LogSystemStatus();
}

private void LogSystemStatus()
{
    Debug.Log($"[{GetModName()}] GameManager: {GameManager.Instance != null}");
    Debug.Log($"[{GetModName()}] Safety Manager: {GameManager.Instance?.safetyManager != null}");
    Debug.Log($"[{GetModName()}] Current Budget: ${GameManager.Instance?.currentBudget}");
}
```

### Common Issues

1. **Mod Not Loading**: Check file is in `Mods/` folder and has `.dll` extension
2. **NullReferenceException**: Verify managers are initialized before accessing
3. **Compilation Errors**: Ensure you're using compatible Unity version

## 🤝 Sharing Your Mod

When sharing your mod:

1. **Include a README**: Describe features, installation, and requirements
2. **Version Your Mod**: Use semantic versioning (1.0.0, 1.1.0, etc.)
3. **License Your Work**: Choose an appropriate license
4. **Test Thoroughly**: Test with different scenarios and settings

### Mod README Template

```markdown
# My Awesome Festival Mod

Version: 1.0.0
Author: YourName

## Description
This mod adds [describe features]

## Features
- Feature 1
- Feature 2
- Feature 3

## Installation
1. Download MyMod.dll
2. Place in FestivalSimUnity/Mods/
3. Launch Festival Simulator

## Requirements
- Festival Simulator v1.0+
- Unity 2021.3+

## Known Issues
- [List any known issues]

## Changelog
### 1.0.0
- Initial release
```

## 📞 Support & Community

- Report bugs in mod loader on the main GitHub
- Share your mods with the community
- Contribute to mod documentation

---

**Happy Modding! 🎉**

Create amazing festival experiences and share them with the world!
