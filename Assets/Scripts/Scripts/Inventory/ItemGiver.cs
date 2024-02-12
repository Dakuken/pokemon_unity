using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : MonoBehaviour
{
   [SerializeField] ItemBase item;
   [SerializeField] int count = 1;
   [SerializeField] Dialog dialog;

   private bool used = false;
   
   public IEnumerator GiveItem(PlayerController player)
   {
      yield return DialogManager.Instance.ShowDialog(dialog);
      
      player.GetComponent<Inventory>().AddItem(item, count);

      used = true;
      string dialogText = $"Vous avez reçu {item.name}";
      if (count > 1)
         dialogText = $"Vous avez reçu {count} {item.name}s";
      
      yield return DialogManager.Instance.ShowDialogText(dialogText);
   }

   public bool CanBeGiven()
   {
      return item != null && count > 0 && !used;
   }
}
