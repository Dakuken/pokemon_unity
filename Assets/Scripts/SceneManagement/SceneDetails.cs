using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    
    [SerializeField] List<SceneDetails> connectedScenes;
    public bool IsLoaded { get; set; }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")){
            Debug.Log($"Entered {gameObject.name}");
            
            LoadScene();
            GameController.Instance.SetCurrentScene(this);
            
            foreach(var scene in connectedScenes){
                scene.LoadScene();
            }
            
            if(GameController.Instance.previousScene != null){
                var previouslyLoadedScene = GameController.Instance.previousScene.connectedScenes;
                foreach(var scene in previouslyLoadedScene){
                    if (!connectedScenes.Contains(scene) && scene != this)
                    {
                        scene.UnloadScene();
                    }
                }
            }
        }
    }
    
    private void LoadScene(){
        if(!IsLoaded){
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;
        }
    }
    
    private void UnloadScene(){
        if(IsLoaded){
            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }
    
    
}
