using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DunItem : MonoBehaviour
{
    public string itemName;
    public enum ItemType { gold, XP, dungeon, battle, keyItem}
    public ItemType itemType;
    public int itemCount;
    public bool inRange;
    public Sprite icon;
    public bool pickUpMessage;

    public int itemPrice;
    public string itemInfo;
    public bool trinket;

    public DunModel dunTarget;
    public BattleModel battleTarget;
    public float itemEffect;

    public enum Rarity { common, uncommon, rare, epic, singular}
    public Rarity rarity;

    public virtual void UseItem(DunModel target = null, BattleModel battleTarget = null)
    {

    }

    public virtual void ConfirmItem()
    {

    }
    public virtual void PickUp()
    {
        InventoryController inventory = FindAnyObjectByType<InventoryController>();
        if (!trinket)
        {
            if (itemType == ItemType.gold)
            {
                inventory.AddGold(itemCount);
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
                        dunItem.itemCount = dunItem.itemCount + 1;
                        inList = true;
                        gameObject.SetActive(false);
                        break;
                    }
                }

                if (!inList)
                {
                    inventory.dungeonItems.Add(this);
                    itemCount++;
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
                        battleItem.itemCount = battleItem.itemCount + 1;
                        inList = true;
                        gameObject.SetActive(false);
                        break;
                    }
                }

                if (!inList)
                {
                    BattleItem battleComponent = GetComponent<BattleItem>();
                    inventory.battleItems.Add(battleComponent);
                    itemCount++;
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
                        keyItem.itemCount = keyItem.itemCount + 1;
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
        }
        if (trinket)
        {
            if (itemType == ItemType.dungeon)
            {
                bool inList = false;
                foreach (DunItem dunItem in inventory.trinketC.activeDunTrinkets)
                {
                    if (dunItem == this)
                    {
                        dunItem.itemCount = dunItem.itemCount + 1;
                        inList = true;
                        gameObject.SetActive(false);
                        break;
                    }
                }

                if (!inList)
                {
                    inventory.trinketC.activeDunTrinkets.Add(this);
                    gameObject.SetActive(false);
                }
            }
            if (itemType == ItemType.battle)
            {
                bool inList = false;
                foreach (DunItem battleItem in inventory.trinketC.activeBattleTrinkets)
                {
                    if (battleItem == this)
                    {
                        battleItem.itemCount = battleItem.itemCount + 1;
                        inList = true;
                        gameObject.SetActive(false);
                        break;
                    }
                }

                if (!inList)
                {
                    inventory.trinketC.activeBattleTrinkets.Add(this);
                    gameObject.SetActive(false);
                }
            }
        }
        Debug.Log(itemCount + " " + itemName + " picked up");
        DunUIController uiController = FindAnyObjectByType<DunUIController>();
        if (pickUpMessage)
        {
            uiController.rangeImage.gameObject.SetActive(false);
            uiController.customImage.gameObject.SetActive(false);
            uiController.ToggleKeyUI(gameObject, false);
            uiController.pickUpUI.gameObject.SetActive(true);
            uiController.pickUpUI.OpenImage(this);
        }
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
