using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokeball : MonoBehaviour, Interactable
{
    [SerializeField] ItemBase item;

    public bool Used { get; set; } = false;
    public IEnumerator Interact(Transform initiator)
    {
        if (!Used)
        {
            initiator.GetComponent<Inventory>().AddItem(item);
            Used = true;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            
            yield return DialogManager.Instance.ShowDialogText($"Vous avez trouver {item.name}");
        }
        //yield return DialogManager.Instance.ShowDialogText("pick up is working");
    }
}
