using UnityEngine;
using UnityEngine.UI;

public class GlobalColorManager : MonoBehaviour
{
    public Material material;

    public void OnHueChanged(Color nuevoColor)
    {
        material.SetColor("_Color", nuevoColor);
    }
   
    void Start()
    {
        material.SetColor("_Color", Color.white);
    }
}
