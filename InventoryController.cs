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
        int availGold = 0;
        int bankGold = EnhancedPrefs.GetPlayerPref("goldBank", 0);

        availGold = gold + bankGold;

        return availGold;
    }

    public void ReduceGold(int goldCost)
    {
        if (gold >= goldCost)
        {
            gold = gold - goldCost;
        }

        if (gold < goldCost)
        {
            if (GetAvailableGold() >= goldCost)
            {
                int neededGold = goldCost - gold;
                int currentBank = EnhancedPrefs.GetPlayerPref("goldBank", 0);
                int saveAmount = currentBank - neededGold;
                EnhancedPrefs.SetPlayerPref("goldBank", saveAmount);
                EnhancedPrefs.SavePlayerPrefs();
            }
            if (GetAvailableGold() < goldCost)
            {
                Debug.Log("Error, Tried to Reduce Gold Beyond the total Gold Amount in Inventory & Bank");
            }
        }
    }

}
