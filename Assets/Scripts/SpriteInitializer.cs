using UnityEngine;
using UnityEngine.UI;

public class SpriteInitializer : MonoBehaviour
{
    private Image image;
    private SpriteTransformer transformer;

    private void Awake()
    {
        image = GetComponent<Image>();
        transformer = GetComponent<SpriteTransformer>();
    }

    private void Start()
    {
        // For UI Previews: If we are a child of a SpriteSpawner, sync with its data automatically
        SpriteSpawner parentSpawner = GetComponentInParent<SpriteSpawner>();
        if (parentSpawner != null && parentSpawner.itemData != null)
        {
            Initialize(parentSpawner.itemData);
        }
    }

    public void Initialize(SpriteData data)
    {
        if (data == null) return;

        if (image == null) image = GetComponent<Image>();
        if (transformer == null) transformer = GetComponent<SpriteTransformer>();

        // Ensure SpriteDataComponent is also populated for the save system
        SpriteDataComponent dataComp = GetComponent<SpriteDataComponent>();
        if (dataComp == null) dataComp = gameObject.AddComponent<SpriteDataComponent>();
        dataComp.data = data;

        if (image != null && data.visual != null)
        {
            image.sprite = data.visual;
            // Use native size of the new sprite
            image.SetNativeSize();
        }

        if (transformer != null)
        {
            transformer.CaptureNativeSize();
        }
    }
}
