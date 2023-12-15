using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;
    
    public bool IsPlayerUnit
    {
        get { return isPlayerUnit; }
    }
    
    public BattleHud Hud
    {
        get { return hud; }
    }
    
    public Pokemon Pokemon { get; set; }

    private Image image;
    Vector3 originalPos;
    Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.rectTransform.localPosition;
        originalColor = image.color;
    }

    public void Setup(Pokemon pokemon)
    {
        Pokemon = pokemon;
        image.sprite = Pokemon.Base.Sprite;
        hud.gameObject.SetActive(true);
        hud.SetData(pokemon);
        
        transform.localScale = new Vector3(1f, 1f, 1f);
        image.color = originalColor;
        PlayEnterAnimation();
    }
    
    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }

    public void PlayEnterAnimation()
    {
        if(isPlayerUnit)
        {
            image.rectTransform.localPosition = new Vector3(-500f, originalPos.y);
        }
        else
        {
            image.rectTransform.localPosition = new Vector3(500f, originalPos.y);
        }
        image.transform.DOLocalMoveX( originalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if(isPlayerUnit)
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        }
        else
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));
        }
        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }
    
    public void PlayHitAnimation()
    {
       var sequence = DOTween.Sequence();
       sequence.Append(image.DOColor(Color.gray, 0.1f));
       sequence.Append(image.DOColor(originalColor, 0.1f));
         
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }

    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0f, 0.5f));
        sequence.Join(image.transform.DOLocalMoveY(originalPos.y + 50f, 0.5f));
        sequence.Join(image.transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }
    
    public IEnumerator PlayBreakOutAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1f, 0.5f));
        sequence.Join(image.transform.DOLocalMoveY(originalPos.y, 0.5f));
        sequence.Join(image.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }
}
