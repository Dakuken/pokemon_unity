using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField usernameInputField;
    [SerializeField]
    public TMP_InputField passwordInputField;

    [SerializeField]
    private TextMeshProUGUI errorText;



    //appeller quand on appuie sur le boutton connexion, va recuper le mot de passe et le nom d'utilisateur et va regarde si il est correct
    public void OnSubmitLogin()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;



        string loginCheckMessage = checkLoginInfo(username, password);

        if (string.IsNullOrEmpty(loginCheckMessage) && username == "root" && password == "root")
        {
                Debug.Log("Login");
                SceneManager.LoadScene(1);
        }
        else
        {
            Debug.LogError("Error:" + loginCheckMessage);
            errorText.text = loginCheckMessage;
        }
    }

    /// <summary>
    /// Regarde si le login est correct et acceptable
    /// </summary>
    /// <returns>Retourne un string remplit ou null pour le loggin et va retourner un string avec l'erreur</returns>
    public string checkLoginInfo(string username, string password)
    {
        string returnString = "";

        if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
        {
            returnString = "Le nom d'utilisteur et le mot de passe est vide ou incorect";
        }
        else if (string.IsNullOrEmpty(username))
        {
            returnString = "Username est vide ou incorect";
        }
        else if (string.IsNullOrEmpty(password))
        {
            returnString = "Password est vide ou incorect";
        }
        else if (username != "root" && password != "root")
        {
            returnString = "Le nom d'utilisteur et le mot de passe est incorect";
        }
        else
        {
            returnString = "";
        }

        return returnString;
    }
    /// <summary>
    /// On veut enlever le message d'erreur
    /// </summary>
    /// <returns></returns>
    public void RemoveErrorText()
    {
        errorText.text = "";
    }
}
