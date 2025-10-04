# Festival Simulator - Unity Edition

A comprehensive, modular festival simulation game inspired by Cities: Skylines and theme park management games. Built in Unity for PC (Windows/Mac/Linux) with full modding support.

## 🎪 Overview

Manage every aspect of a music festival - from safety and medical services to crowd control, weather management, and logistics. Balance attendee satisfaction, safety ratings, and budget to run the perfect festival.

## ✨ Key Features

### Safety & Security
- Real-time incident dashboard
- Security staff planning and certification tracking
- Barrier load calculations and crowd control
- Lightning detection and emergency shutdown protocols
- Lost child protocol and reunification system
- Confiscation and amnesty bin management
- Emergency route planning and monitoring

### Medical Services
- Medical staff certification tracking (First Aiders & Paramedics)
- Naloxone kit inventory and administration logging
- First aid kit management
- Ambulance dispatch system
- Food safety inspections and violation tracking
- Medical facility capacity monitoring
- Multi-severity patient treatment system

### Crowd Management
- Multi-zone crowd density tracking
- Queue management for bars, food, and facilities
- Crowd mood and behavior simulation
- Entry/exit gate flow control
- Crowd crush risk detection and mitigation
- Dynamic capacity management

### Weather System
- Real-time weather conditions with 24-hour forecasting
- Lightning proximity detection
- Automated stage shutdown protocols
- Slip risk index calculation
- Weather impact on crowd mood and safety

### Logistics & Operations
- Waste management with bin capacity tracking
- Water supply monitoring
- Power generation and fuel management
- Food and beverage stock levels
- Parking and transport coordination
- Toilet cleaning schedules

## 🎮 Getting Started

### Prerequisites
- Unity 2021.3 LTS or newer
- Windows/Mac/Linux

### Installation

1. Clone or download this repository
2. Open the project in Unity
3. Open the `FestivalMap` scene in `Assets/Scenes/`
4. Press Play to start the simulation

### Quick Start

```csharp
// The main game is controlled through GameManager.Instance
GameManager.Instance.StartFestival();

// Hire security staff
GameManager.Instance.safetyManager.HireSecurityStaff(20);

// Add medical resources
GameManager.Instance.medicalManager.RestockSupplies("naloxone", 10);

// Simulate crowd movement
GameManager.Instance.crowdManager.SetZoneOccupancy("Main Stage", 5000);

// Change weather
GameManager.Instance.weatherManager.ForceWeatherChange(WeatherCondition.Storm);
```

## 📊 Core Systems

### Game Manager
Central hub connecting all systems. Manages:
- Festival state (active/inactive)
- Budget and revenue
- Time progression
- System initialization

### Safety Manager
- Incident logging and reporting
- Barrier integrity monitoring
- Lightning emergency protocols
- Lost children handling
- Security staffing calculations

### Medical Manager
- Patient treatment queue
- Resource inventory (kits, staff, ambulances)
- Food safety compliance
- Capacity calculations

### Crowd Manager
- Zone-based occupancy tracking
- Queue time optimization
- Crowd mood calculations
- Safety threshold enforcement

### Weather Manager
- Dynamic weather progression
- Lightning detection system
- Safety impact assessment
- Forecast generation

### Logistics Manager
- Waste collection and disposal
- Power generation and monitoring
- Food/drink inventory
- Water supply tracking
- Parking management

## 🔧 Modding Support

This simulator includes a complete modding system! See [MODDING.md](Docs/MODDING.md) for full documentation.

### Quick Mod Example

```csharp
public class MyCustomMod : IFestivalMod
{
    public string GetModName() => "My Festival Mod";
    public string GetModVersion() => "1.0";

    public void OnModLoaded()
    {
        Debug.Log("Custom mod loaded!");
        // Add your custom features here
    }

    public void OnModUnloaded()
    {
        Debug.Log("Custom mod unloaded!");
    }
}
```

Place compiled DLLs in the `Mods/` folder to auto-load.

## 🎨 Recommended Assets

This project works great with low-poly art styles:

- **Synty Studios** - POLYGON series (Concert, City, Modular Fantasy)
- **Kenney Assets** - Free game assets (kenney.nl)
- **Quaternius** - Ultimate Low-Poly Collection

These assets provide the colorful, clean aesthetic similar to Cities: Skylines.

## 📁 Project Structure

```
FestivalSimUnity/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/              # GameManager, ModLoader
│   │   ├── Features/          # Safety, Medical, Crowd, Weather, Logistics
│   │   ├── UI/               # Panels, Dashboards, Menus
│   │   └── Data/             # Data structures and configs
│   ├── Prefabs/              # UI and game object prefabs
│   ├── Models/               # 3D models
│   ├── Textures/             # Textures and materials
│   ├── Audio/                # Sound effects and music
│   └── Scenes/               # Unity scenes
├── Mods/                     # User mods folder
└── Docs/                     # Documentation
```

## 🎯 Must-Have Features Checklist

See [FEATURES.md](Docs/FEATURES.md) for the complete feature list, including:

- ✅ Incident dashboard with filtering
- ✅ Medical staff certification tracking
- ✅ Naloxone kit logging
- ✅ Slip-risk index calculation
- ✅ Lightning PA override system
- ✅ Barrier load calculator
- ✅ Confiscation/amnesty bin tracking
- ✅ Food safety inspection logs
- ✅ Non-compliance fine system
- ✅ Emergency route planning
- ✅ Lost child protocol
- ✅ First-aid kit inventory
- ✅ Security staff planner
- And many more!

## 🚀 Roadmap

### Version 1.0 (Current)
- Core simulation systems
- Basic UI
- Modding framework

### Version 1.1 (Planned)
- Advanced analytics dashboard
- Historical data tracking
- Save/load system
- Scenario challenges

### Version 1.2 (Planned)
- Multiplayer/co-op mode
- Workshop integration
- Advanced weather patterns
- Dynamic event system

### Version 2.0 (Future)
- Full 3D graphics with custom shaders
- VR support
- AI-driven crowd behavior
- Procedural festival generation

## 🤝 Contributing

Mods and add-ons are encouraged! Check the modding documentation to get started.

## 📝 License

This project is open for educational and personal use. If you create mods or add-ons, please credit the original project.

## 🐛 Known Issues

- Weather forecast may show rapid transitions (realistic weather modeling in progress)
- Very high attendee counts (>100k) may impact performance
- Some UI elements require prefab setup in Unity Editor

## 💡 Tips for Festival Success

1. **Always hire enough medical staff** - 1 per 1000 attendees minimum
2. **Monitor lightning distance** - Shutdown stages when <10km
3. **Keep naloxone kits stocked** - Critical for emergencies
4. **Watch crowd density** - Close entries before 4 people/m²
5. **Manage queues** - Long waits kill crowd mood
6. **Stock food/drinks early** - Resupply takes time
7. **Empty bins regularly** - Full bins = unhappy crowds
8. **Check power capacity** - Outages are festival killers

## 📞 Support

For questions, suggestions, or bug reports:
- Create an issue on GitHub
- Check MODDING.md for mod development help
- See FEATURES.md for complete feature documentation

---

**🎵 Good luck running your festival! 🎵**
