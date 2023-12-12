using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    
    private Vector2 input; 
    
    private Character character; 

    private void Awake(){
        character = GetComponent<Character>();
    }
    
    public void HandleUpdate()
    {
        if(!character.IsMoving){
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x !=0) input.y = 0;

            if(input != Vector2.zero){
               StartCoroutine(character.Move(input, OnMoveOver));
            }
        }
        
        character.HandleUpdate();
        
        if(Input.GetKeyDown(KeyCode.Z))
            Interact();
        
    }

    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0,character.OffSetY), 0.2f, GameLayers.i.TriggerableLayers);
        
        foreach(var collider in colliders){
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if(triggerable != null){
                character.Animator.IsMoving = false;
                triggerable.onPlayerTriggered(this);
                break;
            }
        }
    }
    
    void Interact(){
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;
        
        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if(collider != null){
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }
    
    public string Name
    {
        get => name;
    }
    
    public Sprite Sprite
    {
        get => sprite;
    }
    
    public Character Character
    {
        get => character;
    }
}
