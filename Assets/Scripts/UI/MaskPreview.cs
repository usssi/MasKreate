using UnityEngine;
using System.IO;

public class MaskPreview : MonoBehaviour
{
    public Transform previewCanvas;
    private MaskSaveSystem saveSystem;

    public void Initialize(string filePath, MaskSaveSystem system)
    {
        Debug.Log($"MaskPreview: Initializing with {filePath}");
        saveSystem = system;
        if (previewCanvas == null)
        {
            Debug.LogError("MaskPreview: previewCanvas is NULL! Re-assign in Inspector.");
            return;
        }

        if (saveSystem != null)
        {
            saveSystem.LoadDesignByPath(filePath, previewCanvas, false);
        }
        else
        {
            Debug.LogError("MaskPreview: saveSystem provided to Initialize is NULL!");
        }
    }
}
