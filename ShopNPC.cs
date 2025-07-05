using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : DunNPC
{
    public List<DunItem> itemsForSale;
    public enum ShopType { dungeonItems, battleItems, keyItems}
    public ShopType shopType;

    public override void NPCTrigger()
    {
        SceneController controller = FindObjectOfType<SceneController>();
        if (controller.gameState == SceneController.GameState.Dungeon)
        {
            if (controller.dunFull)
            {
                OpenUI();
            }
            if (!controller.dunFull)
            {
                MessagePanelUI message = controller.uiController.messagePanelUI;

                message.OpenMessage("Shop will open after dungeon build is complete");
                message.CloseMessageTimer(3);
            }
        }     
    }
    public override void OpenUI()
    {
        DunUIController uiController = FindObjectOfType<DunUIController>();
        PlayerController player = FindObjectOfType<PlayerController>();

        if (!uiController.isToggling && !uiController.uiActive)
        {
            opened = true;

            ShopUI shopUI = uiController.shopUI;

            if (shopUI != null)
            {
                player.controller.enabled = false;
                if (faceCam != null)
                {
                    faceCam.gameObject.SetActive(true);
                    faceCam.m_Priority = 20;
                }
                shopUI.dungeonUI = true; // opens current DunInventory by default
                shopUI.battleUI = false;
                shopUI.keyUI = false;


                shopUI.OpenShopUI(this);
            }
            if (shopUI == null)
            {
                Debug.Log("ERROR: Did not capture Shop UI from uiController", gameObject);
            }
        }
    }
}
