using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerFov : MonoBehaviour, IPlayerTriggerable
{
    public void onPlayerTriggered(PlayerController player)
    {
        GameController.Instance.OnEnterTrainerView(GetComponentInParent<TrainerController>());
    }
}
