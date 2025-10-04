using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class ModLoader : MonoBehaviour
{
    public static ModLoader Instance { get; private set; }

    [Header("Mod Settings")]
    public string modsFolder = "Mods";
    public bool enableMods = true;

    private List<IFestivalMod> loadedMods = new List<IFestivalMod>();
    private string modsPath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        modsPath = Path.Combine(Application.dataPath, "..", modsFolder);

        if (enableMods)
        {
            LoadMods();
        }
    }

    public void LoadMods()
    {
        if (!Directory.Exists(modsPath))
        {
            Directory.CreateDirectory(modsPath);
            Debug.Log($"Created mods directory at: {modsPath}");
            CreateModReadme();
            return;
        }

        Debug.Log($"Loading mods from: {modsPath}");

        try
        {
            // Load DLL mods
            string[] dllFiles = Directory.GetFiles(modsPath, "*.dll", SearchOption.AllDirectories);

            foreach (string dllPath in dllFiles)
            {
                try
                {
                    LoadModFromDLL(dllPath);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load mod from {dllPath}: {e.Message}");
                }
            }

            // Load JSON configuration mods
            string[] jsonFiles = Directory.GetFiles(modsPath, "*.json", SearchOption.AllDirectories);

            foreach (string jsonPath in jsonFiles)
            {
                try
                {
                    LoadModFromJSON(jsonPath);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load mod config from {jsonPath}: {e.Message}");
                }
            }

            Debug.Log($"Successfully loaded {loadedMods.Count} mods");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading mods: {e.Message}");
        }
    }

    private void LoadModFromDLL(string dllPath)
    {
        Assembly modAssembly = Assembly.LoadFrom(dllPath);

        foreach (Type type in modAssembly.GetTypes())
        {
            if (typeof(IFestivalMod).IsAssignableFrom(type) && !type.IsInterface)
            {
                IFestivalMod mod = (IFestivalMod)Activator.CreateInstance(type);
                mod.OnModLoaded();
                loadedMods.Add(mod);
                Debug.Log($"Loaded mod: {mod.GetModName()} v{mod.GetModVersion()}");
            }
        }
    }

    private void LoadModFromJSON(string jsonPath)
    {
        string jsonContent = File.ReadAllText(jsonPath);
        ModConfig config = JsonUtility.FromJson<ModConfig>(jsonContent);

        if (config != null)
        {
            Debug.Log($"Loaded mod configuration: {config.modName}");
            // Apply configuration (custom buildings, rules, etc.)
            ApplyModConfig(config);
        }
    }

    private void ApplyModConfig(ModConfig config)
    {
        // Apply custom mod settings to game
        if (config.customBuildings != null)
        {
            Debug.Log($"Registering {config.customBuildings.Length} custom buildings from {config.modName}");
        }

        if (config.customRules != null)
        {
            Debug.Log($"Applying {config.customRules.Length} custom rules from {config.modName}");
        }
    }

    private void CreateModReadme()
    {
        string readmePath = Path.Combine(modsPath, "README.txt");
        string readmeContent = @"Festival Simulator - Mods Folder
================================

Place your mod files (.dll or .json) in this directory.

For DLL Mods:
- Create a class that implements IFestivalMod interface
- Compile to .dll and place here

For JSON Mods:
- Create a JSON file with ModConfig structure
- Define custom buildings, rules, scenarios

See MODDING.md in the Docs folder for full documentation.
";
        File.WriteAllText(readmePath, readmeContent);
    }

    public List<IFestivalMod> GetLoadedMods()
    {
        return new List<IFestivalMod>(loadedMods);
    }
}

// Mod interface
public interface IFestivalMod
{
    string GetModName();
    string GetModVersion();
    void OnModLoaded();
    void OnModUnloaded();
}

// Mod configuration structure
[Serializable]
public class ModConfig
{
    public string modName;
    public string version;
    public string author;
    public CustomBuilding[] customBuildings;
    public CustomRule[] customRules;
    public CustomScenario[] customScenarios;
}

[Serializable]
public class CustomBuilding
{
    public string buildingName;
    public string modelPath;
    public float cost;
    public int capacity;
}

[Serializable]
public class CustomRule
{
    public string ruleName;
    public string description;
    public float penaltyAmount;
}

[Serializable]
public class CustomScenario
{
    public string scenarioName;
    public string description;
    public int targetAttendees;
    public float budget;
}
