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
    public List<DunItem> dungeonItems;
    public List<DunItem> battleItems;
    public List<DunItem> keyItems;

    public List<DunItem> masterDungeonItems;
    public List<DunItem> masterBattleItems;
    public List<DunItem> masterKeyItems;

    public TrinketController trinketC;

    public RandomDunItem randomDunItem;
    public RandomTrinketItem randomDunTrinket;


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
    }

    public int GetAvailableGold()
    {
        int availGold = gold;
        return availGold;
    }

    public void AddGold(int goldAdd)
    {
        gold = gold + goldAdd;
        EnhancedPrefs.SetPlayerPref("goldBank", gold);
        EnhancedPrefs.SavePlayerPrefs();
    }

    public void ReduceGold(int goldCost)
    {
        if (gold >= goldCost)
        {
            gold = gold - goldCost;
        }

        if (gold < goldCost)
        {
            Debug.Log("Error, Tried to Reduce Gold Beyond the total Gold Amount in Inventory.  Check Gold Check");          
        }
    }

}
