using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    
    [SerializeField] List<SceneDetails> connectedScenes;
    public bool IsLoaded { get; set; }

    List<SavableEntity> savableEntities;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")){
            Debug.Log($"Entered {gameObject.name}");
            
            LoadScene();
            GameController.Instance.SetCurrentScene(this);
            
            foreach(var scene in connectedScenes){
                scene.LoadScene();
            }
            
            var prevScene = GameController.Instance.previousScene;
            if(GameController.Instance.previousScene != null){
                var previouslyLoadedScene = GameController.Instance.previousScene.connectedScenes;
                foreach(var scene in previouslyLoadedScene){
                    if (!connectedScenes.Contains(scene) && scene != this)
                    {
                        scene.UnloadScene();
                    }
                    
                    if(!connectedScenes.Contains(prevScene)){
                        prevScene.UnloadScene();
                    }
                }
            }
        }
    }
    
    private void LoadScene(){
        if(!IsLoaded){
            var operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;
            
            operation.completed += (AsyncOperation obj) => {
                savableEntities = GetSavableEntitiesInScene();
                SavingSystem.i.RestoreEntityStates(savableEntities);
            };
        }
    }
    
    private void UnloadScene(){
        if(IsLoaded){
            SavingSystem.i.CaptureEntityStates(savableEntities);
            
            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }
    
    List<SavableEntity> GetSavableEntitiesInScene()
    {
        var currentScene = SceneManager.GetSceneByName(gameObject.name);
        savableEntities = FindObjectsOfType<SavableEntity>().Where(x =>x.gameObject.scene == currentScene).ToList();
        return savableEntities;
    }
    
    
}
