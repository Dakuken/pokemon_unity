using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCategory
{
    Items,
    Pokeballs,
    tms
}

public class Inventory : MonoBehaviour, ISavable
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
    
    public ItemBase UseItem(int indexItem, Pokemon selectedPokemon, int selectedCategory)
    {
        var currentSlots = GetSlotsByCategory(selectedCategory);
        
        var item = currentSlots[indexItem].Item;
        bool itemUsed = item.Use(selectedPokemon);
        if (itemUsed)
        {
            RemoveItem(item, selectedCategory);
            return item;
        }
        
        return null;
    }

    public void AddItem(ItemBase item, int count=1)
    {
      int category = (int)GetCategoryFromItem(item);
      var currentSlots = GetSlotsByCategory(category);

      var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);
      if (itemSlot != null)
      {
          itemSlot.Count += count;
      }
      else
      {
          currentSlots.Add(new ItemSlot()
          {
              Item = item,
              Count = count
          });
      }
      OnUpdated?.Invoke();
    }
    public void RemoveItem(ItemBase item, int category )
    {
        var currentSlots = GetSlotsByCategory(category);
        
        var itemslot = currentSlots.First(x => x.Item == item);
        itemslot.Count--;
        if(itemslot.Count == 0)
            currentSlots.Remove(itemslot);
        
        OnUpdated?.Invoke();
    }

    ItemCategory GetCategoryFromItem(ItemBase item)
    {
        if (item is RecoveryItem)
            return ItemCategory.Items;
        else if (item is PokeballItem)
            return ItemCategory.Pokeballs;
        else
            return ItemCategory.tms;
    }
    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    public object CaptureState()
    {
        var saveData = new InventorySaveData()
        {
            items = slots.Select(i => i.GetSaveData()).ToList(),
            pokeballs = pokeballSlots.Select(i => i.GetSaveData()).ToList(),
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as InventorySaveData;

        slots = saveData.items.Select(i => new ItemSlot(i)).ToList();
        pokeballSlots = saveData.items.Select(i => new ItemSlot(i)).ToList();
        
        
        allSlots = new List<List<ItemSlot>>()
        {
            slots,
            pokeballSlots
        };
        
        OnUpdated?.Invoke();
    }
}

[Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemSlot()
    {
        
    }
    public ItemSlot(ItemSaveData saveData)
    {
        item = ItemDB.GetItemByName(saveData.name);
        count = saveData.count;
    }
    
    
    public ItemSaveData GetSaveData()
    {
        var saveData = new ItemSaveData()
        {
            name = item.name,
            count = count
        };
        return saveData;
    }
    public ItemBase Item
    {
        get => item;
        set => item = value;
    }

    public int Count
    {
        get => count;
        set => count = value;
    }
}

[Serializable]
public class ItemSaveData
{
    public string name;
    public int count;
}

[Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> items;
    public List<ItemSaveData> pokeballs;
    public List<ItemSaveData> tms;
}