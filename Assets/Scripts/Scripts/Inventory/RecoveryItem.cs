using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Items/Create new recovery item")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;
    
    [Header("PP")]
    [SerializeField] int ppAmount;
    [SerializeField] bool restoreMaxPP;
    
    [Header("Status")]
    [SerializeField] ConditionID status;
    [SerializeField] bool recoverAllStatus;
    
    [Header("Revive")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;

    public override bool Use(Pokemon pokemon)
    {
        //revive
        if(revive || maxRevive)
        {
            if(pokemon.HP > 0)
                return false;


            if (revive)
                pokemon.IncreaseHp(pokemon.MaxHp / 2);
            else if (maxRevive)
                pokemon.IncreaseHp(pokemon.MaxHp);
            
            pokemon.CureStatus();
            return true;
        }
        
        if(pokemon.HP == 0)
            return false;
        
        //HP
        if (restoreMaxHP || hpAmount > 0)
        {
            if(pokemon.HP == pokemon.MaxHp)
                return false;
            
            if(restoreMaxHP)
                pokemon.IncreaseHp(pokemon.MaxHp);
            else
                pokemon.IncreaseHp(hpAmount);
     
        }
        
        //status
        if (recoverAllStatus || status != ConditionID.none)
        {
           if (pokemon.Status == null && pokemon.VolatileStatus == null)
               return false;

           if (recoverAllStatus)
           {
               pokemon.CureStatus();
                pokemon.CureVolatileStatus();
           }
           else
           {
                if (pokemon.Status.Id == status)
                    pokemon.CureStatus();
                else if(pokemon.VolatileStatus.Id == status)
                    pokemon.CureVolatileStatus();
                else
                    return false;
           }
        }
        
        //PP
        if (restoreMaxHP)
        {
            pokemon.Moves.ForEach(m => m.PP = m.Base.PP);
        }
        else if (ppAmount > 0)
        { 
            pokemon.Moves.ForEach(m => m.PP = m.Base.PP);
        }
        
        return true;
    }
}
