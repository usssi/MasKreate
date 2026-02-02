using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MaskSaveSystem : MonoBehaviour
{
    public Transform bgCanvas;
    public GameObject spritePrefab;
    public ObjectSelector objectSelector;

    private string SavesFolder => Path.Combine(Application.persistentDataPath, "Saves");
    
    // For the editor, we still might want a "latest" or "target" file
    private string DefaultSavePath => Path.Combine(SavesFolder, "mask_design.json");

    private void Awake()
    {
        if (!Directory.Exists(SavesFolder))
        {
            Directory.CreateDirectory(SavesFolder);
        }
    }

    public bool SaveDesign()
    {
        if (bgCanvas == null) 
        {
            Debug.LogError("MaskSaveSystem: Cannot Save! bgCanvas is not assigned. This is normal in Main Menu, but an error in the Mask Editor scene.");
            return false;
        }

        // Find next incremental name
        string fileName = GetNextIncrementalName();
        string fullPath = Path.Combine(SavesFolder, fileName);

        MaskDesignData designData = CollectDesignData();
        
        // Prevent saving empty masks
        if (designData.elements == null || designData.elements.Count == 0)
        {
            Debug.LogWarning("MaskSaveSystem: Design is empty! Aborting save.");
            return false;
        }

        string json = JsonUtility.ToJson(designData, true);
        File.WriteAllText(fullPath, json);
        
        // Also save as "latest" for easy loading in editor if needed
        File.WriteAllText(DefaultSavePath, json);
        
        return true;
    }

    private string GetNextIncrementalName()
    {
        int index = 1;
        string fileName;
        do
        {
            fileName = $"Mask_{index}.json";
            index++;
        } while (File.Exists(Path.Combine(SavesFolder, fileName)));
        
        return fileName;
    }

    private MaskDesignData CollectDesignData()
    {
        MaskDesignData designData = new MaskDesignData();
        int childCount = bgCanvas.childCount;
        int collectedCount = 0;

        foreach (Transform child in bgCanvas)
        {
            SpriteTransformer transformer = child.GetComponent<SpriteTransformer>();
            SpriteDataComponent dataComp = child.GetComponent<SpriteDataComponent>();
            Image image = child.GetComponent<Image>();

            if (transformer != null && dataComp != null && dataComp.data != null)
            {
                MaskElementData element = new MaskElementData
                {
                    spriteDataName = dataComp.data.itemName,
                    localPosition = child.localPosition,
                    widthOffset = transformer.width,
                    heightOffset = transformer.height,
                    scale = transformer.scale,
                    zRotation = transformer.zRotation,
                    color = image != null ? image.color : Color.white
                };
                designData.elements.Add(element);
                collectedCount++;
            }
            else
            {
                Debug.LogWarning($"MaskSaveSystem: Skipping child {child.name}. Reason: Transformer={transformer != null}, DataComp={dataComp != null}, Data={(dataComp != null ? dataComp.data != null : "N/A")}");
            }
        }
        return designData;
    }

    public void LoadDesign()
    {
        LoadDesignByPath(DefaultSavePath, bgCanvas, true);
    }

    public void LoadDesignByPath(string path, Transform targetCanvas, bool isEditor = false)
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning($"MaskSaveSystem: No save file found at {path}");
            return;
        }

        if (targetCanvas == null)
        {
            Debug.LogError("MaskSaveSystem: targetCanvas is NULL!");
            return;
        }

        if (spritePrefab == null)
        {
            Debug.LogError("MaskSaveSystem: spritePrefab is NULL!");
            return;
        }

        if (isEditor)
        {
            PrepareEditorForLoad();
        }

        // Clear existing children in target
        int clearedCount = 0;
        foreach (Transform child in targetCanvas)
        {
            Destroy(child.gameObject);
            clearedCount++;
        }

        string json = File.ReadAllText(path);
        MaskDesignData designData = JsonUtility.FromJson<MaskDesignData>(json);

        if (designData == null || designData.elements == null)
        {
            Debug.LogError($"MaskSaveSystem: Failed to deserialize MaskDesignData from {path}");
            return;
        }


        foreach (var elementData in designData.elements)
        {
            if (SpriteDataRegistry.Instance == null)
            {
                Debug.LogError("MaskSaveSystem: SpriteDataRegistry.Instance is NULL! Is it missing from the scene?");
                break;
            }

            SpriteData spriteAsset = SpriteDataRegistry.Instance.GetSpriteData(elementData.spriteDataName);
            if (spriteAsset == null)
            {
                Debug.LogError($"MaskSaveSystem: Could not find SpriteData for {elementData.spriteDataName}");
                continue;
            }

            GameObject newSprite = Instantiate(spritePrefab, targetCanvas);
            newSprite.transform.localPosition = elementData.localPosition;

            // Initialize components
            SpriteInitializer initializer = newSprite.GetComponent<SpriteInitializer>();
            if (initializer != null) 
            {
                initializer.Initialize(spriteAsset);
            }
            else
            {
                Debug.LogWarning($"MaskSaveSystem: SpriteInitializer missing on prefab for {elementData.spriteDataName}");
            }

            Image image = newSprite.GetComponent<Image>();
            if (image != null) image.color = elementData.color;

            SpriteTransformer transformer = newSprite.GetComponent<SpriteTransformer>();
            if (transformer != null)
            {
                transformer.width = elementData.widthOffset;
                transformer.height = elementData.heightOffset;
                transformer.scale = elementData.scale;
                transformer.zRotation = elementData.zRotation;
                transformer.isInteractable = isEditor;
            }
        }
    }

    private void PrepareEditorForLoad()
    {
        // Force a deselect in the Unity Editor to prevent Inspector crashes
#if UNITY_EDITOR
        UnityEditor.Selection.activeGameObject = null;
#endif

        // Clear selection in our system
        if (objectSelector != null) 
        {
            objectSelector.SelectObject(null);
        }
        else
        {
            Debug.LogWarning("MaskSaveSystem: ObjectSelector not assigned! Selection might not clear properly.");
        }
    }

    public string[] GetAllSaveFiles()
    {
        if (!Directory.Exists(SavesFolder))
        {
             Debug.LogWarning($"MaskSaveSystem: Saves folder does not exist at {SavesFolder}");
             return new string[0];
        }
        
        // Get all .json files in the Saves folder
        string[] allFiles = Directory.GetFiles(SavesFolder, "*.json");
        
        List<string> filtered = new List<string>();
        foreach(string f in allFiles)
        {
            string fileName = Path.GetFileName(f);
            // We only want Mask_1.json, Mask_2.json, etc. 
            // We skip "mask_design.json" because you said it was looking empty.
            if (fileName.StartsWith("Mask_") && fileName.EndsWith(".json"))
            {
                filtered.Add(f);
            }
        }

        return filtered.ToArray();
    }
}
