using System.Collections.Generic;
using UnityEngine;

public class PokemonManager : MonoBehaviour
{
    private static PokemonManager _instance;
    public static PokemonManager Instance => _instance;

    public List<PokemonBase> PokemonList { get; private set; }

    private void Awake()
    {
        // Assurez-vous qu'il n'y a qu'une seule instance du PokemonManager dans la scène
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        // Initialiser la liste de Pokémon
        PokemonList = new List<PokemonBase>();
    }
}