using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionManager : MonoBehaviour
{
    [SerializeField] GameObject evolutionUI;
    [SerializeField] Image pokemonImage;


    public event Action OnStartEvolution;
    public event Action OnCompleteEvolution;
    public static EvolutionManager i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public IEnumerator Evolve(Pokemon pokemon, Evolution evolution)
    {
        OnStartEvolution?.Invoke();
        evolutionUI.SetActive(true);

        pokemonImage.sprite = pokemon.Base.Sprite;
        yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.name} is evolving");

        var oldPokemon = pokemon.Base;
        pokemon.Evolve(evolution);
        
        pokemonImage.sprite = pokemon.Base.Sprite;
        yield return DialogManager.Instance.ShowDialogText($"{oldPokemon.name} evolue en {pokemon.Base.name}");
        
        evolutionUI.SetActive(false);
        OnCompleteEvolution?.Invoke();
    }
}
