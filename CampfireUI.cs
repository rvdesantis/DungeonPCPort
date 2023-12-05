using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DTT.PlayerPrefsEnhanced;
using UnityEngine.InputSystem.XR;

public class CampfireUI : MonoBehaviour
{
    public SceneController controller;
    public DunUIController uiController;
    public Button bankBT;
    public Button partyBT;
    public Button exitBT;
    public string bankString;
    public string partyString;
    public TextMeshProUGUI infoTXT;
    public ConfirmUI confirmUI;
    public List<AudioClip> uiSounds; // 0 - move, 1 - press, - 2 gold, 3 - confirm

    public void ToggleCampFireUI(bool openThis)
    {
        uiController.isToggling = true;  

        if (openThis)
        {          
            uiController.uiActive = true;
            controller.playerController.enabled = false;
            gameObject.SetActive(true);
            bankBT.Select();
        }

        if (!openThis)
        {
            uiController.uiActive = false;
            controller.playerController.enabled = true;
            gameObject.SetActive(false);
        }
    }

    public void Bank()
    {
        uiController.uiAudioSource.PlayOneShot(uiSounds[3]);
        SaveController save = FindObjectOfType<SaveController>();
        save.HomeBank();
    }

    public void BankButton()
    {
        uiController.uiAudioSource.PlayOneShot(uiSounds[2]);
        confirmUI.ConfirmMessageUI(bankString, false, false, true);
    }

    public void PartyButton()
    {

    }

    public void ExitButton()
    {
        uiController.RemoteToggleTimer();
        ToggleCampFireUI(false);
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == bankBT.gameObject)
        {
            if (infoTXT.text != bankString)
            {
                infoTXT.text = bankString;
                uiController.uiAudioSource.PlayOneShot(uiSounds[0]);
            }
        }

        if (EventSystem.current.currentSelectedGameObject == partyBT.gameObject)
        {
            if (infoTXT.text != partyString)
            {
                infoTXT.text = partyString;
                uiController.uiAudioSource.PlayOneShot(uiSounds[0]);
            }
        }

        if (EventSystem.current.currentSelectedGameObject == exitBT.gameObject)
        {
            if (infoTXT.text != "Close Campfire Menu?")
            {
                infoTXT.text = "Close Campfire Menu?";
                uiController.uiAudioSource.PlayOneShot(uiSounds[0]);
            }
        }

        if (gameObject.activeSelf)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                uiController.uiAudioSource.PlayOneShot(uiSounds[1]);
                ToggleCampFireUI(false);
            }
        }
    }
}
