using System.Collections;
using System.Collections.Generic;
using System.Text;
using MiniJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class LoginMenu : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TextMeshProUGUI errorMessageText;

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
                    Debug.Log("Connexion réussie. Token : " + token);
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

}