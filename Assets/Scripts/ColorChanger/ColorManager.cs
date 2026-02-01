using UnityEngine;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour
{
    public static Color colorActual = Color.white;
    public GameObject[] stickersEnUI; // Arrastra aquí los botones de la izquierda

    public void OnHueChanged(Color nuevoColor) 
    {
        colorActual = nuevoColor;

        // Cambiamos el color SOLO a los iconos de la lista de la izquierda
        foreach(GameObject icono in stickersEnUI) {
            icono.GetComponent<Image>().color = nuevoColor;
        }
    }
}
