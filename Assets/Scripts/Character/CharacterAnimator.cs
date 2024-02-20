using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
   [SerializeField] List<Sprite> walkDownFrames;
   [SerializeField] List<Sprite> walkLeftFrames;
   [SerializeField] List<Sprite> walkRightFrames;
   [SerializeField] List<Sprite> walkUpFrames;
   [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;
   
   public float MoveX { get; set; }
   public float MoveY { get; set; }
   public bool IsMoving { get; set; }
   
   public bool IsJumping { get; set; }
   public FacingDirection DefaultDirection { get; set; }

   private SpriteAnimator walkDownAnim;
   private SpriteAnimator walkLeftAnim;
   private SpriteAnimator walkRightAnim;
   private SpriteAnimator walkUpAnim;
   
   SpriteAnimator currentAnim;
   bool wasPreviouslyMoving;
   
   SpriteRenderer spriteRenderer;

   private void Start()
   {
      spriteRenderer = GetComponent<SpriteRenderer>();
      walkDownAnim = new SpriteAnimator(walkDownFrames, spriteRenderer);
      walkLeftAnim = new SpriteAnimator(walkLeftFrames, spriteRenderer);
      walkRightAnim = new SpriteAnimator(walkRightFrames, spriteRenderer);
      walkUpAnim = new SpriteAnimator(walkUpFrames, spriteRenderer);
      SetFacingDirection(defaultDirection);
      
      currentAnim = walkDownAnim;
   }

   private void Update()
   {
      var prevAnim = currentAnim;
      
      if(MoveX == 1)
         currentAnim = walkRightAnim;
      else if(MoveX ==- 1)
         currentAnim = walkLeftAnim;
      else if(MoveY == 1)
         currentAnim = walkUpAnim;
      else if(MoveY == -1)
         currentAnim = walkDownAnim;
      
      if(currentAnim != prevAnim || wasPreviouslyMoving != IsMoving)
         currentAnim.Start();
      if(IsJumping)
         spriteRenderer.sprite = currentAnim.Frames[currentAnim.Frames.Count-1];
      else if(IsMoving)
         currentAnim.HandleUpdate();
      else
         spriteRenderer.sprite = currentAnim.Frames[0];
      
      wasPreviouslyMoving = IsMoving;
   }

   public void SetFacingDirection(FacingDirection dir)
   {
      if(dir == FacingDirection.Right)
         MoveX = 1;
      else if(dir == FacingDirection.Left)
         MoveX = -1;
      else if(dir == FacingDirection.Up)
         MoveY = 1;
      else if(dir == FacingDirection.Down)
         MoveY = -1;
         
   }
}

public enum FacingDirection {
   Up,
   Down,
   Left,
   Right
}
