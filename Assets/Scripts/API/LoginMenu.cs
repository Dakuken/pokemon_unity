using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MiniJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class LoginMenu : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TextMeshProUGUI errorMessageText;

    private List<PokemonBase> pokemonList = new List<PokemonBase>();
    
    public void SeConnecter()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;
        errorMessageText.text = "";

        StartCoroutine(ConnexionAPI(email, password));
    }

    IEnumerator ConnexionAPI(string email, string password)
    {
        Dictionary<string, string> loginData = new Dictionary<string, string>();
        loginData["email"] = email;
        loginData["password"] = password;
        
        string jsonData = Json.Serialize(loginData);
        
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm("https://alexisdelazzari.site/api-pokemon/auth/login", ""))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            
            yield return www.SendWebRequest();
            
                string jsonResponse = www.downloadHandler.text;
                Dictionary<string, object> response = Json.Deserialize(jsonResponse) as Dictionary<string, object>;
                
                if (response.ContainsKey("token"))
                {
                    string token = response["token"].ToString();
                    Debug.Log("Connexion réussie.");
                    StartCoroutine(FetchPokemon(token));
                }
                else if (response.ContainsKey("message"))
                {
                    string errorMessage = response["message"].ToString();
                    Debug.LogError("Échec de la connexion : " + errorMessage);
                    errorMessageText.text = errorMessage;
                }
                else
                {
                    Debug.LogError("Réponse inattendue du serveur.");
                }
        }
    }

    IEnumerator FetchPokemon(string token)
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://alexisdelazzari.site/api-pokemon/pokedex/private"))
        {
            www.SetRequestHeader("Authorization", "Bearer " + token);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur lors de la récupération des données : " + www.error);
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                List<object> pokemonsData = Json.Deserialize(jsonResponse) as List<object>;

                if (pokemonsData == null || pokemonsData.Count == 0)
                {
                    Debug.Log("Aucun Pokémon trouvé.");
                }
                else
                {
                    // Parcourir les données des pokémons et créer des objets PokemonBase
                    foreach (Dictionary<string, object> pokemonData in pokemonsData)
                    {
                        PokemonBase newPokemon = ScriptableObject.CreateInstance<PokemonBase>();
                        newPokemon.Name = pokemonData["nom"].ToString();;
                        newPokemon.Description = pokemonData["description"].ToString();
                        
                        string base64String = pokemonData["image"].ToString();
                        byte[] bytes = Convert.FromBase64String(base64String.Split(',')[1]);
                        Texture2D texture = new Texture2D(1, 1);
                        texture.LoadImage(bytes);
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                        newPokemon.Sprite = sprite;
                        
                        Dictionary<string, object> type1Data = pokemonData["type1"] as Dictionary<string, object>;
                        string uuid1String = type1Data["uuid"].ToString();
                        if (type1Data.ContainsKey("uuid"))
                        {
                            
                            if (int.TryParse(uuid1String, out int type1UUID))
                            {
                                newPokemon.Type1 = SelectionType(type1UUID);
                            }
                            else
                            {
                                Debug.LogError("Impossible de convertir l'UUID en entier : " + uuid1String);
                            }
                        }
                        
                        Dictionary<string, object> type2Data = pokemonData["type2"] as Dictionary<string, object>;
                        string uuid2String = type2Data["uuid"].ToString();
                        if (type2Data.ContainsKey("uuid"))
                        {
                            
                            if (int.TryParse(uuid2String, out int type2UUID))
                            {
                                newPokemon.Type1 = SelectionType(type2UUID);
                            }
                            else
                            {
                                Debug.LogError("Impossible de convertir l'UUID en entier : " + uuid2String);
                            }
                        }
                        
                        newPokemon.MaxHp = int.Parse(pokemonData["pv"].ToString());
                        newPokemon.Attack = int.Parse(pokemonData["attaque"].ToString());
                        newPokemon.Defense = int.Parse(pokemonData["defense"].ToString());
                        newPokemon.SpAttack = int.Parse(pokemonData["attaqueSpeciale"].ToString());
                        newPokemon.SpDefense = int.Parse(pokemonData["defenseSpeciale"].ToString());
                        newPokemon.Speed = int.Parse(pokemonData["vitesse"].ToString());
                        newPokemon.CatchRate = int.Parse(pokemonData["tauxCapture"].ToString());
                        newPokemon.ExpYield = int.Parse(pokemonData["xp"].ToString());
                        newPokemon.GrowthRate = GrowthRate.Fast;
                        newPokemon.LearnableMoves = new List<LearnableMove>();
                        newPokemon.Evolutions = new List<Evolution>();
                        
                        List<object> attaquesData = pokemonData["listAttaques"] as List<object>;
                        if (attaquesData != null)
                        {
                            foreach (Dictionary<string, object> attaqueData in attaquesData)
                            {
                                Dictionary<string, object> attaqueObjData = attaqueData["attaque"] as Dictionary<string, object>;
                                if (attaqueObjData != null)
                                {
                                    MoveBase newAttaque = ScriptableObject.CreateInstance<MoveBase>();;
                                    newAttaque.Name = attaqueObjData["nom"].ToString();
                                    newAttaque.Power = int.Parse(attaqueObjData["puissance"].ToString());
                                    newAttaque.Accuracy = 100;
                                    newAttaque.PP = int.Parse(attaqueObjData["pp"].ToString());
                                    newAttaque.Priority = 0;
                                    newAttaque.AlwaysHits = false;
                                    newAttaque.Description = attaqueObjData["description"].ToString();
                                    newAttaque.Target = MoveTarget.Foe;
                                    newAttaque.Category = MoveCategory.Physical;
                                    newAttaque.Effects = new MoveEffects();
                                    newAttaque.Secondaries = new List<SecondariesEffects>();
                                    
                                    Dictionary<string, object> type = attaqueObjData["type"] as Dictionary<string, object>;
                                    string typeString = type["uuid"].ToString();
                                    if (type.ContainsKey("uuid"))
                                    {
                                        if (int.TryParse(typeString, out int typeUuid))
                                        {
                                            newAttaque.Type = SelectionType(typeUuid);
                                        }
                                        else
                                        {
                                            Debug.LogError("Impossible de convertir l'UUID en entier : " + typeUuid);
                                        }
                                    }
                                    
                                    LearnableMove newLearnableMove = new LearnableMove();
                                    newLearnableMove.Base = newAttaque;
                                    newLearnableMove.Level = 1;
                                
                                    newPokemon.LearnableMoves.Add(newLearnableMove);
                                }
                            }
                        }
                        
                        pokemonList.Add(newPokemon);
                    }

                    Debug.Log("Pokémons récupérés : " + pokemonList.Count);
                    //affiche les leaneables moves
                    foreach (LearnableMove learnableMove in pokemonList[0].LearnableMoves)
                    {
                        Debug.Log(learnableMove.Base.Name);
                    }
                }
            }
        }
    }
    

    PokemonType SelectionType(int idType)
    {
        switch (idType)
        {
            case 1:
                return PokemonType.Grass;
            case 2:
                return PokemonType.Fire;
            case 3:
                return PokemonType.Water;
            case 4:
                return PokemonType.Bug;
            case 5:
                return PokemonType.Normal;
            case 6:
                return PokemonType.Electric;
            case 7:
                return PokemonType.Poison;
            case 8:
                return PokemonType.Fairy;
            case 9:
                return PokemonType.Flying;
            case 10:
                return PokemonType.Fighting;
            case 11:
                return PokemonType.Psychic;
            case 12:
                return PokemonType.Ground;
            case 13:
                return PokemonType.Rock;
            case 14:
                return PokemonType.Ghost;
            case 15:
                return PokemonType.Steel;
            case 16:
                return PokemonType.Ice;
            case 17:
                return PokemonType.Dragon;
            case 18:
                return PokemonType.Dark;
            case 19:
                return PokemonType.None;
            default:
                return PokemonType.None;
        }
    }

}