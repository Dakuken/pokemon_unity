using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] ChoiceBox choiceBox;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;
    
    public event Action OnShowDialog;
    public event Action OnCloseDialog;
    
    public static DialogManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    
    
    public bool IsShowing { get; private set; }

    public IEnumerator ShowDialogText(string text, bool waitForInput=true,bool autoClose=true,List<string> choices=null, Action<int> onChoiceSelected=null )
    {
        OnShowDialog?.Invoke();
        IsShowing = true;
        dialogBox.SetActive(true);
        
        yield return TypeDialog(text);
        if (waitForInput)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z)); 
        }
        
        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        if (autoClose)
        {
            CloseDialog();
        }
        OnCloseDialog?.Invoke();
    }

    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
    }
    public IEnumerator ShowDialog(Dialog dialog, List<string> choices=null, Action<int> onChoiceSelected=null)
    {
        yield return new WaitForEndOfFrame();
        
        OnShowDialog?.Invoke();
        
        AudioManager.i.PlaySFX(AudioId.UISelect);
        
        IsShowing = true;
        
        /*this.dialog = dialog;
        onDialogFinished = onFinished;*/
        
        dialogBox.SetActive(true);

        foreach (var line in dialog.Lines)
        {
           yield return TypeDialog(line);
           yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }

        if (choices != null && choices.Count > 1)
        {
           yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }
        dialogBox.SetActive(false);
        IsShowing = false;
        OnCloseDialog?.Invoke();

        //StartCoroutine(TypeDialog(dialog.Lines[0]));
    }
    
    public IEnumerator TypeDialog(string line)
    {
        //isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/ lettersPerSecond);
        }
        //isTyping = false;
    }
    
    public void HandleUpdate()
    {
        
       /* if (Input.GetKeyDown(KeyCode.Z) && !isTyping)
        {
            ++currentLine;
            if (currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            }
            else
            {
                currentLine = 0;
                IsShowing = false;
                dialogBox.SetActive(false);
                onDialogFinished?.Invoke();
                OnCloseDialog?.Invoke();
            }
        }*/
    }
}
