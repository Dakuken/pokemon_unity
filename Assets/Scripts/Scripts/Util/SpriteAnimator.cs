using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator 
{
    SpriteRenderer spriteRenderer;
    List<Sprite> frames;
    float frameRate;
    
    float timer;
    int currentFrame;
    
    public SpriteAnimator( List<Sprite> frames, SpriteRenderer spriteRenderer, float frameRate=0.16f)
    {
        this.spriteRenderer = spriteRenderer;
        this.frames = frames;
        this.frameRate = frameRate;
        
        if (frames.Count > 0)
        {
            spriteRenderer.sprite = frames[0];
        }
    }
    
    public void Start()
    {
        currentFrame = 0;
        timer = 0f;
    }
    
    public void HandleUpdate()
    {
        timer += Time.deltaTime;
        if (timer > frameRate)
        {
            currentFrame = (currentFrame + 1) % frames.Count;
            spriteRenderer.sprite = frames[currentFrame];
            timer -= frameRate;
        }
    }
    
    public List<Sprite> Frames
    {
        get { return frames; }
    }
    

}
