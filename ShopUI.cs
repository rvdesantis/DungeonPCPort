using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ShopUI : MonoBehaviour
{
    public PartyController party;
    public DunUIController uiController;
    public InventoryController inventory;
    public InventoryUI inventoryUI;
    public ConfirmUI confirmUI;
    public MessagePanelUI messageUI;

    public List<GameObject> inactives;

    public TextMeshProUGUI titleTXT;
    public List<Button> itemButtons;
    public List<Image> itemIMGs;
    public List<TextMeshProUGUI> itemStockTXTs;

    public int itemIndex;

    public DunItem currentItem;
    public ShopNPC currentShop;
    public Action afterAction;

    public List<AudioClip> shopSounds; // 0 coins, 

    public void OpenShopUI(ShopNPC activeShop)
    {
        gameObject.SetActive(true);
        uiController.uiActive = true;

        currentShop = activeShop;
        int count = activeShop.itemsForSale.Count;
        List<DunItem> activeList = activeShop.itemsForSale;

        if (currentShop.shopType == ShopNPC.ShopType.dungeonItems)
        {
            inventoryUI.OpenInventory();
        }

        if (currentShop.shopType == ShopNPC.ShopType.battleItems)
        {
            inventoryUI.OpenInventory();
            inventoryUI.OpenBattleInventory();
        }

        if (currentShop.shopType == ShopNPC.ShopType.keyItems)
        {
            inventoryUI.OpenInventory();
            inventoryUI.OpenKeyInventory();
        }


        foreach (GameObject ob in inactives)
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

    public void ConfirmPurchase() // attached to buttons
    {
        string confirmMSS = "Are you sure you would like to purchase " + currentItem.itemName + " for " + currentItem.itemPrice + " gold?"; 
        foreach (Button bt in itemButtons)
        {
            bt.interactable = false;
        }
        int currentG = inventory.GetAvailableGold();
   
        confirmUI.targetAction = null;
        if (currentG >= currentItem.itemPrice)
        {
            confirmUI.targetAction = BuyItem;
        }
        if (currentG < currentItem.itemPrice)
        {
            confirmUI.targetAction = NotEnoughGold; ;
        }
        confirmUI.ConfirmMessageNPC(confirmMSS, null, this);
    }

    public void NotEnoughGold()
    {
        uiController.uiAudioSource.PlayOneShot(shopSounds[1]);

        PlayerController player = uiController.controller.playerController;

        int gold = inventory.GetAvailableGold();
        string notGold = "Not enough gold to purchase " + currentItem.itemName;

        messageUI.OpenMessage(notGold);
        messageUI.CloseMessageTimer(4);
        player.controller.enabled = true;

        foreach (Button bt in itemButtons)
        {
            int x = itemButtons.IndexOf(bt);
            if (x < currentShop.itemsForSale.Count)
            {
                bt.interactable = true;
            }
        }

        itemButtons[0].Select();
    }

    public void BuyItem()
    {
        uiController.uiAudioSource.PlayOneShot(shopSounds[0]);
        bool dunI = false;
        bool keyI = false;
        bool batI = false;
        if (!currentItem.trinket)
        {
            if (currentItem.itemType == DunItem.ItemType.keyItem)
            {
                keyI = true;               
            }
            if (currentItem.itemType == DunItem.ItemType.battle)
            {
                batI = true;              
            }
            if (currentItem.itemType == DunItem.ItemType.dungeon)
            {
                dunI = true;                
            }
            if (dunI)
            {
                Debug.Log("Adding to Dun Inventory");
                foreach (DunItem masterItem in inventory.masterDungeonItems)
                {
                    if (masterItem.itemName == currentItem.itemName)
                    {
                        Debug.Log("Adding " + masterItem.itemName + " to Dungeon Inventory");
                        masterItem.PickUp();
                        inventory.ReduceGold(masterItem.itemPrice);
                        inventoryUI.OpenInventory();
                        break;
                    }
                }

            }
            if (keyI)
            {
                Debug.Log("Adding to Key Inventory");
                foreach (DunItem masterItem in inventory.masterKeyItems)
                {
                    if (masterItem.itemName == currentItem.itemName)
                    {
                        Debug.Log("Adding " + masterItem.itemName + " to Key Items Inventory");
                        masterItem.PickUp();
                        inventory.ReduceGold(masterItem.itemPrice);
                        inventoryUI.OpenKeyInventory();
                        break;
                    }
                }
            }
            if (batI)
            {
                Debug.Log("Adding to Battle Inventory");
                foreach (DunItem masterItem in inventory.masterBattleItems)
                {
                    if (masterItem.itemName == currentItem.itemName)
                    {
                        Debug.Log("Adding " + masterItem.itemName + " to Battle Inventory");
                        masterItem.PickUp();
                        inventory.ReduceGold(masterItem.itemPrice);                        
                        inventoryUI.OpenBattleInventory();
                        break;
                    }
                }
            }      
            foreach (Button bt in itemButtons)
            {
                int x = itemButtons.IndexOf(bt);
                if (x < currentShop.itemsForSale.Count)
                {
                    bt.interactable = true;
                }
            }
            itemButtons[0].Select();
        }
        if (currentItem.trinket)
        {
            if (currentShop.faceCam != null)
            {
                currentShop.faceCam.m_Priority = -1;
                currentShop.faceCam.gameObject.SetActive(false);
            }
            uiController.inventoryUI.CloseUI();
            uiController.ToggleKeyUI(gameObject, false);
            uiController.pickUpUI.gameObject.SetActive(true);
            uiController.pickUpUI.OpenImage(currentItem);
            uiController.pickUpUI.afterAction = currentItem.PickUp;
            gameObject.SetActive(false);
        }


    }

    public void CloseUI()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        player.controller.enabled = true;
        uiController.isToggling = true;
        uiController.RemoteToggleTimer(.2f);
        uiController.inventoryUI.CloseUI();
        uiController.uiActive = false;
        if (uiController.confirmUI.gameObject.activeSelf)
        {
            uiController.confirmUI.gameObject.SetActive(false);
        }
        if (currentShop.faceCam != null)
        {
            currentShop.faceCam.m_Priority = -1;
            currentShop.faceCam.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    public void TextUpdater()
    {
        foreach (Button bt in itemButtons)
        {            
            if (EventSystem.current.currentSelectedGameObject == bt.gameObject)
            {
                int x = itemButtons.IndexOf(bt);
                itemIndex = x;
                currentItem = currentShop.itemsForSale[x];
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
