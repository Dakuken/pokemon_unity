using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryUIState { ItemSelection, PartySelection, Busy }

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;
    
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;
    [SerializeField] Text categoryText;
    
    [SerializeField] Image UpArrow;
    [SerializeField] Image DownArrow;
    
    [SerializeField] PartyScreen partyScreen;
    
    Action<ItemBase> onItemUsed;
    
    Inventory inventory;
    int selectedItem = 0;
    int selectedCategory = 0;
    
    InventoryUIState state;
    
    RectTransform itemListRectTransform;
    
    List<ItemSlotUI> slotUIList;
    
    const int itemsInView = 8;
    
    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRectTransform = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateItemList();
        
        inventory.OnUpdated += UpdateItemList;
    }

    void UpdateItemList()
    {
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var itemSlotGameObject = Instantiate(itemSlotUI, itemList.transform);
            itemSlotGameObject.SetData(itemSlot);
            
            slotUIList.Add(itemSlotGameObject);
        }
        
        UpdateItemSelection();
    }

    public void HandleUpdate(Action onBack, Action<ItemBase> onItemUsed=null)
    {
        this.onItemUsed = onItemUsed;
        
        if (state == InventoryUIState.ItemSelection)
        {
            int prevSelected = selectedItem;
            int prevCategory = selectedCategory;
       
            if(Input.GetKeyDown(KeyCode.DownArrow))
                ++selectedItem;
            else if(Input.GetKeyDown(KeyCode.UpArrow))
                --selectedItem;
            else if(Input.GetKeyDown(KeyCode.RightArrow))
                ++selectedCategory;
            else if(Input.GetKeyDown(KeyCode.LeftArrow))
                --selectedCategory;
       
            if(selectedCategory > Inventory.ItemCategories.Count - 1)
                selectedCategory = 0;
            else if(selectedCategory < 0)
                selectedCategory = Inventory.ItemCategories.Count - 1;
            
            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.GetSlotsByCategory(selectedCategory).Count - 1);

            if (prevCategory != selectedCategory)
            {
                ResetSelection();
                categoryText.text = Inventory.ItemCategories[selectedCategory];
                UpdateItemList();
            }
            else if(prevSelected != selectedItem)
                UpdateItemSelection();

            if (Input.GetKeyDown(KeyCode.Z))
                ItemSelected();
            else if (Input.GetKeyDown(KeyCode.X))
            {
                onBack?.Invoke();
            }
        }
        else if (state == InventoryUIState.PartySelection)
        {
            Action onSelected = () =>
            {
                StartCoroutine(UseItem());
            };
            
            Action onBackPartyScreen = ClosePartyScreen;
            
            partyScreen.HandleUpdate(onSelected, onBackPartyScreen);
        }
    }

    void ItemSelected()
    {
        if (selectedCategory == (int)ItemCategory.Pokeballs)
        {
            StartCoroutine(UseItem());
        }
        else
        {
            OpenPartyScreen();
        }
    }

    IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;
        
        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);
        if (usedItem != null)
        {
            if(!(usedItem is PokeballItem)){
                yield return DialogManager.Instance.ShowDialogText($"The player used {usedItem.ItemName}");
            }
            onItemUsed?.Invoke(usedItem);
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"It won't have any effect");
        }
        
        ClosePartyScreen();
    }
    
    void UpdateItemSelection()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);
        
        selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count - 1);
        
        for (int i = 0; i < slotUIList.Count; ++i)
        {
            if (i == selectedItem)
                slotUIList[i].NameText.color = GlobalSettings.Instance.HighlightedColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }
        

        if (slots.Count > 0)
        {
            var item = slots[selectedItem].Item;
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }

        HandleScrolling();
    }

    void HandleScrolling()
    {
        if(slotUIList.Count <= itemsInView) return;
        
        float scrollPos = Mathf.Clamp(selectedItem - itemsInView/2 ,0, selectedItem )* slotUIList[0].Height;
        itemListRectTransform.localPosition = new Vector2(itemListRectTransform.localPosition.x, scrollPos);
        
        bool showUpArrow = selectedItem > itemsInView / 2;
        UpArrow.gameObject.SetActive(showUpArrow);
        
        bool showDownArrow = selectedItem + itemsInView / 2 < slotUIList.Count;
        DownArrow.gameObject.SetActive(showDownArrow);
    }
    
    void ResetSelection()
    {
        selectedItem = 0;
        
        UpArrow.gameObject.SetActive(false);
        DownArrow.gameObject.SetActive(false);
        
        itemIcon.sprite = null;
        itemDescription.text = "";
    }

    void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);
    }
    
    void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;
        partyScreen.gameObject.SetActive(false);
    }
    
}
