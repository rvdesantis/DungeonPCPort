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

}
