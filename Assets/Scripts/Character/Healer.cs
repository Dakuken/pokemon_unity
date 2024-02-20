using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public IEnumerator Heal(Transform player, Dialog dialog)
    {
        int SelectedChoice = 0;
        
        yield return DialogManager.Instance.ShowDialogText("Voulez-vous que je soigne vos pokemon ?",
            choices : new List<string>() {"Oui, merci " ,"Non merci"},
            onChoiceSelected:(choiceIndex) => SelectedChoice = choiceIndex);

        if (SelectedChoice == 0)
        {
            yield return Fader.i.FadeIn(0.5f);
            
            var playerParty = player.GetComponent<PokemonParty>();
            playerParty.Pokemons.ForEach(p => p.Heal());
            playerParty.PartyUpdated();
       
            yield return Fader.i.FadeOut(0.5f);
            yield return DialogManager.Instance.ShowDialogText("La vie de tes pokemons est restauré , prends en soin");
        }
        else if (SelectedChoice == 1)
        {
            yield return DialogManager.Instance.ShowDialogText("Okay, revient si tu change d'avis");
        }
       
    }
}
