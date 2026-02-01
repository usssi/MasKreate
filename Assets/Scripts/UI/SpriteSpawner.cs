using UnityEngine;
using UnityEngine.UI;

public class SpriteSpawner : MonoBehaviour
{
    public GameObject spritePrefab;
    public SpriteData itemData;
    public Transform canvas;

    private void Awake()
    {
        // Try to find data in children if not assigned
        if (itemData == null)
        {
            var dataComp = GetComponentInChildren<SpriteDataComponent>();
            if (dataComp != null) itemData = dataComp.data;
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
        rect.localScale = new Vector3(1f, 1f, 1f);

        // Ensure the spawned sprite is interactable
        SpriteTransformer transformer = nuevoSprite.GetComponent<SpriteTransformer>();
        if (transformer != null)
        {
            transformer.isInteractable = true;
        }
    }
}
