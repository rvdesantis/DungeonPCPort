using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class GobMapVendorNPC : DunNPC
{
    public ConfirmUI confirmUI;
    public MessagePanelUI messageUI;
    public MapController mapC;
    public int mapCost;
    public InventoryController inventory;
    public DunUIController uiController;
    public bool toggling;
    public bool soldOut;

    public override void NPCTrigger()
    {
        if (uiController.controller.dunFull && uiController.controller.gameState == SceneController.GameState.Dungeon)
        {
            if (!toggling && !uiController.isToggling && !soldOut)
            {
                audioSource.PlayOneShot(audioClips[0]);
                if (inventory.mapstatus == InventoryController.MapInventoryStatus.sketched)
                {
                    mapCost = 50;
                }
                if (inventory.mapstatus == InventoryController.MapInventoryStatus.outlined)
                {
                    mapCost = 250;
                }

                string confirmMSS = "Buy 1 Map Fragement from Goblin Map Vendor for " + mapCost + " Gold?\nGold In Inventory (" + inventory.GetAvailableGold() + ")";
                if (inventory.GetAvailableGold() < mapCost)
                {
                    confirmMSS = confirmMSS + "\n**NOT ENOUGH GOLD**";
                }
                faceCam.m_Priority = 10;
                confirmUI.activeCAM = faceCam;
                confirmUI.ConfirmMessageNPC(confirmMSS, null, null, null, this);
                return;
            }

            if (!toggling && !uiController.isToggling && soldOut)
            {
                MessageTimer("Map Vendor is Sold Out!");
            }
        }
        if (!uiController.controller.dunFull)
        {
            MessagePanelUI message = uiController.messagePanelUI;

            message.OpenMessage("Map Vendor will open after dungeon build is complete");
            message.CloseMessageTimer(3);
        }
       

    }

    IEnumerator MessageTimer(string mss)
    {
        if (soldOut)
        {
            DistanceController distance = FindAnyObjectByType<DistanceController>();        
        }
        toggling = true;
        messageUI.OpenMessage(mss);
        yield return new WaitForSeconds(3);
        messageUI.gameObject.SetActive(false);
        toggling = false;
    }

    IEnumerator ToggleTimer()
    {
        yield return new WaitForSeconds(.25f);
        toggling = false;
    }

    public override void Confirm()
    {
        if (inventory.GetAvailableGold() < mapCost)
        {
            string mss = "Not Enough Gold!";
            StartCoroutine(MessageTimer(mss));
        }
        if (inventory.GetAvailableGold() >= mapCost)
        {
            string mss = "Map Fragment Purchased!";
            inventory.ReduceGold(mapCost);
            inventory.mapItemPrefab.PickUp();
            if (inventory.mapstatus == InventoryController.MapInventoryStatus.full)
            {
                soldOut = true;                
            }
            StartCoroutine(MessageTimer(mss));
        }

        StartCoroutine(ToggleTimer());
    }


}
