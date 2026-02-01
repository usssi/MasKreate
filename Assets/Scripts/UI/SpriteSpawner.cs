using UnityEngine;

public class SpriteSpawner : MonoBehaviour
{
    public GameObject spritePrefab;
    public Transform canvas;

    public void SpawnSprite()
    {
        GameObject nuevoSprite = Instantiate(spritePrefab, canvas);
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
