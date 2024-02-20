using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{
    public void onPlayerTriggered(PlayerController player)
    {
        if(UnityEngine.Random.Range(1,101) <= 10){
            player.Character.Animator.IsMoving = false;
            GameController.Instance.StartBattle();
        }
    }
    
    public bool TriggerRepeatedly => true; 
}
