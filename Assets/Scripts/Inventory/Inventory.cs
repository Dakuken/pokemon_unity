using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    [SerializeField] List<ItemSlot> slots;
    [SerializeField] List<ItemSlot> pokeballSlots;
    
    List<List<ItemSlot>> allSlots;
    
    public event Action OnUpdated;

    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>()
        {
            slots,
            pokeballSlots
        };
    }

    public static List<string> ItemCategories {get; private set;} = new List<string>(){
        "ITEMS",
        "POKEBALLS"
    };
    
    public List<ItemSlot> GetSlotsByCategory(int category)
    {
        return allSlots[category];
    }
    
    public ItemBase UseItem(int indexItem, Pokemon selectedPokemon)
    {
        var item = slots[indexItem].Item;
        bool itemUsed = item.Use(selectedPokemon);
        if (itemUsed)
        {
            RemoveItem(item);
            return item;
        }
        
        return null;
    }
    
    public void RemoveItem(ItemBase item)
    {
        var itemslot = slots.First(x => x.Item == item);
        itemslot.Count--;
        if(itemslot.Count == 0)
            slots.Remove(itemslot);
        
        OnUpdated?.Invoke();
    }
    
    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }
}

[Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;
    
    public ItemBase Item => item;

    public int Count
    {
        get => count;
        set => count = value;
    }
}
