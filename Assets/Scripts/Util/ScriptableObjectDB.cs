using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectDB<T> : MonoBehaviour where T : ScriptableObject
{
    private static Dictionary<string, T> objects;
    
    public static void Init()
    {
        objects = new Dictionary<string, T>();
        
        var objectArray= Resources.LoadAll<T>("");
        foreach (var obj in objectArray)
        {
            if (objects.ContainsKey(obj.name))
            {
                Debug.LogError("There are two or more pokemons with the same name: " + obj.name);
                continue;
            }
            
            objects[obj.name] = obj;
        }
        
    }
    
    public static T GetObjectByName(string name)
    {
        if (!objects.ContainsKey(name))
        {
            Debug.LogError("Couldn't find a pokemon with name: " + name);
            return null;
        }
        return objects[name];
    }
}

