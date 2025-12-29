using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureRoomChest : DunChest
{
    public DunPortal roomExitPortal;
    

    public void FillTreasureChest()
    {
        InventoryController inventory = FindAnyObjectByType<InventoryController>();
        List<DunItem> rareRewards = new List<DunItem>();

        foreach (DunItem item in inventory.treasurePool)
        {
            if (item.rarity != DunItem.Rarity.common && item.rarity != DunItem.Rarity.epic)
            {
                rareRewards.Add(item);
            }
        }

  
        int x = 0;
        if (rareRewards.Count == 0)
        {
            chestItem = inventory.randomALLItem;
            Debug.Log("Error, Inventory Treasure Pool does not have Med/Rare Items");
            return;
        }
        if (rareRewards.Count != 0)
        {
            x = Random.Range(0, rareRewards.Count);
            chestItem = rareRewards[x];
            fixedTreasure = true;
        }        
    }


    public override void OpenChest()
    {
        base.OpenChest();
        roomExitPortal.gameObject.SetActive(true);
    }
}
