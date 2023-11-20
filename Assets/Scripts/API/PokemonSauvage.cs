using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class PokemonSauvage : MonoBehaviour
{

    public class RandomNumberSelector
    {
        private static System.Random random = new System.Random();

        public static int ChooseRandomNumber()
        {
            int[] options = new int[] { 1, 4, 7 };
            int index = random.Next(options.Length);
            return options[index];
        }
    }
    public Text pokemonNameText;
    public Image pokemonIMG;

    private string URL = "https://pokemon-api-justinburnel.replit.app/Pokedex/";
    // private int IndexPokemon = 1;

    // private string pokemonName;



    private string GetURL()
    {
        int randomIndex = RandomNumberSelector.ChooseRandomNumber();
        return URL + randomIndex.ToString();
    }

    private void Start()
    {
        URL = GetURL();
        StartCoroutine(GetDataFromAPI());
    }


    IEnumerator GetDataFromAPI()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(URL))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {

                string data = www.downloadHandler.text;
                //Debug.Log(data);


                PokemonJoshua myData = JsonUtility.FromJson<PokemonJoshua>(data);
                if (pokemonNameText != null)
                {
                    pokemonNameText.text = myData.nom; ;

                    UnityWebRequest wwwImage = UnityWebRequestTexture.GetTexture(myData.image);
                    yield return wwwImage.SendWebRequest();
                    Debug.Log(myData.image);

                    if (wwwImage.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log(wwwImage.error);
                    }
                    else
                    {
                        Texture2D texture = DownloadHandlerTexture.GetContent(wwwImage);
                        pokemonIMG.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                    }
                }

            }
        }
    }
}



