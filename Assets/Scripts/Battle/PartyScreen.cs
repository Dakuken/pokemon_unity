using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;
    
    List<PartyMemberUI> memberSlots;
    List<Pokemon> pokemons;

    public void Init()
    {
        memberSlots = new List<PartyMemberUI>(GetComponentsInChildren<PartyMemberUI>());
    }

    public void SetPartyData(List<Pokemon> pokemons)
    {
        this.pokemons = pokemons;
        
        for(int i = 0; i < memberSlots.Count; i++)
        {
            if(i < pokemons.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(pokemons[i]);
            }
            else
            {
                memberSlots[i].gameObject.SetActive(false);
            }
        }
        messageText.text = "Choose a Pokemon";
    }
    
    public void UpdateMemberSelection(int selectedMember)
    {
        for(int i = 0; i < memberSlots.Count; i++)
        {
            if(i == selectedMember)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    }
    
    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
