using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : DunItem
{


    public override void PickUp()
    {
        InventoryController inventory = FindAnyObjectByType<InventoryController>();
        MapController map = FindAnyObjectByType<MapController>();

        Debug.Log(itemCount + " " + itemName + " picked up");

        if (inventory.mapstatus == InventoryController.MapInventoryStatus.sketched)
        {
            map.ShowMapOutLine();
            return;
        }
        if (inventory.mapstatus == InventoryController.MapInventoryStatus.outlined)
        {
            map.ShowFullMap(false);
            return;
        }
        if (inventory.mapstatus == InventoryController.MapInventoryStatus.full)
        {
            map.ShowFullMap(true);
            return;
        }

       
    }
}
