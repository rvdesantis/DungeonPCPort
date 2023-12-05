using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmUI : MonoBehaviour
{
    public enum AttachedUI { BlackSmith, Magic, Campfire}
    public AttachedUI attachedUI;
    public TextMeshProUGUI message;
    public TextMeshProUGUI confirm;

    public Button yesBT;
    public Button noBT;

    public BlacksmithUI blackSmithUI;
    public DunUIController uiController;
    public CampfireUI campFireUI;

    public Action targetAction;

    public void ConfirmMessageUI(string mss = "", bool blackSmith = false, bool magic = false, bool campFire = false)
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
    }

    public void ConfirmMessageNPC(string mss, BlacksmithNPC blacksmith = null, TemplarNPC templar = null)
    {
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

    }



    public void YesButton()
    {
        targetAction.Invoke();
        targetAction = null;

        if (attachedUI == AttachedUI.Campfire)
        {
            campFireUI.bankBT.interactable = true;
            campFireUI.partyBT.interactable = true;
            campFireUI.exitBT.interactable = true;

            campFireUI.exitBT.Select();
        }

        gameObject.SetActive(false);
    }

    public void NoButton()
    {
        targetAction = null;
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
        gameObject.SetActive(false);

    }
        
}
