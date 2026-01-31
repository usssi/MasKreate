using UnityEngine;

public class TabManager : MonoBehaviour
{
    public GameObject[] paneles;
    public GameObject [] images;

    public void MostrarPanel(int indice)
    {
        for (int i = 0; i < paneles.Length; i++)
        {
            paneles[i].SetActive(false);
            images[i].SetActive(false);
        }

        if (indice >= 0 && indice < paneles.Length)
        {
            paneles[indice].SetActive(true);
            images[indice].SetActive(true);
        }
    }
}
