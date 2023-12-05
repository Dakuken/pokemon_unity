using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Portal : MonoBehaviour, IPlayerTriggerable

{
    public void onPlayerTriggered(PlayerController player)
    {
        Debug.Log("Player triggered");
    }
}
