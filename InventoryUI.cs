using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class InventoryUI : MonoBehaviour
{
    public enum InvState { Dungeon, Battle, Trinket, Key }
    public InvState invState;
    public DunUIController uiController;
    public InventoryController inventory;
    public SceneController controller;
    public ShopUI shopUI;
    public ConfirmUI confirmUI;

    public List<Button> invButtons;
    public Button exitButton;
    public Sprite emptySprite;
    public List<Image> invImages;
    public List<Image> tabs;
    public List<Sprite> tabSprites;
    public int tabIndex;
    public int buttonIndex;

    public TextMeshProUGUI text;
    public TextMeshProUGUI infoText;
    public List<TextMeshProUGUI> itemCountTXTs;
    public DunItem activeItem;

    public Image tabCommand;
    public Image exitCommand;

    public bool toggleSwap;
    public bool joystick;

    public List<Image> inputImages;
    public List<Image> joyImages;


    IEnumerator SwapTimer()
    {
        yield return new WaitForSeconds(.25f);
        toggleSwap = false;
    }

    public void ConfirmUseItem() // attached to Inv Buttons
    {
        
        if (buttonIndex != 1 && buttonIndex != 2)
        {
            foreach (Button bt in invButtons)
            {
                bt.interactable = false;
            }

            string mss = "Are you sure you would like to use " + activeItem.itemName + "?";
            if (invState == InvState.Dungeon)
            {
                activeItem = inventory.dungeonItems[buttonIndex];
            }
            if (invState == InvState.Battle)
            {
                activeItem = inventory.battleItems[buttonIndex];
            }
            if (invState == InvState.Key)
            {
                activeItem = inventory.keyItems[buttonIndex];
            }
            confirmUI.gameObject.SetActive(true);
            confirmUI.ConfirmMessageUI(mss, false, false, false, false, true);
        }

        if (buttonIndex == 2)
        {
            MapController mapC = uiController.controller.mapController;

            mapC.ToggleMap();
            gameObject.SetActive(false);
        }

    }

    public void BackUseItem()
    {
        InventoryUI inventoryUI = FindObjectOfType<DunUIController>().inventoryUI;
        
        foreach (Button bt in invButtons)
        {
            int x = invButtons.IndexOf(bt);
            if (inventoryUI.invState == InvState.Dungeon)
            {
                if (x < inventory.dungeonItems.Count)
                {
                    bt.interactable = true;
                }
            }
            if (inventoryUI.invState == InvState.Battle)
            {
                if (x < inventory.battleItems.Count)
                {
                    bt.interactable = true;
                }
            }
            if (inventoryUI.invState == InvState.Key)
            {
                if (x < inventory.keyItems.Count)
                {
                    bt.interactable = true;
                }
            }
        }
        invButtons[buttonIndex].Select();
    }

    public void TriggerUseItem()
    {
        if (activeItem.itemCount > 0)
        {
            if (!activeItem.gameObject.activeSelf)
            {
                activeItem.gameObject.SetActive(true);
            }
            activeItem.UseItem();
            activeItem.itemCount--;
            if (activeItem == inventory.dungeonItems[0])
            {
                inventory.battleItems[0].itemCount--;
            }
            if (activeItem == inventory.battleItems[0])
            {
                inventory.dungeonItems[0].itemCount--;
            }
        }
        uiController.uiActive = false;
        uiController.RemoteToggleTimer(.2f);
        gameObject.SetActive(false);
    }


    public void OpenInventory()
    {
        if (!exitButton.gameObject.activeSelf)
        {
            if (!shopUI.gameObject.activeSelf)
            {
                exitButton.gameObject.SetActive(true);
            }
        }
        gameObject.SetActive(true);
        tabIndex = 0;
        // Set Tabs
        tabs[0].sprite = tabSprites[0];
        tabs[1].sprite = tabSprites[1];
        tabs[2].sprite = tabSprites[2];
        invState = InvState.Dungeon;


        foreach (DunItem item in inventory.dungeonItems)
        {
            int x = inventory.dungeonItems.IndexOf(item);
            itemCountTXTs[x].text = item.itemCount.ToString();
            invButtons[x].interactable = true;
            itemCountTXTs[x].gameObject.SetActive(true);
            invImages[x].sprite = item.icon;
            invImages[x].gameObject.SetActive(true);
        }

        foreach (DunItem item in inventory.trinketC.activeDunTrinkets)
        {
            int x = inventory.trinketC.activeDunTrinkets.IndexOf(item) + inventory.dungeonItems.Count;
            itemCountTXTs[x].text = item.itemCount.ToString();
            invButtons[x].interactable = true;
            itemCountTXTs[x].gameObject.SetActive(true);
            invImages[x].sprite = item.icon;
            invImages[x].gameObject.SetActive(true);
        }

        infoText.text = inventory.dungeonItems[0].itemInfo;

        // gold default - 1
        invImages[1].sprite = inventory.dungeonItems[1].icon;
        int Gunits = inventory.GetAvailableGold();
        itemCountTXTs[1].text = Gunits.ToString();
        invButtons[1].interactable = true;
        itemCountTXTs[1].color = Color.yellow;
        itemCountTXTs[1].gameObject.SetActive(true);

        //map fragment - 2
        invImages[2].sprite = inventory.dungeonItems[2].icon;
        itemCountTXTs[2].gameObject.SetActive(true);

        if (inventory.mapstatus == InventoryController.MapInventoryStatus.sketched)
        {
            itemCountTXTs[2].text = "0 of 3";
        }
        if (inventory.mapstatus == InventoryController.MapInventoryStatus.outlined)
        {
            itemCountTXTs[2].text = "1 of 3";
        }
        if (inventory.mapstatus == InventoryController.MapInventoryStatus.full)
        {
            itemCountTXTs[2].text = "2 of 3";
        }
        if (inventory.mapstatus == InventoryController.MapInventoryStatus.secret)
        {
            itemCountTXTs[2].text = "Full";
        }
        invButtons[2].interactable = true;
        // import remaining inventory in positions 3+;


        foreach(Button itemBT in invButtons)
        {
            if (shopUI.gameObject.activeSelf)
            {
                itemBT.interactable = false;
                shopUI.itemButtons[0].Select();
            }
            if (!shopUI.gameObject.activeSelf)
            {
                itemBT.interactable = true;
            }

            int x = invButtons.IndexOf(itemBT);
            if (x >= inventory.dungeonItems.Count + inventory.trinketC.activeDunTrinkets.Count)
            {
                itemBT.interactable = false;
                invImages[x].sprite = emptySprite;
                itemCountTXTs[x].gameObject.SetActive(false);
            }          
        }
        invButtons[0].Select();
    }

    public void OpenBattleInventory()
    {
        if (!exitButton.gameObject.activeSelf)
        {
            if (!shopUI.gameObject.activeSelf)
            {
                exitButton.gameObject.SetActive(true);
            }
        }
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        text.text = "Battle Inventory";
        tabIndex = 1;
        invState = InvState.Battle;
        // Set Tabs
        tabs[0].sprite = tabSprites[1];
        tabs[1].sprite = tabSprites[2];
        tabs[2].sprite = tabSprites[0];
        itemCountTXTs[1].color = Color.white;

        // set Potion Count
        inventory.battleItems[0].itemCount = inventory.dungeonItems[0].itemCount;

        foreach (DunItem item in inventory.battleItems)
        {
            if (item.itemName == "Revive")
            {
                int reviveCount = 0;
                foreach (DunItem dun in inventory.dungeonItems)
                {
                    if (dun.itemName == "Revive")
                    {
                        reviveCount = dun.itemCount;
                        break;
                    }
                }
                item.itemCount = reviveCount;
            }
            BattleItem battleComponent = item.GetComponent<BattleItem>();
            int x = inventory.battleItems.IndexOf(battleComponent);
            itemCountTXTs[x].text = item.itemCount.ToString();
            invButtons[x].interactable = true;
            itemCountTXTs[x].gameObject.SetActive(true);
            invImages[x].sprite = item.icon;
            invImages[x].gameObject.SetActive(true);
        }

        foreach (DunItem item in inventory.trinketC.activeBattleTrinkets)
        {
            int x = inventory.trinketC.activeBattleTrinkets.IndexOf(item) + inventory.battleItems.Count;
            itemCountTXTs[x].text = item.itemCount.ToString();
            invButtons[x].interactable = true;
            itemCountTXTs[x].gameObject.SetActive(true);
            invImages[x].sprite = item.icon;
            invImages[x].gameObject.SetActive(true);
        }


        infoText.text = inventory.battleItems[0].itemInfo;
        foreach (Button itemBT in invButtons)
        {
            int x = invButtons.IndexOf(itemBT);
            if (x > 1)
            {
                if (x > inventory.battleItems.Count + inventory.trinketC.activeBattleTrinkets.Count - 1)
                {
                    itemBT.interactable = false;
                    invImages[x].sprite = emptySprite;
                    itemCountTXTs[x].gameObject.SetActive(false);
                }
            }
        }
        invButtons[0].Select();
    }

    void ButtonHighlightChecker()
    {
        foreach (Button bt in invButtons)
        {
            int x = invButtons.IndexOf(bt);
            if (EventSystem.current.currentSelectedGameObject == bt.gameObject)
            {
                buttonIndex = x;
                if (invState == InvState.Dungeon)
                {
                    if (x < inventory.dungeonItems.Count)
                    {
                        if (activeItem == null || activeItem != inventory.dungeonItems[x])
                        {
                            activeItem = inventory.dungeonItems[x];
                        }
                        infoText.text = inventory.dungeonItems[x].itemInfo;
                    }
                    if (x >= inventory.dungeonItems.Count && x < inventory.trinketC.activeDunTrinkets.Count + inventory.dungeonItems.Count)
                    {
                        if (activeItem == null || activeItem != inventory.trinketC.activeDunTrinkets[x - inventory.dungeonItems.Count])
                        {
                            activeItem = inventory.trinketC.activeDunTrinkets[x - inventory.dungeonItems.Count];
                        }
                        infoText.text = inventory.trinketC.activeDunTrinkets[x - inventory.dungeonItems.Count].itemInfo;
                    }


                }
                if (invState == InvState.Battle)
                {
                    if (activeItem == null || activeItem != inventory.battleItems[x])
                    {
                        activeItem = inventory.battleItems[x];
                    }
                    infoText.text = inventory.battleItems[x].itemInfo;
                }
                if (invState == InvState.Key)
                {
                    if (activeItem == null || activeItem != inventory.keyItems[x])
                    {
                        activeItem = inventory.keyItems[x];
                    }
                    infoText.text = inventory.keyItems[x].itemInfo;
                }
            }
        }
    }

    public void OpenKeyInventory()
    {
        if (!exitButton.gameObject.activeSelf)
        {
            if (!shopUI.gameObject.activeSelf)
            {
                exitButton.gameObject.SetActive(true);
            }
        }
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        text.text = "Key Item Inventory";
        tabIndex = 2;
        invState = InvState.Key;
        // Set Tabs
        tabs[0].sprite = tabSprites[2];
        tabs[1].sprite = tabSprites[0];
        tabs[2].sprite = tabSprites[1];
        itemCountTXTs[1].color = Color.white;
        // Choas Gem Default - 0

        invImages[0].sprite = inventory.keyItems[0].icon;
        infoText.text = inventory.keyItems[0].itemInfo;
        itemCountTXTs[0].text = inventory.keyItems[0].itemCount.ToString();

        foreach (Button itemBT in invButtons)
        {
            int x = invButtons.IndexOf(itemBT);
            if (x <= inventory.keyItems.Count - 1)
            {
                invImages[x].sprite = inventory.keyItems[x].icon;
                invImages[x].gameObject.SetActive(true);
                if (!shopUI.gameObject.activeSelf)
                {
                    itemBT.interactable = true;
                }
                if (shopUI.gameObject.activeSelf)
                {
                    itemBT.interactable = false;
                    shopUI.itemButtons[0].Select();
                }
                itemCountTXTs[x].gameObject.SetActive(true);
                itemCountTXTs[x].text = inventory.keyItems[x].itemCount.ToString();
            }
            if (x > inventory.keyItems.Count - 1)
            {
                itemBT.interactable = false;
                invImages[x].sprite = emptySprite;
                itemCountTXTs[x].gameObject.SetActive(false);
            }

        }
        invButtons[0].Select();
    }

    public void OpenTrinketInventory()
    {
        if (!exitButton.gameObject.activeSelf)
        {
            if (!shopUI.gameObject.activeSelf)
            {
                exitButton.gameObject.SetActive(true);
            }
        }
        text.text = "Key Item Inventory";
        tabIndex = 3;
        invState = InvState.Trinket;
    }

    public void JoyStickSwap()
    {
        foreach (Image image in inputImages)
        {
            image.gameObject.SetActive(false);
        }
        foreach (Image image in joyImages)
        {
            image.gameObject.SetActive(true);
        }
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && gameObject.activeSelf && uiController.uiActive)
        {
            bool highlighted = false;
            foreach (Button button in invButtons)
            {
                if (EventSystem.current.currentSelectedGameObject == button)
                {
                    highlighted = true;
                }
            }

            if (!highlighted)
            {
                if (invButtons[0].gameObject.activeSelf)
                {
                    invButtons[0].Select();
                }
            }
        }

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.JoystickButton3))
        {
            if (gameObject.activeSelf && uiController.uiActive && !toggleSwap)
            {
                if (tabIndex == 0)
                {
                    toggleSwap = true;
                    OpenBattleInventory();
                    StartCoroutine(SwapTimer());
                    return;
                }
                if (tabIndex == 1)
                {
                    toggleSwap = true;
                    OpenKeyInventory();
                    StartCoroutine(SwapTimer());
                    return;
                }
                if (tabIndex == 2)
                {
                    toggleSwap = true;
                    OpenInventory();
                    StartCoroutine(SwapTimer());
                    return;
                }
            }
        }

        if (uiController.joystick)
        {
            if (Input.GetKey(KeyCode.Joystick1Button1))
            {
                if (gameObject.activeSelf && uiController.uiActive)
                {
                    uiController.CloseDunInventory();
                }
            }
        }

        ButtonHighlightChecker();
            
    }

}
