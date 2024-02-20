using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapAreaAPI : MonoBehaviour
{
    List<PokemonEncounterRecord> wildPokemons = new List<PokemonEncounterRecord>();
    [SerializeField] List<PokemonBase> wildPokemonLocal;
    private void Start()
    {
        List<PokemonBase> pokemonList = PokemonManager.Instance.PokemonList;

        if (pokemonList.Count == 0)
        {
            foreach (var pokemon in wildPokemonLocal)
            {
                PokemonEncounterRecord record = new PokemonEncounterRecord();
                record.pokemon = pokemon;
                record.chancePercentage = 100 / wildPokemonLocal.Count;
                record.levelRange = new Vector2Int(2, 5);
                wildPokemons.Add(record);
            }
        }
        else
        {
            foreach (var pokemon in pokemonList)
            {
                PokemonEncounterRecord record = new PokemonEncounterRecord();
                record.pokemon = pokemon;
                record.chancePercentage = 100 / pokemonList.Count;
                record.levelRange = new Vector2Int(2, 5);
                wildPokemons.Add(record);
            }
        }

        int totalChance = 0;
        foreach (var record in wildPokemons)
        {
            record.chanceLower = totalChance;
            record.chanceUpper = totalChance + record.chancePercentage;

            totalChance = totalChance + record.chancePercentage;
        }
        
        
    }

    public Pokemon GetRandomWildPokemon()
    {
        int randVal = Random.Range(1, 101);
        var pokemonRecord = wildPokemons.First(p => randVal >= p.chanceLower && randVal <= p.chanceUpper);
        var levelRange = pokemonRecord.levelRange;
        int level = levelRange.y == 0 ? levelRange.x : Random.Range(levelRange.x, levelRange.y + 1);

        var wildPokemon = new Pokemon(pokemonRecord.pokemon, level);
        // var wildPokemon = wildPokemons[Random.Range(0, wildPokemons.Count)];
        wildPokemon.Init();
        return wildPokemon;
    }
}

