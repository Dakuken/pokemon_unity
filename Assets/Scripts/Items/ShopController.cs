using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ShopState {Menu, Buying, Selling, Busy}
public class ShopController : MonoBehaviour
{
    [SerializeField]  Vector2 shopCameraOffSet;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField]  ShopUI shopUI;
    [SerializeField]  WalletUI walletUI;
    [SerializeField]  CountUI countUI;

    public event Action OnStart;
    public event Action OnFinish;
    
    ShopState state;

    private Merchant merchant;
    public static ShopController i { get; private set; }

    
    private void Awake()
    {
        i = this;
    }

    private Inventory inventory;
    private void Start()
    {
        inventory = Inventory.GetInventory();
    }

    public IEnumerator StartTrading(Merchant merchant)
    {
        this.merchant = merchant;
        OnStart?.Invoke();
        yield return StartMenuState();
    }

    IEnumerator StartMenuState()
    {
        state = ShopState.Menu;
        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText("Que puis-je faire pour vous", 
            waitForInput: false,
            choices: new List<string>() { "Buy", "Sell", "Quit" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            state = ShopState.Buying;
            yield return GameController.Instance.MoveCamera(shopCameraOffSet);
            walletUI.Show();
            shopUI.Show(merchant.AvailableItems, (item) => StartCoroutine(BuyItem(item)),
               () =>StartCoroutine(OnBackFromBuying()));
            state = ShopState.Buying;
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
            inventoryUI.HandleUpdate( OnBackFromSelling,(selectedItem) => StartCoroutine(SellItem(selectedItem)));
            
        }
        else if (state == ShopState.Buying)
        {
            shopUI.HandleUpdate();
        }
    }

    void OnBackFromSelling()
    {
        inventoryUI.gameObject.SetActive(false);
        StartCoroutine(StartMenuState());
    }

    IEnumerator SellItem(ItemBase item)
    {
        state = ShopState.Busy;

        if (!item.IsSellable)
        {
            yield return DialogManager.Instance.ShowDialogText("Tu ne peux pas vendre cette objet");
            state = ShopState.Selling;
            yield break;
        }

        walletUI.Show();
        
        float sellingPrice = Mathf.Round( item.Price / 2);
        int countToSell = 1;

        int itemCount = inventory.GetItemCount(item);
        
        if (itemCount > 1)
        {
            yield return DialogManager.Instance.ShowDialogText($"Combien voulez-vous en vendre? ",
                waitForInput: false, autoClose: false);

            yield return countUI.ShowSelector(itemCount, sellingPrice, (selectedCount) => countToSell = selectedCount);
            
            DialogManager.Instance.CloseDialog();
        }

        sellingPrice = sellingPrice * countToSell;
            
            
        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText($"Je peux vous donner {sellingPrice} pour sa! Voulez-vous le vendre ? ", 
            waitForInput: false,
            choices: new List<string>() { "Oui", "Non"},
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            inventory.RemoveItem(item, countToSell);
            Wallet.i.AddMoney(sellingPrice);
            yield return DialogManager.Instance.ShowDialogText($"Vous recevez {sellingPrice}");
        }

        walletUI.Close();
        
        state = ShopState.Selling;
    }

    IEnumerator BuyItem(ItemBase item)
    {
        state = ShopState.Busy;
        yield return DialogManager.Instance.ShowDialogText("Combien voulez-vous en achetez", 
            waitForInput: false , autoClose: false);

        int countToBuy = 1 ;
        yield return countUI.ShowSelector(100, item.Price,
            (selectedCount) => countToBuy = selectedCount);
        
        DialogManager.Instance.CloseDialog();

        float totalPrice = item.Price * countToBuy;

        if (Wallet.i.HasMoney(totalPrice))
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"Cela vous fera {totalPrice}, voulez-vous continuer ? ", 
                waitForInput: false,
                choices: new List<string>() { "Oui", "Non"},
                onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

            if (selectedChoice == 0)
            {
                inventory.AddItem(item, countToBuy);
                Wallet.i.TakeMoney(totalPrice);
                yield return DialogManager.Instance.ShowDialogText("Merci pour votre achat");
            }
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText("Vous n'avez pas asser d'argent");
        }

        state = ShopState.Buying;
    }

    IEnumerator OnBackFromBuying()
    {
        yield return GameController.Instance.MoveCamera(-shopCameraOffSet);
        shopUI.Close();
        walletUI.Close();
        StartCoroutine(StartMenuState());
    }
}
