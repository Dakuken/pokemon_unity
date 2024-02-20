using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummaryScreen : MonoBehaviour
{
    [Header("Basic Details")]
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Image image;
    
    [Header("Pokemon Skills")]
    [SerializeField] Text hpText;
    [SerializeField] Text atkText;
    [SerializeField] Text defText;
    [SerializeField] Text atkSpeText;
    [SerializeField] Text defSpeText;
    [SerializeField] Text SpeedText;
    [SerializeField] Text ExpPointText;
    [SerializeField] Text nextLvlText;
    [SerializeField] Transform expBar;

    private Pokemon pokemon;
    public void SetBasicDetails(Pokemon pokemon)
    {
        this.pokemon = pokemon;
        
        nameText.text = pokemon.Base.name;
        levelText.text = "" + pokemon.Level;
        image.sprite = pokemon.Base.Sprite;
    }

    public void SetSkills()
    {
        hpText.text = $"{pokemon.HP}/{pokemon.MaxHp}";
        atkText.text = "" + pokemon.Attack;
        defText.text = "" + pokemon.Defense;
        atkSpeText.text = "" + pokemon.SpAttack;
        defSpeText.text = "" + pokemon.SpDefense;
        SpeedText.text = "" + pokemon.Speed;

        ExpPointText.text = "" + pokemon.Exp;
        nextLvlText.text = "" + (pokemon.Base.GetExpForLevel(pokemon.Level + 1) - pokemon.Exp);
        expBar.localScale = new Vector3(pokemon.GetNormalizedExp(), 1);
    }
}
