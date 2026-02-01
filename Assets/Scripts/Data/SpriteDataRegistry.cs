using UnityEngine;
using System.Collections.Generic;

public class SpriteDataRegistry : MonoBehaviour
{
    private static SpriteDataRegistry _instance;
    public static SpriteDataRegistry Instance 
    { 
        get 
        {
            if (_instance == null) _instance = FindFirstObjectByType<SpriteDataRegistry>();
            return _instance; 
        }
        private set => _instance = value;
    }

    public List<SpriteData> allSpriteData;
    private Dictionary<string, SpriteData> dataMap = new Dictionary<string, SpriteData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeMap();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeMap()
    {
        dataMap.Clear();
        foreach (var data in allSpriteData)
        {
            if (data != null && !dataMap.ContainsKey(data.itemName))
            {
                dataMap.Add(data.itemName, data);
            }
        }
    }

    public SpriteData GetSpriteData(string itemName)
    {
        if (dataMap.Count == 0 && allSpriteData.Count > 0) InitializeMap();
        
        if (dataMap.TryGetValue(itemName, out SpriteData data))
        {
            return data;
        }
        return null;
    }
}
