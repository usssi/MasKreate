using UnityEngine;

public class SpriteTransformer : MonoBehaviour
{
    [Tooltip("Percentage offset for width (e.g. 0 = 100% of base size)")]
    public float width = 0f; 
    [Tooltip("Percentage offset for height")]
    public float height = 0f; 
    public float scale = 1f; // Default scale
    public float zRotation;
    public bool isInteractable = true; // Flag to enable/disable interaction

    [Header("Base Dimensions (Auto-captured)")]
    public float baseWidth;
    public float baseHeight;

    public float CurrentWidth => baseWidth * (1 + width / 100f);
    public float CurrentHeight => baseHeight * (1 + height / 100f);

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (baseWidth == 0 || baseHeight == 0)
        {
            baseWidth = rectTransform.sizeDelta.x;
            baseHeight = rectTransform.sizeDelta.y;
        }
    }

    private void Update()
    {
        UpdateTransform();
    }

    private void UpdateTransform()
    {
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(CurrentWidth, CurrentHeight);
            rectTransform.localScale = Vector3.one * scale; // Uniform scale
            
            // Preserving X and Y rotation, only modifying Z
            Vector3 currentEuler = rectTransform.localEulerAngles;
            rectTransform.localEulerAngles = new Vector3(currentEuler.x, currentEuler.y, zRotation);
        }
    }

    private void Reset()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            baseWidth = rectTransform.sizeDelta.x;
            baseHeight = rectTransform.sizeDelta.y;
            width = 0;
            height = 0;
            scale = rectTransform.localScale.x;
            zRotation = rectTransform.localEulerAngles.z;
        }
        isInteractable = true;
    }
}
