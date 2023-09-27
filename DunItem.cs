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
                if (keyItem == this)
                {
                    keyItem.itemCount = keyItem.itemCount + itemCount;                    
                    inList = true;
                    gameObject.SetActive(false);
                    break;
                }
            }

            if (!inList)
            {
                inventory.keyItems.Add(this);
                gameObject.SetActive(false);
            }
        }

        Debug.Log(itemName + " picked up");
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
