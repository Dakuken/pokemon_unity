using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;


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
public class getData : MonoBehaviour
{
    public Text pokemonNameText;
    public Text pokemonIdText;

    public Text pokemonType1;

    public Image pokemonIMG;

    public Text pokemonAttaque1;
    public Text pokemonAttaque2;
    public Text pokemonAttaque3;
    public Text pokemonAttaque4;
    private string URL = "https://pokemon-api-justinburnel.replit.app/Pokedex/";
    //private int IndexPokemon = 1;

    private string pokemonName;



    private string GetURL()
    {
        int randomIndex = RandomNumberSelector.ChooseRandomNumber();
        return URL + randomIndex.ToString();
    }

    public void GetDataFromButton()
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
                Debug.Log(data);


                Pokemon myData = JsonUtility.FromJson<Pokemon>(data);
                if (pokemonNameText != null)
                {
                    pokemonNameText.text = myData.nom; ;
                    pokemonIdText.text = myData.id.ToString();
                    pokemonType1.text = myData.type1;
                    if (pokemonType1.text == "feu")
                    {
                        pokemonType1.color = Color.red;
                    }
                    else if (pokemonType1.text == "plante")
                    {
                        pokemonType1.color = Color.green;
                    }
                    else if (pokemonType1.text == "eau")
                    {
                        pokemonType1.color = Color.blue;
                    }
                    UnityWebRequest wwwImage = UnityWebRequestTexture.GetTexture(myData.image);
                    yield return wwwImage.SendWebRequest();

                    if (wwwImage.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log(wwwImage.error);
                    }
                    else
                    {
                        Texture2D texture = DownloadHandlerTexture.GetContent(wwwImage);
                        pokemonIMG.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                    }
                    if (myData.attaques.Count <= 1 || myData.attaques[0].nom != null)
                    {
                        pokemonAttaque1.text = "";

                    }
                    else
                    {
                        pokemonAttaque1.text = myData.attaques[0].nom;
                    }
                    if (myData.attaques.Count <= 1 || myData.attaques[1].nom != null)
                    {
                        pokemonAttaque2.text = "";

                    }
                    else
                    {
                        pokemonAttaque2.text = myData.attaques[1].nom;
                    }
                    if (myData.attaques.Count <= 2 || myData.attaques[2].nom != null)
                    {
                        pokemonAttaque3.text = "";

                    }
                    else
                    {
                        pokemonAttaque3.text = myData.attaques[2].nom;
                    }
                    if (myData.attaques.Count <= 3 || myData.attaques[3].nom == null)
                    {
                        pokemonAttaque4.text = "";
                    }
                    else
                    {
                        pokemonAttaque4.text = myData.attaques[3].nom;
                    }
                    Debug.Log(myData.attaques[1].nom);

                }

            }
        }
    }
}
