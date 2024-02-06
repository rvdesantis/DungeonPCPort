using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static InventoryUI;

public class ShopUI : MonoBehaviour
{
    public PartyController party;
    public DunUIController uiController;
    public InventoryController inventory;
    public InventoryUI inventoryUI;
    public ConfirmUI confirmUI;

    public List<GameObject> inactives;

    public TextMeshProUGUI titleTXT;
    public List<Button> itemButtons;
    public List<Image> itemIMGs;
    public List<TextMeshProUGUI> itemStockTXTs;
    public ShopNPC currentShop;
    public Action afterAction;

    public void OpenShopUI(ShopNPC activeShop)
    {
        gameObject.SetActive(true);
        uiController.uiActive = true;

        currentShop = activeShop;
        int count = activeShop.itemsForSale.Count;
        List<DunItem> activeList = activeShop.itemsForSale;

        inventoryUI.OpenInventory();
        foreach(GameObject ob in inactives)
        {
            ob.gameObject.SetActive(false);
        }

        foreach (Button bt in inventoryUI.invButtons)
        {
            bt.interactable = false;
        }

        foreach (Button shopBT in itemButtons)
        {
            int x = itemButtons.IndexOf(shopBT);

            if (x < count)
            {
                shopBT.interactable = true;
                itemStockTXTs[x].gameObject.SetActive(false);
                itemIMGs[x].gameObject.SetActive(true);
                itemIMGs[x].sprite = activeList[x].icon;
            }

            if (x >= count)
            {
                shopBT.interactable = false;
                itemStockTXTs[x].gameObject.SetActive(true);
                itemIMGs[x].gameObject.SetActive(false);
            }
        }

        titleTXT.text = activeList[0].itemName + "( " + activeList[0].itemPrice + " Gold)";
        itemButtons[0].Select();

        
    }

    public void ConfirmPurchase()
    {
        string confirmMSS = "";


     
    }

    public void CloseUI()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        player.controller.enabled = true;
        uiController.uiActive = false;
        currentShop = null;
        gameObject.SetActive(false);
    }

    public void TextUpdater()
    {
        foreach (Button bt in itemButtons)
        {
            int x = itemButtons.IndexOf(bt);
            if (EventSystem.current.currentSelectedGameObject == bt.gameObject)
            {
                DunItem currentItem = currentShop.itemsForSale[x];
                titleTXT.text = null;
                titleTXT.text = currentItem.itemName + " (" + currentItem.itemPrice + " Gold)";
                inventoryUI.infoText.text = currentItem.itemInfo;
            }
        }
    }


    private void Update()
    {
        TextUpdater();
    }

}
