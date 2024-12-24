using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTT.PlayerPrefsEnhanced;

public class InventoryController : MonoBehaviour
{
    public enum MapInventoryStatus { sketched, outlined, full, secret}
    public MapInventoryStatus mapstatus;
    public MapItem mapItemPrefab;
    public int gold;
    public List<DunItem> treasurePool;
    public List<DunItem> dungeonItems;
    public List<BattleItem> battleItems;
    public List<DunItem> keyItems;

    public List<DunItem> masterDungeonItems;
    public List<BattleItem> masterBattleItems;
    public List<DunItem> masterKeyItems;

    public TrinketController trinketC;

    public RandomDunItem randomDunItem;
    public RandomTrinketItem randomDunTrinket;
    public RandomTrinketItem randomBattleTrinket;
    public RandomItemALL randomALLItem;
    public void StartReset()
    {
        foreach (DunItem item in dungeonItems)
        {
            item.itemCount = 0;
        }
        foreach (DunItem item in battleItems)
        {
            item.itemCount = 0;
        }
        foreach (DunItem item in keyItems)
        {
            item.itemCount = 0;
        }


        FillTreasurePool();
    }

    public void FillTreasurePool()
    {
        foreach (DunItem dun in masterDungeonItems) // includes map prefab
        {
            treasurePool.Add(dun);
        }
        foreach (BattleItem bat in masterBattleItems)
        {
            int x = masterBattleItems.IndexOf(bat);
            if (x != 0 && x != 2) // to remove battle potions & revive, mirrored in dungeon items
            {
                treasurePool.Add(bat);
            }
        }
        foreach (DunTrinket dunT in trinketC.masterDunTrinkets)
        {
            treasurePool.Add(dunT);
        }
        foreach (DunTrinket batT in trinketC.masterBattleTrinkets)
        {
            int x = trinketC.masterBattleTrinkets.IndexOf(batT);
            if (x != 1) // to remove puppy trinket
            {
                treasurePool.Add(batT);
            }
        }
    }


    public int GetAvailableGold()
    {
        int availGold = gold;
        return availGold;
    }

    public void AddGold(int goldAdd)
    {
        gold = gold + goldAdd;
        StatsTracker stats = FindObjectOfType<StatsTracker>();
        stats.totalGold = stats.totalGold + goldAdd;

        EnhancedPrefs.SetPlayerPref("goldBank", gold); EnhancedPrefs.SavePlayerPrefs();
    }

    public void ReduceGold(int goldCost)
    {
        if (gold >= goldCost)
        {
            gold = gold - goldCost;
            EnhancedPrefs.SetPlayerPref("goldBank", gold);  EnhancedPrefs.SavePlayerPrefs();
        }

        if (gold < goldCost)
        {
            Debug.Log("Error, Tried to Reduce Gold Beyond the total Gold Amount in Inventory.  Check Gold Check");          
        }
    }

    public void LoadBattleInventory()
    {

    }
}
