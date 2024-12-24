using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmUI : MonoBehaviour
{
    public enum AttachedUI { BlackSmith, Magic, Campfire, NPC, Shop, Inventory}
    public AttachedUI attachedUI;
    public TextMeshProUGUI message;
    public TextMeshProUGUI confirm;

    public Button yesBT;
    public Button noBT;

    public BlacksmithUI blackSmithUI;
    public ShopUI shopUI;
    public DunUIController uiController;
    public InventoryUI inventoryUI;
    public CampfireUI campFireUI;

    public CinemachineVirtualCamera activeCAM;
    public PlayerController player;
    public Action targetAction;
    public Action noAction;

    private void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }
    }

    public void ConfirmMessageUI(string mss = "", bool blackSmith = false, bool magic = false, bool campFire = false, bool inputC = false, bool inventory = false, bool bossRoom = false)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }    
        message.text = mss;
        if (blackSmith)
        {
            attachedUI = AttachedUI.BlackSmith;           
            targetAction += blackSmithUI.ConfirmUpgrade;   
        }
        if (magic)
        {
            attachedUI = AttachedUI.Magic;          
            targetAction += uiController.spellSmithUI.ConfirmUpgrade;      
        }       
        if (inputC)
        {
            targetAction += NoButton;          
        }
        if (inventory)
        {
            attachedUI = AttachedUI.Inventory;
            targetAction += inventoryUI.TriggerUseItem;
        }
        yesBT.Select();

    }

    public void ConfirmMessageNPC(string mss, BlacksmithNPC blacksmith = null, ShopUI ShopUI = null, TemplarNPC templar = null, GobMapVendorNPC goblin = null, NecromancerNPC necro = null)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        player.enabled = false;
    
        if (blacksmith != null)
        {
            targetAction = null;
            message.text = mss;
            targetAction += blacksmith.OpenUI;
            gameObject.SetActive(true);
            yesBT.Select();
        }
        if (templar != null)
        {
            targetAction = null;
            message.text = mss;
            targetAction += templar.TriggerAttackConf;
            gameObject.SetActive(true);
            yesBT.Select();
        }
        if (goblin != null)
        {
            targetAction = null;
            message.text = mss;
            targetAction += goblin.Confirm;
            gameObject.SetActive(true);
            yesBT.Select();
        }
        if (ShopUI) // target action nulled and set by ShopUI
        {
            attachedUI = AttachedUI.Shop;
            message.text = mss;
            gameObject.SetActive(true);
            yesBT.Select();
        }
        if (necro != null)
        {
            targetAction = null;
            message.text = mss;
            targetAction += necro.Resurrect;
            gameObject.SetActive(true);
            yesBT.Select();
        }
    }

    public void ConfirmItem(string mss, DunItem item)
    {
        Debug.Log("Starting Item ConfirmUI");
        message.text = mss;
        targetAction = item.ConfirmItem;
        yesBT.Select();
    }



    public void YesButton()
    {
        targetAction.Invoke();
        targetAction = null;
        if (activeCAM != null)
        {
            activeCAM.m_Priority = -5;
            activeCAM = null;
        }
        if (attachedUI == AttachedUI.Campfire)
        {
            campFireUI.bankBT.interactable = true;
            campFireUI.partyBT.interactable = true;
            campFireUI.exitBT.interactable = true;

            campFireUI.exitBT.Select();
        }
        uiController.isToggling = true;
        uiController.RemoteToggleTimer(.1f);
        player.enabled = true;
        gameObject.SetActive(false);
    }

    public void NoButton()
    {
        targetAction = null;
        if (activeCAM != null)
        {
            activeCAM.m_Priority = -5;
            activeCAM = null;
        }
        if (attachedUI == AttachedUI.BlackSmith)
        {
            blackSmithUI.exitBT.Select();
        }
        if (attachedUI == AttachedUI.Campfire)
        {
            campFireUI.bankBT.interactable = true;
            campFireUI.partyBT.interactable = true;
            campFireUI.exitBT.interactable = true;

            campFireUI.exitBT.Select();
        }
        if (attachedUI == AttachedUI.Shop)
        {
            foreach (Button bt in shopUI.itemButtons)
            {
                int x = shopUI.itemButtons.IndexOf(bt);
                int count = shopUI.currentShop.itemsForSale.Count;
                if (x < count)
                {
                    bt.interactable = true;
                }
            }
            shopUI.itemButtons[0].Select();
        }
        if (attachedUI == AttachedUI.Inventory)
        {
            inventoryUI.BackUseItem();
        }
        player.enabled = true;
        player.controller.enabled = true;
        uiController.isToggling = true;
        if (noAction != null)
        {
            noAction.Invoke();
            noAction = null;
        }
        uiController.RemoteToggleTimer(.1f);
        gameObject.SetActive(false);

    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.JoystickButton1))
        {
            NoButton();
        }
    }

}
