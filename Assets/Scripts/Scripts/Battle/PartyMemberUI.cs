using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HpBar hpBar;


    private Pokemon _pokemon;
    
    public void Init(Pokemon pokemon){
        
        _pokemon = pokemon;
        UpdateData();
        
        _pokemon.OnHpChanged += UpdateData;
    }
    
    void UpdateData()
    {
        nameText.text = _pokemon.Base.Name;
        levelText.text = "Lvl " + _pokemon.Level;
        hpBar.SetHP((float)_pokemon.HP / _pokemon.MaxHp);
    }
    
    public void SetSelected(bool selected){
        if(selected){
            nameText.color = GlobalSettings.Instance.HighlightedColor;
        }else{
            nameText.color = Color.black;
        }
    }
    
    
}
