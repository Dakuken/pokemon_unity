using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask fovLayer;
    
    public static GameLayers i { get; private set; }
    
    private void Awake()
    {
        i = this;
    }
    
    public LayerMask SolidObjectsLayer{
        get { return solidObjectsLayer; }
    }
    
    public LayerMask GrassLayer{
        get { return grassLayer; }
    }
    
    public LayerMask InteractableLayer{
        get { return interactableLayer; }
    }
    
    public LayerMask PlayerLayer{
        get { return playerLayer; }
    }
    
    public LayerMask FovLayer{
        get { return fovLayer; }
    }

}
