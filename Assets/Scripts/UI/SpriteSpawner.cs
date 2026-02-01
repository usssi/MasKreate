using UnityEngine;
using UnityEngine.UI;

public class SpriteSpawner : MonoBehaviour
{
    public GameObject spritePrefab;
    public SpriteData itemData;
    public Transform canvas;
    public ObjectSelector objectSelector;

    private void Awake()
    {
        // Try to find data in children if not assigned
        if (itemData == null)
        {
            var dataComp = GetComponentInChildren<SpriteDataComponent>();
            if (dataComp != null) itemData = dataComp.data;
        }

        // Try to find ObjectSelector in scene if not assigned
        if (objectSelector == null)
        {
            objectSelector = FindAnyObjectByType<ObjectSelector>();
        }
    }

    public void SpawnSprite()
    {
        GameObject nuevoSprite = Instantiate(spritePrefab, canvas);
        nuevoSprite.GetComponent<Image>().color = ColorManager.colorActual;
        
        // Initialize with data
        SpriteInitializer initializer = nuevoSprite.GetComponent<SpriteInitializer>();
        if (initializer != null)
        {
            initializer.Initialize(itemData);
        }

        RectTransform rect = nuevoSprite.GetComponent<RectTransform>();
        nuevoSprite.transform.localPosition = Vector3.zero;
        rect.localScale = spritePrefab.transform.localScale;

        // Ensure the spawned sprite is interactable
        SpriteTransformer transformer = nuevoSprite.GetComponent<SpriteTransformer>();
        if (transformer != null)
        {
            transformer.isInteractable = true;
            
            // Auto-select the newly spawned object
            if (objectSelector != null)
            {
                objectSelector.SelectObject(transformer);
            }
        }
    }
}
