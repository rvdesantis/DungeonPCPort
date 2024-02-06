using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockedUI : MonoBehaviour
{
    public UnlockController unlockController;
    public SceneController controller;
    public DunUIController uiController;
    public PartyController party;
    public Image unlockImage;
    public RawImage photoBoothRaw;
    public Photobooth photoBooth;
    public TextMeshProUGUI typeTXT;
    public TextMeshProUGUI infoTXT;
    public Button nxtBT;
    public Button exitBT;
    public Action nextUnlockAction;
    public bool toggleTimer;

    public void OpenUnlockUI(string unlockInfo, bool finalUnlock, int unlockIndex, bool character = false, bool npc = false, bool item = false, bool enemy = false)
    {
        uiController.uiActive = true;
        controller.playerController.enabled = false;  
        infoTXT.text = unlockInfo;

        uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[4]);
        if (character)
        {
            typeTXT.text = "New Party Member";
            unlockImage.gameObject.SetActive(false);
            photoBoothRaw.gameObject.SetActive(true);
            photoBooth.SayCheese(party.masterParty[unlockIndex]);
        }
        if (npc)
        {
            typeTXT.text = "New NPC Character";
            unlockImage.gameObject.SetActive(true);
            photoBoothRaw.gameObject.SetActive(false);
        }
        if (item)
        {
            typeTXT.text = "New Item";
            unlockImage.gameObject.SetActive(true);
            photoBoothRaw.gameObject.SetActive(false);
        }
        if (enemy)
        {
            typeTXT.text = "New Enemy";
            unlockImage.gameObject.SetActive(true);
            photoBoothRaw.gameObject.SetActive(false);
        }
        gameObject.SetActive(true);

        if (finalUnlock)
        {
            nxtBT.gameObject.SetActive(false);
            exitBT.gameObject.SetActive(true);
            exitBT.Select();
        }
        if (!finalUnlock)
        {
            exitBT.gameObject.SetActive(false);
            nxtBT.gameObject.SetActive(true);
            nxtBT.Select();
        }
    }

    public void CloseUnlockUI()
    {
        uiController.uiActive = false;
        controller.playerController.enabled = true;
        gameObject.SetActive(false);
    }

    public void NextUnlock()
    {
        nextUnlockAction.Invoke();
    }

    IEnumerator ToggleTimer()
    {
        yield return new WaitForSeconds(.25f);
        toggleTimer = false;
    }

    private void Update()
    {
        if (gameObject.activeSelf && !toggleTimer)
        {
            if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.JoystickButton1))
            {
                toggleTimer = true;
                if (nextUnlockAction == null)
                {
                    CloseUnlockUI();
                    StartCoroutine(ToggleTimer());
                }
                if (nextUnlockAction != null)
                {
                    nextUnlockAction.Invoke();
                    StartCoroutine(ToggleTimer());
                }
            }
        }
    }
}
