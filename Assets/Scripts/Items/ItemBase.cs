using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string itemName;
    [SerializeField] string description;
    [SerializeField] Sprite icon;
    [SerializeField] float price;
    [SerializeField] bool isSellable;
    
    public string ItemName => itemName;
    public string Description => description;
    public Sprite Icon => icon;

    public float Price => price;
    public bool IsSellable => isSellable;
    
    public virtual bool Use(Pokemon pokemon)
    {
        return false;
    }

    public virtual bool CanUseInBattle => true;
    public virtual bool CanUseOutSideBattle => true;
}
