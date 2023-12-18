using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] Transform spawnPoint;
    [SerializeField] DestinationIdentifier destinationPortal;
    
    PlayerController player;
    public void onPlayerTriggered(PlayerController player)
    {
        this.player = player;
        StartCoroutine(SwitchScene());
    }

    Fader fader;
    
    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);
        
        GameController.Instance.PausedGame(true);
        yield return fader.FadeIn(0.5f);
        
        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.spawnPoint.position);
        
        yield return fader.FadeOut(0.5f);
        GameController.Instance.PausedGame(false);
        
        Destroy(gameObject);
    }
    
    public Transform SpawnPoint => spawnPoint;
}

public enum DestinationIdentifier { A, B, C, D, E }
