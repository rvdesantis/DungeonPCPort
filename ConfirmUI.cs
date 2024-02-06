using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmUI : MonoBehaviour
{
    public enum AttachedUI { BlackSmith, Magic, Campfire, NPC}
    public AttachedUI attachedUI;
    public TextMeshProUGUI message;
    public TextMeshProUGUI confirm;

    public Button yesBT;
    public Button noBT;

    public BlacksmithUI blackSmithUI;
    public DunUIController uiController;
    public CampfireUI campFireUI;
    public CinemachineVirtualCamera activeCAM;
    public PlayerController player;
    public Action targetAction;

    private void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }
    }

    public void ConfirmMessageUI(string mss = "", bool blackSmith = false, bool magic = false, bool campFire = false, bool inputC = false)
    {
        if (blackSmith)
        {
            attachedUI = AttachedUI.BlackSmith;
            message.text = mss;
            targetAction += blackSmithUI.ConfirmUpgrade;
            yesBT.Select();
        }
        if (magic)
        {
            attachedUI = AttachedUI.Magic;
            message.text = mss;
            targetAction += uiController.spellSmithUI.ConfirmUpgrade;
            yesBT.Select();
        }
        if (campFire)
        {
            campFireUI.bankBT.interactable = false;
            campFireUI.partyBT.interactable = false;
            campFireUI.exitBT.interactable = false;

            attachedUI = AttachedUI.Campfire;
            message.text = mss;
            targetAction += campFireUI.Bank;
            gameObject.SetActive(true);
            yesBT.Select();
        }
        if (inputC)
        {
            message.text = mss;
            targetAction += NoButton;
            gameObject.SetActive(true);
            yesBT.Select();
        }

    }

    public void ConfirmMessageNPC(string mss, BlacksmithNPC blacksmith = null, SpellNPC spellSmith = null, ShopNPC shopNPC = null, TemplarNPC templar = null, GobMapVendorNPC goblin = null)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        player.enabled = false;
        targetAction = null;
        if (blacksmith != null)
        {
            message.text = mss;
            targetAction += blacksmith.OpenUI;
            gameObject.SetActive(true);
            yesBT.Select();
        }
        if (templar != null)
        {
            message.text = mss;
            targetAction += templar.TriggerAttackConf;
            gameObject.SetActive(true);
            yesBT.Select();
        }
        if (goblin != null)
        {
            message.text = mss;
            targetAction += goblin.Confirm;
            gameObject.SetActive(true);
            yesBT.Select();
        }

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
        uiController.RemoteToggleTimer();
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
        player.enabled = true;
        uiController.isToggling = true;
        uiController.RemoteToggleTimer();
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
