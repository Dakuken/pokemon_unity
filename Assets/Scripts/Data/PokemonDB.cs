using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonDB
{
   static Dictionary<string, PokemonBase> pokemons;

    public static void Init()
    {
        pokemons = new Dictionary<string, PokemonBase>();
        
        var pokemonArray= Resources.LoadAll<PokemonBase>("");
        foreach (var pokemon in pokemonArray)
        {
            if (pokemons.ContainsKey(pokemon.Name))
            {
                Debug.LogError("There are two or more pokemons with the same name: " + pokemon.Name);
                continue;
            }
            
            pokemons[pokemon.Name] = pokemon;
        }
        
    }
    
    public static PokemonBase GetPokemonByName(string name)
    {
        if (!pokemons.ContainsKey(name))
        {
            Debug.LogError("Couldn't find a pokemon with name: " + name);
            return null;
        }
        return pokemons[name];
    }
}
