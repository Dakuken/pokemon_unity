using System.Collections;
using System.Linq;
using UnityEngine;

public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] DestinationIdentifier destinationPortal;
    
    PlayerController player;
    public void onPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        this.player = player;
        StartCoroutine(Teleport());
    }

    Fader fader;
    
    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    IEnumerator Teleport()
    {
        GameController.Instance.PausedGame(true);
        yield return fader.FadeIn(0.5f);

        var destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.spawnPoint.position);
        
        yield return fader.FadeOut(0.5f);
        GameController.Instance.PausedGame(false);
    }
    
    public Transform SpawnPoint => spawnPoint;
    
    public bool TriggerRepeatedly => false; 
}
