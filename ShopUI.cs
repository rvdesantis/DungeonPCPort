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

    // Display Info Tab
    public GameObject infoTab;
    public TextMeshProUGUI itemTitle;
    public TextMeshProUGUI itemInfo;
    public Image itemImage;

    // Current Inventory UI
    public GameObject currentInvTab;
    public TextMeshProUGUI invTabTitleTXT;
    public List<Image> InvIMGList;
    public List<TextMeshProUGUI> InvCountTXT;
    public bool dungeonUI;
    public bool battleUI;
    public bool keyUI;

    public void OpenShopUI(ShopNPC activeShop)
    {
        gameObject.SetActive(true);
        uiController.uiActive = true;

        currentShop = activeShop;
        if (currentShop.shopType == ShopNPC.ShopType.dungeonItems)
        {
            dungeonUI = true;
            battleUI = false;
            keyUI = false;
        }


        if (currentShop.shopType == ShopNPC.ShopType.battleItems)
        {
            dungeonUI = false;
            battleUI = true;
            keyUI = false;
        }
        if (currentShop.shopType == ShopNPC.ShopType.keyItems)
        {
            dungeonUI = false;
            battleUI = false;
            keyUI = true;
        }
        int count = activeShop.itemsForSale.Count;
        List<DunItem> activeList = activeShop.itemsForSale;

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

        titleTXT.text = null;
        inventoryUI.infoText.text = null;
        itemButtons[0].Select();

        infoTab.SetActive(true);
        itemTitle.text = titleTXT.text;
        itemInfo.text = activeList[0].itemInfo + "\nPRICE: " + activeList[0].itemPrice + "G";
        itemImage.sprite = activeList[0].icon;
        RefreshCurrentInventory();
        
    }

    public void RefreshCurrentInventory() // set to DunItems
    {
        currentInvTab.SetActive(false);
        if (dungeonUI)
        {
            int InvCount = inventory.dungeonItems.Count;
            foreach (Image img in InvIMGList)
            {
                int imIndex = InvIMGList.IndexOf(img);
                if (imIndex > InvCount - 1)
                {
                    img.gameObject.SetActive(false);
                    InvCountTXT[imIndex].text = 0.ToString();
                    InvCountTXT[imIndex].gameObject.SetActive(false);
                }
                if (imIndex <= InvCount - 1)
                {
                    img.gameObject.SetActive(true);
                    InvIMGList[imIndex].sprite = inventory.dungeonItems[imIndex].icon;
                    InvCountTXT[imIndex].gameObject.SetActive(true);
                    InvCountTXT[imIndex].text = inventory.dungeonItems[imIndex].itemCount.ToString();

                    if (inventory.dungeonItems[imIndex].itemName == "Gold")
                    {
                        InvCountTXT[imIndex].text = inventory.GetAvailableGold().ToString();
                    }
                }
            }
        }
        if (battleUI)
        {
            int InvCount = inventory.battleItems.Count;
            foreach (Image img in InvIMGList)
            {
                int imIndex = InvIMGList.IndexOf(img);
                if (imIndex > InvCount - 1)
                {
                    img.gameObject.SetActive(false);
                    InvCountTXT[imIndex].text = 0.ToString();
                    InvCountTXT[imIndex].gameObject.SetActive(false);
                }
                if (imIndex <= InvCount - 1)
                {
                    img.gameObject.SetActive(true);
                    InvIMGList[imIndex].sprite = inventory.battleItems[imIndex].icon;
                    InvCountTXT[imIndex].gameObject.SetActive(true);
                    InvCountTXT[imIndex].text = inventory.battleItems[imIndex].itemCount.ToString();     
                    
                    if (imIndex == 0)
                    {
                        InvCountTXT[imIndex].text = inventory.dungeonItems[0].itemCount.ToString(); // set potion count
                    }
                }
            }
        }

        if (keyUI)
        {
            int InvCount = inventory.keyItems.Count;
            foreach (Image img in InvIMGList)
            {
                int imIndex = InvIMGList.IndexOf(img);
                if (imIndex > InvCount - 1)
                {
                    img.gameObject.SetActive(false);
                    InvCountTXT[imIndex].text = 0.ToString();
                    InvCountTXT[imIndex].gameObject.SetActive(false);
                }
                if (imIndex <= InvCount - 1)
                {
                    img.gameObject.SetActive(true);
                    InvIMGList[imIndex].sprite = inventory.keyItems[imIndex].icon;
                    InvCountTXT[imIndex].gameObject.SetActive(true);
                    InvCountTXT[imIndex].text = inventory.keyItems[imIndex].itemCount.ToString();

                    if (imIndex == 0)
                    {
                        InvCountTXT[imIndex].text = inventory.dungeonItems[0].itemCount.ToString(); // set potion count
                    }
                }
                if (imIndex == 0) // brings up Chaos Orbs even if 0 are in Inventory
                {
                    img.gameObject.SetActive(true);
                    InvIMGList[0].sprite = inventory.keyItems[0].icon;
                    InvCountTXT[0].gameObject.SetActive(true);
                    InvCountTXT[0].text = inventory.keyItems[0].itemCount.ToString();
                }
            }
        }
       
        currentInvTab.SetActive(true);
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
                        masterItem.pickUpMessage = false;
                        masterItem.PickUp();
                        inventory.ReduceGold(masterItem.itemPrice);
                        RefreshCurrentInventory();
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
                        masterItem.pickUpMessage = false;
                        masterItem.PickUp();
                        inventory.ReduceGold(masterItem.itemPrice);
                        RefreshCurrentInventory();
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
                        masterItem.pickUpMessage = false;
                        masterItem.PickUp();
                        inventory.ReduceGold(masterItem.itemPrice);
                        RefreshCurrentInventory();
                        break;
                    }
                }
            }
            int count = currentShop.itemsForSale.Count;
            foreach (Button bt in itemButtons)
            {
                int x = itemButtons.IndexOf(bt);

                if (x < count)
                {
                    bt.interactable = true;
                    itemStockTXTs[x].gameObject.SetActive(false);
                    itemIMGs[x].gameObject.SetActive(true);
                    itemIMGs[x].sprite = currentShop.itemsForSale[x].icon;
                }

                if (x >= count)
                {
                    bt.interactable = false;
                    itemStockTXTs[x].gameObject.SetActive(true);
                    itemIMGs[x].gameObject.SetActive(false);
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
        PlayerController player = FindAnyObjectByType<PlayerController>();
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
        infoTab.SetActive(false);
        currentInvTab.SetActive(false);
        gameObject.SetActive(false);
    }

    public void ItemUpdater()
    {
        foreach (Button bt in itemButtons)
        {            
            if (EventSystem.current.currentSelectedGameObject == bt.gameObject)
            {
                int x = itemButtons.IndexOf(bt);
                itemIndex = x;
                currentItem = currentShop.itemsForSale[x];
                titleTXT.text = null;      
                inventoryUI.infoText.text = null;

                itemTitle.text = currentItem.itemName;
                itemInfo.text = currentItem.itemInfo + "\nPRICE: " + currentItem.itemPrice + "G";
                itemImage.sprite = currentItem.icon;
            }
        }
    }


    private void Update()
    {
        ItemUpdater();
    }

}
