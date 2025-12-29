using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItemALL : DunItem
{
    public InventoryController inventory;

    public override void PickUp()
    {
        DunItem selectedItem = DunItemShuffle();

        if (!selectedItem.trinket)
        {
            if (selectedItem.itemType == ItemType.gold)
            {

            }
            if (selectedItem.itemType == ItemType.XP)
            {

            }
            if (selectedItem.itemType == ItemType.dungeon)
            {
                bool inList = false;
                foreach (DunItem dunItem in inventory.dungeonItems)
                {
                    if (dunItem == selectedItem)
                    {
                        dunItem.itemCount = dunItem.itemCount + itemCount;
                        inList = true;
                        gameObject.SetActive(false);
                        break;
                    }
                }

                if (!inList)
                {
                    inventory.dungeonItems.Add(selectedItem);
                    gameObject.SetActive(false);
                }
            }
            if (itemType == ItemType.battle)
            {
                bool inList = false;
                foreach (DunItem battleItem in inventory.battleItems)
                {
                    if (battleItem == selectedItem)
                    {
                        battleItem.itemCount = battleItem.itemCount + itemCount;
                        inList = true;
                        gameObject.SetActive(false);
                        break;
                    }
                }

                if (!inList)
                {
                    BattleItem battleComponent = GetComponent<BattleItem>();
                    inventory.battleItems.Add(battleComponent);
                    gameObject.SetActive(false);
                }
            }
            if (itemType == ItemType.keyItem)
            {
                bool inList = false;
                foreach (DunItem keyItem in inventory.keyItems)
                {
                    if (keyItem.itemName == selectedItem.itemName)
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
                    inventory.keyItems.Add(selectedItem);
                    gameObject.SetActive(false);
                }
            }
        }
        if (trinket)
        {
            DunTrinket activeTrinket = selectedItem.GetComponent<DunTrinket>();
            if (selectedItem.itemType == ItemType.dungeon)
            {
                inventory.trinketC.activeDunTrinkets.Add(selectedItem);

            }
            if (itemType == ItemType.battle)
            {
                inventory.trinketC.activeBattleTrinkets.Add(selectedItem);
                gameObject.SetActive(false);
            }

            activeTrinket.SetTrinket();
        }
        Debug.Log(itemCount + " " + itemName + " picked up");

        DunUIController uiController = FindAnyObjectByType<DunUIController>();
        uiController.rangeImage.gameObject.SetActive(false);
        uiController.customImage.gameObject.SetActive(false);
        uiController.ToggleKeyUI(gameObject, false);
        uiController.pickUpUI.gameObject.SetActive(true);
        uiController.pickUpUI.OpenImage(selectedItem);


    }

    public bool MapCheck()
    {
        bool full = false;
        if (inventory == null)
        {
            inventory = FindAnyObjectByType<InventoryController>();
        }

        if (inventory.mapstatus == InventoryController.MapInventoryStatus.secret)
        {
            full = true;
        }

        return full;
    }

    public DunItem DunItemShuffle()
    {
        DunItem randomItem = null;
        List<DunItem> list = new List<DunItem>();
        if (MapCheck() == false)
        {
            list.Add(inventory.mapItemPrefab);
            list.Add(inventory.mapItemPrefab);
            list.Add(inventory.mapItemPrefab);
            list.Add(inventory.mapItemPrefab);
            list.Add(inventory.mapItemPrefab); // adding 5x for common

        }

        foreach (DunItem poolItem in inventory.treasurePool)
        {
            if (poolItem.rarity == Rarity.common)
            {
                list.Add(poolItem);
                list.Add(poolItem);
                list.Add(poolItem);
                list.Add(poolItem);
                list.Add(poolItem);
            }
            if (poolItem.rarity == Rarity.uncommon)
            {
                list.Add(poolItem);
                list.Add(poolItem);
                list.Add(poolItem);
            }
            if (poolItem.rarity == Rarity.rare)
            {
                list.Add(poolItem);
            }
            if (poolItem.rarity == Rarity.epic)
            {
                int epicChance = Random.Range(0, 5);
                if (epicChance == 0)
                {
                    list.Add(poolItem);
                } 
            }
        }

        randomItem = list[Random.Range(0, list.Count)];


        return randomItem;
    }

}
