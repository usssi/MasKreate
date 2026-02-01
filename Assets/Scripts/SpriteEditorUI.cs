using UnityEngine;
using UnityEngine.UI;
using TMPro;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpriteEditorUI : MonoBehaviour
{
    [Header("UI References")]
    // Sliders removed as interaction is now gizmo-based

    [Header("UI Feedback (Optional)")]
    public TextMeshProUGUI widthValueText;
    public TextMeshProUGUI heightValueText;
    public TextMeshProUGUI scaleValueText;
    public TextMeshProUGUI rotationValueText;

    [Header("Width Settings")]
    public float widthStep = 10f;
    public float minWidth = -100f;
    public float maxWidth = 100f;

    [Header("Height Settings")]
    public float heightStep = 10f;
    public float minHeight = -100f;
    public float maxHeight = 100f;

    [Header("Scale Settings")]
    public float scaleStep = 0.1f;
    public float minScale = 0.1f;
    public float maxScale = 5.0f;

    [Header("Rotation Settings")]
    public float rotationStep = 15f;
    public float minRotation = -180f;
    public float maxRotation = 180f;

    private SpriteTransformer currentTarget;
    

    private void Start()
    {
        // Sliders removed
    }

    public void SetTarget(SpriteTransformer target)
    {
        currentTarget = target;
        if (currentTarget != null)
        {
            // Update Text
            UpdateTextLabels(currentTarget.width, currentTarget.height, currentTarget.scale, currentTarget.zRotation);
        }
    }


    // --- Gizmo Support ---

    public void UpdateFromGizmoWidth(float totalWidth)
    {
        if (currentTarget == null || currentTarget.baseWidth == 0) return;
        
        // Convert total absolute width back to percentage offset
        float offset = ((totalWidth / currentTarget.baseWidth) - 1f) * 100f;
        float finalVal = RoundToStep(Mathf.Clamp(offset, minWidth, maxWidth), widthStep);
        
        currentTarget.width = finalVal;
        if (widthValueText) widthValueText.text = finalVal.ToString("F0");
    }

    public void UpdateFromGizmoHeight(float totalHeight)
    {
        if (currentTarget == null || currentTarget.baseHeight == 0) return;
        
        // Convert total absolute height back to percentage offset
        float offset = ((totalHeight / currentTarget.baseHeight) - 1f) * 100f;
        float finalVal = RoundToStep(Mathf.Clamp(offset, minHeight, maxHeight), heightStep);
        
        currentTarget.height = finalVal;
        if (heightValueText) heightValueText.text = finalVal.ToString("F0");
    }

    public void UpdateFromGizmoScale(float totalScale)
    {
        if (currentTarget == null) return;
        float finalVal = RoundToStep(Mathf.Clamp(totalScale, minScale, maxScale), scaleStep);
        currentTarget.scale = finalVal;
        if (scaleValueText) scaleValueText.text = finalVal.ToString("F2");
    }

    public void UpdateFromGizmoRotation(float totalRotation)
    {
        if (currentTarget == null) return;
        float finalVal = RoundToStep(Mathf.Clamp(totalRotation, minRotation, maxRotation), rotationStep);
        currentTarget.zRotation = finalVal;
        if (rotationValueText) rotationValueText.text = finalVal.ToString("F0");
    }

    private float RoundToStep(float value, float step)
    {
        if (step <= 0) return value;
        return Mathf.Round(value / step) * step;
    }

    private void UpdateTextLabels(float w, float h, float s, float r)
    {
        if (widthValueText) widthValueText.text = w.ToString("F0");
        if (heightValueText) heightValueText.text = h.ToString("F0");
        if (scaleValueText) scaleValueText.text = s.ToString("F2");
        if (rotationValueText) rotationValueText.text = r.ToString("F0");
    }
}
