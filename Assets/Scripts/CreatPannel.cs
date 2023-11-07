using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelCreator : MonoBehaviour
{
    void Start()
    {
        // Créer un nouveau GameObject pour le panneau
        GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Quad);

        // Définir la position du panneau
        panel.transform.position = new Vector3(0, 0, 0); // Changer les coordonnées XYZ selon vos besoins

        // Définir la taille du panneau
        panel.transform.localScale = new Vector3(2, 1, 1); // Changer la taille selon vos besoins

        // Changer la couleur du panneau
        Renderer panelRenderer = panel.GetComponent<Renderer>();
        if (panelRenderer != null)
        {
            panelRenderer.material.color = Color.white; // Changer la couleur du panneau selon vos besoins
        }

        // Ajouter un script ou un comportement au panneau si nécessaire
        panel.AddComponent<PanelCreator>(); // Remplacez YourCustomScript par le nom de votre propre script
    }
}

