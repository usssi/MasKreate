using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour
{
    public static Color colorActual = Color.white;
    public GameObject[] stickersEnUI; // Arrastra aquí los botones de la izquierda
    
    private List<SpriteTransformer> nonInteractableSprites = new List<SpriteTransformer>();

    private void Awake()
    {
        // Buscamos todos los SpriteTransformer en la escena (incluyendo desactivados)
        SpriteTransformer[] allTransformers = FindObjectsByType<SpriteTransformer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (var transformer in allTransformers)
        {
            // Solo agregamos los que no son interactuables
            if (!transformer.isInteractable)
            {
                nonInteractableSprites.Add(transformer);
            }
        }
    }

    public void OnHueChanged(Color nuevoColor) 
    {
        colorActual = nuevoColor;

        // Cambiamos el color SOLO a los iconos de la lista de la izquierda
        foreach(GameObject icono in stickersEnUI) {
            icono.GetComponent<Image>().color = nuevoColor;
        }

        // También cambiamos el color a los SpriteTransformers no interactuables
        foreach (var transformer in nonInteractableSprites)
        {
            // Asumimos que tienen un componente Image o SpriteRenderer
            Image img = transformer.GetComponent<Image>();
            if (img != null)
            {
                img.color = nuevoColor;
            }
            else
            {
                SpriteRenderer sr = transformer.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = nuevoColor;
                }
            }
        }
    }
}
