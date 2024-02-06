using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq.Expressions;
using Unity.VisualScripting;

public class InventoryUI : MonoBehaviour
{
    public enum InvState { Dungeon, Battle, Trinket, Key}
    public InvState invState;
    public DunUIController uiController;
    public InventoryController inventory;
    public SceneController controller;
    public ShopUI shopUI;

    public List<Button> invButtons;
    public Sprite emptySprite;
    public List<Image> invImages;
    public List<Image> tabs;
    public List<Sprite> tabSprites;
    public int tabIndex;
    public int buttonIndex;

    public TextMeshProUGUI text;
    public TextMeshProUGUI infoText;
    public List<TextMeshProUGUI> itemCountTXTs;

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



    public void OpenInventory()
    {
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
        }
       
        infoText.text = inventory.dungeonItems[0].itemInfo;

        // gold default - 1
        invImages[1].sprite = inventory.dungeonItems[1].icon;
        itemCountTXTs[1].text = inventory.gold.ToString();
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
            if (!shopUI.gameObject.activeSelf)
            {
                itemBT.interactable = true;
            }
            if (shopUI.gameObject.activeSelf)
            {
                itemBT.interactable = false;
                shopUI.itemButtons[0].Select();
            }

            int x = invButtons.IndexOf(itemBT);
            if (x > 2)
            {
                if (inventory.dungeonItems.Count == 3)
                {
                    itemBT.interactable = false;
                    invImages[x].sprite = emptySprite;
                    itemCountTXTs[x].gameObject.SetActive(false);
                }

                if (inventory.dungeonItems.Count > 3)
                {
                    if (x < inventory.dungeonItems.Count)
                    {
                        invImages[x].sprite = inventory.dungeonItems[x].icon;
                        invImages[x].gameObject.SetActive(true);
                        itemBT.interactable = true;

                        itemBT.interactable = false;
                        invImages[x].sprite = emptySprite;

                        itemCountTXTs[x].text = inventory.dungeonItems[x].itemCount.ToString();
                    }
                }
            }
        }
        invButtons[0].Select();
    }

    public void OpenBattleInventory()
    {
        text.text = "Battle Inventory";
        tabIndex = 1;
        invState = InvState.Battle;
        // Set Tabs
        tabs[0].sprite = tabSprites[1];
        tabs[1].sprite = tabSprites[2];
        tabs[2].sprite = tabSprites[0];
        itemCountTXTs[1].color = Color.white;
        // Potion Default - 0
      

        infoText.text = inventory.battleItems[0].itemInfo;

        foreach (Button itemBT in invButtons)
        {
            int x = invButtons.IndexOf(itemBT);
            if (x > 0)
            {
                if (x > inventory.battleItems.Count - 1)
                {
                    itemBT.interactable = false;
                    invImages[x].sprite = emptySprite;
                    itemCountTXTs[x].gameObject.SetActive(false);
                }
                if (x <= inventory.battleItems.Count - 1)
                {
                    invImages[x].sprite = inventory.battleItems[x].icon;
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
                    itemCountTXTs[x].text = inventory.battleItems[x].itemCount.ToString();
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
                    infoText.text = inventory.dungeonItems[x].itemInfo;
                }
                if (invState == InvState.Battle)
                {
                    infoText.text = inventory.battleItems[x].itemInfo;
                }
                if (invState == InvState.Key)
                {
                    infoText.text = inventory.keyItems[x].itemInfo;
                }
            }
        }
    }

    public void OpenKeyInventory()
    {
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
