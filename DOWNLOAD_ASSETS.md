# Festival Simulator - Asset Download Guide

## 🎨 Free Assets - Quick Setup (10 Minutes)

### Kenney.nl Free Assets (Recommended for Instant Start)

#### Step 1: Download These Packs

1. **City Kit** - Buildings, infrastructure, stages
   - URL: https://kenney.nl/assets/city-kit-commercial
   - Download: "City Kit Commercial" (Direct Download)
   - Size: ~50MB

2. **Music Kit** - Instruments, speakers, audio equipment
   - URL: https://kenney.nl/assets/music-kit
   - Download: Direct download
   - Size: ~20MB

3. **Food Kit** - Food vendors, carts, props
   - URL: https://kenney.nl/assets/food-kit
   - Download: Direct download
   - Size: ~15MB

4. **Furniture Kit** - Tables, chairs, barriers
   - URL: https://kenney.nl/assets/furniture-kit
   - Download: Direct download
   - Size: ~30MB

5. **Nature Kit** - Trees, rocks, terrain
   - URL: https://kenney.nl/assets/nature-kit
   - Download: Direct download
   - Size: ~40MB

**Total Download Size: ~155MB**
**License: CC0 (Public Domain) - Use freely!**

#### Step 2: Extract and Organize

```bash
# Create assets folder
mkdir FestivalSimUnity/DownloadedAssets

# Extract all zips to:
FestivalSimUnity/DownloadedAssets/
├── KenneyCityKit/
├── KenneyMusicKit/
├── KenneyFoodKit/
├── KenneyFurnitureKit/
└── KenneyNatureKit/
```

#### Step 3: Import to Unity

1. Open Unity project
2. In Unity: `Assets > Import Package > Custom Package`
3. For each kit:
   - Select the `.fbx` or `.obj` files
   - Import to `Assets/Models/Kenney/[KitName]/`

**OR use drag-and-drop:**
- Drag extracted folders directly into Unity's `Assets/Models/` folder

### Quaternius Free Models (Alternative/Additional)

#### Ultimate Low Poly Pack
- URL: https://quaternius.com/packs/ultimatelowpoly.html
- Download: Direct download
- Size: ~200MB
- Contains: 1000+ models including characters, props, nature

**Import same way as Kenney assets**

## 💎 Premium Assets - Synty Studios (Cities: Skylines Look)

### Recommended Synty Packs

#### Option 1: POLYGON Concert Pack ($29.95)
- URL: https://assetstore.unity.com/packages/3d/environments/polygon-concert-low-poly-3d-art-by-synty-185056
- **Perfect for festivals!** Includes:
  - Stage setups (main, secondary)
  - Crowd barriers
  - Speakers and lights
  - Concert props
  - Crowd characters

#### Option 2: POLYGON City Pack ($39.95)
- URL: https://assetstore.unity.com/packages/3d/environments/urban/polygon-city-low-poly-3d-art-by-synty-95214
- Includes:
  - Buildings
  - Infrastructure
  - Vehicles
  - Props

#### Option 3: Both Packs ($69.90 - Best Value)
- Get complete festival + city environment
- Consistent art style
- Professional quality

### How to Buy & Import Synty

1. Visit Unity Asset Store links above
2. Add to cart, purchase
3. In Unity: `Window > Package Manager`
4. Select "My Assets"
5. Find POLYGON pack
6. Click "Download" then "Import"

## 🎯 Quick Import Instructions

### Method 1: Drag and Drop (Fastest)

1. Download and extract Kenney packs
2. Open Unity project
3. Drag folders into `Assets/Models/`
4. Unity auto-imports!

### Method 2: Import Package (Cleaner)

```
Unity Menu:
Assets > Import Package > Custom Package
- Select downloaded files
- Choose import location
- Click "Import"
```

### Method 3: Script-Based (Advanced)

Create `Assets/Editor/AssetImporter.cs`:

```csharp
using UnityEditor;
using UnityEngine;
using System.IO;

public class AssetImporter : EditorWindow
{
    [MenuItem("Festival/Import Kenney Assets")]
    static void ImportKenneyAssets()
    {
        string sourcePath = "C:/Users/jamie/FestivalSimUnity/DownloadedAssets/";
        string targetPath = "Assets/Models/Kenney/";

        if (Directory.Exists(sourcePath))
        {
            FileUtil.CopyFileOrDirectory(sourcePath, targetPath);
            AssetDatabase.Refresh();
            Debug.Log("Kenney assets imported!");
        }
        else
        {
            Debug.LogError("Source path not found: " + sourcePath);
        }
    }
}
```

## 📦 Post-Import Setup

### Create Prefabs from Models

1. **Navigate to imported models**
2. **Drag model to scene**
3. **Add components:**
   - Box Collider (for interactions)
   - Custom scripts (if needed)
4. **Drag back to Project** (creates prefab)
5. **Delete from scene**

### Example: Create Stage Prefab

```csharp
// Assets/Editor/PrefabCreator.cs
using UnityEditor;
using UnityEngine;

public class PrefabCreator
{
    [MenuItem("Festival/Create Stage Prefabs")]
    static void CreateStagePrefabs()
    {
        // Find stage model
        GameObject stageModel = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Models/Kenney/CityKit/stage.fbx"
        );

        if (stageModel != null)
        {
            // Instantiate
            GameObject stage = GameObject.Instantiate(stageModel);
            stage.name = "MainStagePrefab";

            // Add collider
            stage.AddComponent<BoxCollider>();

            // Add custom component
            stage.AddComponent<StageController>();

            // Save as prefab
            PrefabUtility.SaveAsPrefabAsset(stage, "Assets/Prefabs/MainStage.prefab");
            GameObject.DestroyImmediate(stage);

            Debug.Log("Stage prefab created!");
        }
    }
}
```

## 🚀 Build Your First Scene

### 5-Minute Scene Setup

1. **Create Terrain**
   - `GameObject > 3D Object > Terrain`
   - Paint grass texture

2. **Add Main Stage**
   - Drag Kenney stage model to center
   - Position at (0, 0, 0)

3. **Add Food Area**
   - Drag food cart models
   - Arrange in a row

4. **Add Barriers**
   - Drag fence/barrier models
   - Create crowd control zones

5. **Add Trees**
   - Paint trees on terrain edges

6. **Test**
   - Press Play
   - Watch systems initialize!

### Scene Layout Template

```
Festival Ground Layout:
- Main Stage: (0, 0, 0)
- Second Stage: (50, 0, 50)
- Food Court: (-30, 0, 20)
  - Cart 1: (-30, 0, 20)
  - Cart 2: (-25, 0, 20)
  - Cart 3: (-20, 0, 20)
- Bar Area: (30, 0, 20)
- Medical Tent: (-50, 0, -20)
- Entrance Gates: (0, 0, -60)
- Parking: (0, 0, -100)
```

## 🔗 Direct Download Links

### Kenney Assets (Free)

```bash
# Quick download commands (PowerShell)

# City Kit
Invoke-WebRequest -Uri "https://kenney.nl/content