using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;
    
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;
    
    [SerializeField] Image UpArrow;
    [SerializeField] Image DownArrow;

    int selectedItem;
    List<ItemBase> availableItems;
    Action<ItemBase> onItemSelected;
    Action onBack;
    
    
    List<ItemSlotUI> slotUIList;
    
    const int itemsInView = 8;
    
    RectTransform itemListRectTransform;
    
    private void Awake()
    {
        itemListRectTransform = itemList.GetComponent<RectTransform>();
    }
    
    public void Show(List<ItemBase> availableItems, Action<ItemBase> onItemSelected, Action onBack)
    {
        this.availableItems = availableItems;
        this.onItemSelected = onItemSelected;
        this.onBack = onBack;
        
        gameObject.SetActive(true);
        UpdateItemList();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void HandleUpdate()
    {
        var prevSelection = selectedItem;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, availableItems.Count - 1);
        
        if(selectedItem != prevSelection)
            UpdateItemSelection();
        
        if(Input.GetKeyDown(KeyCode.Z))
            onItemSelected?.Invoke(availableItems[selectedItem]);
        else if (Input.GetKeyDown(KeyCode.X))
            onBack?.Invoke();
        
    }
    void UpdateItemList()
    {
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach (var item in availableItems)
        {
            var itemSlotGameObject = Instantiate(itemSlotUI, itemList.transform);
            itemSlotGameObject.SetNameAndPrice(item);
            
            slotUIList.Add(itemSlotGameObject);
        }
        
        UpdateItemSelection();
    }
    
    void UpdateItemSelection()
    {
       
        selectedItem = Mathf.Clamp(selectedItem, 0, availableItems.Count - 1);
        for (int i = 0; i < slotUIList.Count; ++i)
        {
            if (i == selectedItem)
                slotUIList[i].NameText.color = GlobalSettings.Instance.HighlightedColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }
       
        if (availableItems.Count > 0)
        {
            var item = availableItems[selectedItem];
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
}
