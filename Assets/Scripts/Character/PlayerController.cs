using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    
    public float moveSpeed;
    public LayerMask solidObjectsLayer;
    public LayerMask grassLayer;
    public LayerMask interactableLayer;

    public event Action OnEncountered;

    private bool isMoving;
    private Vector2 input; 

    private Animator animator;

    private void Awake(){
        animator = GetComponent<Animator>();
    }
    
    public void HandleUpdate()
    {
        if(!isMoving){
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x !=0) input.y = 0;

            if(input != Vector2.zero){
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if(isWalkable(targetPos)){
                    StartCoroutine(Move(targetPos));
                }
            }
            
        }

        animator.SetBool("isMoving", isMoving);
        
        if(Input.GetKeyDown(KeyCode.Z))
            Interact();
        
    }
    
    void Interact(){
        var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactPos = transform.position + facingDir;
        
        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, interactableLayer);
        if(collider != null){
            collider.GetComponent<Interactable>()?.Interact();
        }
    }
    
    

    IEnumerator Move(Vector3 targetPos){
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon){
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;

        checkForEncounters();
    }

    private bool isWalkable(Vector3 targetPos){
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer | interactableLayer ) != null){
            return false;
        } 
        return true;
    }

    private void checkForEncounters(){ 
        if(Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null){
            if(UnityEngine.Random.Range(1,101) <= 10){
                OnEncountered();
                animator.SetBool("isMoving", false);
            }
        }
    }
}
