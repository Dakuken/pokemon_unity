using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Items/Create new pokeball item")]
public class PokeballItem : ItemBase
{
    [SerializeField] float catchRate = 1;
    
    public override bool Use(Pokemon pokemon)
    {
        if(GameController.Instance.State != GameState.Battle)
            return false;
        
        return true;
    }

    public override bool CanUseOutSideBattle => false;
    public float CatchRate { get => catchRate; }
   
}
