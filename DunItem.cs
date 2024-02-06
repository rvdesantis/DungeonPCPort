using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class DunItem : MonoBehaviour
{
    public string itemName;
    public enum ItemType { gold, XP, dungeon, battle, keyItem}
    public ItemType itemType;
    public int itemCount;
    public bool inRange;
    public Sprite icon;
    public int itemPrice;
    public string itemInfo;

    public virtual void PickUp()
    {
        InventoryController inventory = FindObjectOfType<InventoryController>();

        if (itemType == ItemType.gold)
        {
            inventory.gold = inventory.gold + itemCount;
        }
        if (itemType == ItemType.XP)
        {

        }
        if (itemType == ItemType.dungeon)
        {
            bool inList = false;
            foreach (DunItem dunItem in inventory.dungeonItems)
            {
                if (dunItem == this)
                {
                    dunItem.itemCount = dunItem.itemCount + itemCount;
                    inList = true;
                    gameObject.SetActive(false);
                    break;
                }
            }

            if (!inList)
            {
                inventory.dungeonItems.Add(this);
                gameObject.SetActive(false);
            }
        }
        if (itemType == ItemType.battle)
        {
            bool inList = false;
            foreach (DunItem battleItem in inventory.battleItems)
            {
                if (battleItem == this)
                {
                    battleItem.itemCount = battleItem.itemCount + itemCount;
                    inList = true;
                    gameObject.SetActive(false);
                    break;
                }
            }

            if (!inList)
            {
                inventory.battleItems.Add(this);
                gameObject.SetActive(false);
            }
        }
        if (itemType == ItemType.keyItem)
        {
            bool inList = false;
            foreach (DunItem keyItem in inventory.keyItems)
            {
                if (keyItem.itemName == itemName)
                {
                    keyItem.itemCount = keyItem.itemCount + itemCount;                    
                    inList = true;
                    gameObject.SetActive(false);
                    break;
                }
            }

            if (!inList)
            {
                itemCount = 1;
                inventory.keyItems.Add(this);
                gameObject.SetActive(false);
            }
        }

        Debug.Log(itemCount + " " + itemName + " picked up");

        DunUIController uiController = FindObjectOfType<DunUIController>();
        uiController.rangeImage.gameObject.SetActive(false);
        uiController.customImage.gameObject.SetActive(false);
        uiController.ToggleKeyUI(gameObject, false);
        uiController.pickUpUI.gameObject.SetActive(true);
        uiController.pickUpUI.OpenImage(this);
    }

    private void Update()
    {
        if (inRange)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                PickUp();
            }
        }
    }
}
