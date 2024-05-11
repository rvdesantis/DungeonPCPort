using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PickUpUI : MonoBehaviour
{
    public SceneController controller;
    public DunUIController uiController;
    public TextMeshProUGUI itemTitle;
    public TextMeshProUGUI itemInfo;

    public Image itemImage;
    public Button exitBT;
    public GameObject joyParent;

    public Action afterAction;

    public void OpenImage(DunItem item)
    {
        uiController.uiActive = true;
        uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[1]);
        controller.playerController.enabled = false;
        itemTitle.text = item.itemName + " (" + item.itemCount.ToString() + ")";
        itemInfo.text = item.itemInfo;
        itemImage.sprite = item.icon;
        exitBT.Select();
    }

    public void ClosePanel()
    {
        uiController.uiActive = false;
        controller.playerController.enabled = true;
        if (afterAction != null)
        {
            afterAction.Invoke();   
        }
        afterAction = null;
        PlayerController player = uiController.controller.playerController;
        player.enabled = true;
        uiController.uiActive = false;
        gameObject.SetActive(false);


    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if (uiController.joystick)
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button1))
                {
                    ClosePanel();
                    return;
                }                
            }

            if (EventSystem.current.currentSelectedGameObject != exitBT)
            {
                exitBT.Select();
            }
        }
    }

}
