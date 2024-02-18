using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ShopState {Menu, Buying, Selling, Busy}
public class ShopController : MonoBehaviour
{
    [SerializeField] InventoryUI inventoryUI;

    public event Action OnStart;
    public event Action OnFinish;
    
    ShopState state;
    public static ShopController i { get; private set; }

    
    private void Awake()
    {
        i = this;
    }
    public IEnumerator StartTrading(Merchant merchant)
    {
        OnStart?.Invoke();
        yield return StartMenuState();
    }

    IEnumerator StartMenuState()
    {
        state = ShopState.Menu;
        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText("Que puis-je faire pour vous", waitForInput: false,
            choices: new List<string>() { "Buy", "Sell", "Quit" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            
        }else if (selectedChoice == 1)
        {
            state = ShopState.Selling;
            inventoryUI.gameObject.SetActive(true);
        }else if (selectedChoice == 2)
        {
            OnFinish?.Invoke();
            yield break;
        }
    }

    public void HandleUpdate()
    {
        if (state == ShopState.Selling)
        {
            inventoryUI.HandleUpdate(OnBackFromSelling,(selectedItem) => { });
            
        }
    }

    void OnBackFromSelling()
    {
        inventoryUI.gameObject.SetActive(false);
        StartCoroutine(StartMenuState());
    }
}
