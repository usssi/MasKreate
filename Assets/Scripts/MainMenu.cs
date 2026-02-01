using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Gallery Settings")]
    public GameObject previewPrefab;
    public Transform galleryContainer;
    public MaskSaveSystem saveSystem;

    private void Start()
    {
        // Only try to populate if we have a container assigned.
        // This prevents errors in scenes where the MainMenu script is just used for buttons.
        if (galleryContainer != null && previewPrefab != null)
        {
            PopulateGallery();
        }
    }

    public void PopulateGallery()
    {
        
        if (saveSystem == null) 
        {
            saveSystem = FindFirstObjectByType<MaskSaveSystem>();
        }

        if (saveSystem == null) 
        { 
            // Silent return if not found - might be a transition or another script instance
            return; 
        }
        if (galleryContainer == null || previewPrefab == null) 
        {
            return; 
        }

        // Clear existing previews
        int clearedCount = 0;
        foreach (Transform child in galleryContainer)
        {
            Destroy(child.gameObject);
            clearedCount++;
        }

        string[] saveFiles = saveSystem.GetAllSaveFiles();

        foreach (string file in saveFiles)
        {
            GameObject previewObj = Instantiate(previewPrefab, galleryContainer);
            MaskPreview preview = previewObj.GetComponent<MaskPreview>();
            if (preview != null)
            {
                preview.Initialize(file, saveSystem);
            }
            else
            {
                Debug.LogError("MainMenu: Instantiated preview prefab is missing MaskPreview component!");
            }
        }
    }

   public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GoBack()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void Quit()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
