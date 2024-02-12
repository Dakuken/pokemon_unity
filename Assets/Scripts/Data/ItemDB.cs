using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB 
{
    static Dictionary<string, ItemBase> items;

    public static void Init()
    {
        items = new Dictionary<string, ItemBase>();
        
        var ItemArray= Resources.LoadAll<ItemBase>("");
        foreach (var item in ItemArray)
        {
            if (items.ContainsKey(item.name))
            {
                Debug.LogError("There are two or more moves with the same name: " + item.name);
                continue;
            }
            
            items[item.name] = item;
        }
        
    }
    
    public static ItemBase GetItemByName(string name)
    {
        if (!items.ContainsKey(name))
        {
            Debug.LogError("Couldn't find move with name: " + name);
            return null;
        }
        return items[name];
    }
}
