using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DTT.PlayerPrefsEnhanced;
using UnityEngine.EventSystems;

public class SpellSmithUI : MonoBehaviour
{
    public SceneController controller;
    public DunUIController uiController;
    public Photobooth photoBooth;
    public PartyController party;
    public SpellNPC activeSpellSmith;
    public DunModel activeModel;
    public InventoryController inventory;
    public ConfirmUI confirmUI;


    public Image currencyImage;
    public List<Sprite> currencyIcons; // 0-gold, 1-XP, 2-Chaos
    public TextMeshProUGUI currencyCost;

    public Image spellImage;

    public Image topLArrow;
    public Image topRArrow;
    public Image midLArrow;
    public Image midRArrow;

    public TextMeshProUGUI textLineOne;
    public TextMeshProUGUI textLineTwo;

    public int index;
    public int partyIndex;
    public int currencyIndex;

    public float activePlier;
    public float upgradeFinalPer;

    public List<Button> rButtons;
    public List<Button> lButtons;

    public Button spellBT; 
    public Button exitBT;

    public bool toggling;
    public List<AudioClip> uiSounds;



    IEnumerator CloseTimer()
    {
        toggling = true;
        spellBT.interactable = true;      
        spellImage.gameObject.SetActive(true);
        uiController.uiActive = false;
        controller.playerController.enabled = true;
        controller.playerController.cinPersonCam.m_Priority = 10;
        activeSpellSmith.opened = false;
        activeSpellSmith.faceCam.m_Priority = -1;
        uiController.isToggling = true;

        photoBooth.ResetBooth();

        yield return new WaitForSeconds(.15f);
        toggling = false;
        uiController.RemoteToggleTimer(.1f);
        gameObject.SetActive(false);
    }
    IEnumerator Toggle()
    {
        yield return new WaitForSeconds(.25f);
        toggling = false;
    }

    public void OpenConfirm() // opened with button in UI
    {
        string conMess = "Are you sure you would like to perminently increase " + activeModel.modelName + "'s Spell Power?";
        string finishMess = "";

        float increaseAmount = 3 * activePlier;
        float currentpercent = 0;
        if (currencyIndex == 2)
        {
            increaseAmount = (1.5f * increaseAmount);
        }
        currentpercent = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "SpellPercent", 0f);
        Debug.Log("Current Power% Bonus Set to " + currentpercent + ". Current increase% set to " + increaseAmount);
        finishMess = " Stat by " + increaseAmount + "%? (Total Boost " + (currentpercent + increaseAmount) + "%)";
        upgradeFinalPer = currentpercent + increaseAmount;
        confirmUI.gameObject.SetActive(true);
        confirmUI.ConfirmMessageUI(conMess + finishMess, false, true);
    }

    public void ConfirmUpgrade()
    {
        int costInt = 0;
        bool funds = true;
        string fundsMessage = "";
        Debug.Log("Checking Funds");
        if (currencyIndex == 0)
        {
            activeSpellSmith.currencyIndex = 0;
            int count = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "SpellUpCount", 0);
            int cost = 100 * (count + 1);
            costInt = cost;
            if (cost > inventory.GetAvailableGold())
            {
                funds = false;
                fundsMessage = "Not Enough Gold To Upgrade " + activeModel.modelName + "'s Spell Power";
            }
            
        }
        if (currencyIndex == 1)
        {
            activeSpellSmith.currencyIndex = 1;
            int count = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "SpellUpCount", 0);
            int cost = 100 * (count + 1);
            int XP = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "XP", 0);
            costInt = cost;
            if (cost > XP)
            {
                Debug.Log("Not Enough Currency");
                funds = false;
                fundsMessage = "Not Enough Gold To Upgrade " + activeModel.modelName + "'s Spell Power";
            }
            if (cost <= XP)
            {

            }   
        }
        if (currencyIndex == 2)
        {
            activeSpellSmith.currencyIndex = 2;
            costInt = 1;
            if (inventory.keyItems[0].itemCount < 1)
            {
                funds = false;
                fundsMessage = "Not Enough Gems To Upgrade " + activeModel.modelName + "'s Spell Power";
            }
        }

        if (funds)
        {
            activeSpellSmith.charIndex = partyIndex;
            activeSpellSmith.UpgradeSpellPower(activeModel, upgradeFinalPer, costInt, currencyIndex);      
            if (activeSpellSmith.singleUse)
            {
                activeSpellSmith.remove = true;
                DistanceController distance = FindObjectOfType<DistanceController>();
                distance.npcS.Remove(activeSpellSmith);

                uiController.interactUI.gameObject.SetActive(false);
                uiController.interactUI.activeObj = null;

                uiController.rangeImage.gameObject.SetActive(false);
                uiController.customImage.gameObject.SetActive(false);
            }
        }

        if (!funds)
        {
            uiController.messagePanelUI.OpenMessage(fundsMessage);
            uiController.messagePanelUI.CloseMessageTimer(4);
        }


        CloseUI();
    }


    public void OpenSmithUI(float multiplier = 1, SpellNPC spellSmith = null, int charIndex = 0, int currencyIndedx = 0)
    {
        if (spellSmith == null)
        {
            Debug.Log("Error, no blacksmith attached to UI");
        }
        if (toggling)
        {
            toggling = false;
        }
        activeSpellSmith = spellSmith;
        uiController.uiActive = true;
        controller.playerController.enabled = false;
        controller.playerController.cinPersonCam.m_Priority = -1;
        spellSmith.faceCam.m_Priority = 10;
        currencyImage.sprite = currencyIcons[currencyIndedx]; // sets to cold by default
        currencyCost.color = Color.yellow;

        topLArrow.gameObject.SetActive(true);
        topRArrow.gameObject.SetActive(true);

        midLArrow.gameObject.SetActive(false);
        midRArrow.gameObject.SetActive(false);

        index = 0;
        partyIndex = charIndex;
        currencyIndex = currencyIndedx;
        activePlier = multiplier;

        activeModel = party.activeParty[charIndex];
        textLineOne.text = "Select Character";
        textLineTwo.text = "(" + activeModel.modelName + ")";

        photoBooth.SayCheese(activeModel);
       
        int x = party.SpellUpCounters[0];
        int price = 100 * (1 + x);
        currencyCost.text = "";

        gameObject.SetActive(true);
        rButtons[0].Select();
    }

    public void CloseUI()
    {
        StartCoroutine(CloseTimer());
    }

    public void ArrowRTop()
    {
        uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
        toggling = true;
        int abc = partyIndex;
        rButtons[0].Select();
        if (abc == 2)
        {
            activeModel = party.activeParty[0];
            partyIndex = 0;
            textLineOne.text = "Select Character";
            textLineTwo.text = activeModel.modelName;

            photoBooth.SayCheese(activeModel);


            StartCoroutine(Toggle());
            return;
        }
        if (abc == 1)
        {
            activeModel = party.activeParty[2];
            partyIndex = 2;
            textLineOne.text = "Select Character";
            textLineTwo.text = activeModel.modelName;

            photoBooth.SayCheese(activeModel);


            StartCoroutine(Toggle());
            return;
        }
        if (abc == 0)
        {
            activeModel = party.activeParty[1];
            partyIndex = 1;
            textLineOne.text = "Select Character";
            textLineTwo.text = activeModel.modelName;

            photoBooth.SayCheese(activeModel);


            StartCoroutine(Toggle());
            return;
        }
    }

    public void ArrowRMid()
    {
        uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
        toggling = true;
        currencyIndex++;
        if (currencyIndex == 3)
        {
            currencyIndex = 0;
        }
        rButtons[1].Select();

        textLineOne.text = "Select Currency";
        if (currencyIndex == 0)
        {
            textLineTwo.text = "Available Gold (" + inventory.GetAvailableGold() + ")";
        }
        if (currencyIndex == 1)
        {
            int availXP = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "XP", 0);
            textLineTwo.text = activeModel.modelName + " XP (" + availXP + ")";
        }
        if (currencyIndex == 2)
        {
            textLineTwo.text = "Available Gems (" + inventory.keyItems[0].itemCount + ")";
        }
        currencyImage.sprite = currencyIcons[currencyIndex];
        StartCoroutine(Toggle());
        return;
    }

    public void ArrowLTop()
    {
        uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
        toggling = true;
        lButtons[0].Select();
        int abc = party.activeParty.IndexOf(activeModel);

        if (abc == 2)
        {
            activeModel = party.activeParty[1];
            partyIndex = 1;
            textLineOne.text = "Select Character";
            textLineTwo.text = activeModel.modelName;
            photoBooth.SayCheese(activeModel);


            StartCoroutine(Toggle());
            return;
        }
        if (abc == 1)
        {
            activeModel = party.activeParty[0];
            partyIndex = 0;
            textLineOne.text = "Select Character";
            textLineTwo.text = activeModel.modelName;

            photoBooth.SayCheese(activeModel);


            StartCoroutine(Toggle());
            return;
        }
        if (abc == 0)
        {
            activeModel = party.activeParty[2];
            partyIndex = 2;
            textLineOne.text = "Select Character";
            textLineTwo.text = activeModel.modelName;

            photoBooth.SayCheese(activeModel);


            StartCoroutine(Toggle());
            return;
        }
    }

    public void ArrowLMid()
    {
        uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
        toggling = true;
        currencyIndex--;
        if (currencyIndex == -1)
        {
            currencyIndex = 2;
        }
        rButtons[1].Select();
        textLineOne.text = "Select Currency";
        if (currencyIndex == 0)
        {

            textLineTwo.text = "Available Gold (" + inventory.GetAvailableGold() + ")";
        }
        if (currencyIndex == 1)
        {
            int availXP = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "XP", 0);
            textLineTwo.text = activeModel.modelName + " XP (" + availXP + ")";
        }
        if (currencyIndex == 2)
        {
            textLineTwo.text = "Available Gems (" + inventory.keyItems[0].itemCount + ")";
        }
        currencyImage.sprite = currencyIcons[currencyIndex];
        StartCoroutine(Toggle());
        return;
    }

    void UIUp()
    {
        toggling = true;
   
        if (index > 0)
        {
            index--;
            if (index == 3)
            {
                uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
                exitBT.Select();
                textLineOne.text = "Close Menu?";
                textLineTwo.text = "";
                currencyCost.text = "";
                topLArrow.gameObject.SetActive(false);
                topRArrow.gameObject.SetActive(false);
                midLArrow.gameObject.SetActive(false);
                midRArrow.gameObject.SetActive(false);
            }
            if (index == 2)
            {
                uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
                topLArrow.gameObject.SetActive(false);
                topRArrow.gameObject.SetActive(false);
                midLArrow.gameObject.SetActive(false);
                midRArrow.gameObject.SetActive(false);
                spellBT.Select();   
                textLineOne.text = "Permanently Increase Spell Power?";
                if (currencyIndex == 0)
                {
                    int x = party.SpellUpCounters[partyIndex];
                    int price = 100 * (x + 1);
                    currencyCost.text = "Price (Gold): " + price;
                    textLineTwo.text = "Available Gold (" + inventory.GetAvailableGold() + ")";
                }
                if (currencyIndex == 1)
                {
                    int x = party.SpellUpCounters[partyIndex];
                    int price = 100 * (x + 1);
                    currencyCost.text = "Price (XP): " + price;
                    int availXP = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "XP", 0);
                    textLineTwo.text = activeModel.modelName + " XP (" + availXP + ")";
                }
                if (currencyIndex == 2)
                {
                    currencyCost.text = "Price (Gem): " + 1;
                    textLineTwo.text = "Chaos Gem (" + inventory.keyItems[0].itemCount + ")";
                }

            }
            if (index == 1)
            {
                uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
                topLArrow.gameObject.SetActive(false);
                topRArrow.gameObject.SetActive(false);
                midLArrow.gameObject.SetActive(true);
                midRArrow.gameObject.SetActive(true);
                textLineOne.text = "Select Currency";
                currencyCost.text = "";
                if (currencyIndex == 0)
                {
                    textLineTwo.text = "Available Gold (" + inventory.GetAvailableGold() + ")";
                }
                if (currencyIndex == 1)
                {
                    int availXP = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "XP", 0);
                    textLineTwo.text = activeModel.modelName + " XP (" + availXP + ")";
                }
                if (currencyIndex == 2)
                {
                    textLineTwo.text = "Chaos Gem (" + inventory.keyItems[0].itemCount + ")";
                }
            }
            if (index == 0)
            {
                uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
                topLArrow.gameObject.SetActive(true);
                topRArrow.gameObject.SetActive(true);
                midLArrow.gameObject.SetActive(false);
                midRArrow.gameObject.SetActive(false);
                textLineOne.text = "Select Character";
                textLineTwo.text = activeModel.modelName;
                currencyCost.text = "";
            }
        }
        StartCoroutine(Toggle());
    }

    void UIDown()
    {
        toggling = true;
        uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
        if (index < 3)
        {
            index++;
            if (index == 3)
            {
                exitBT.Select();
                textLineOne.text = "Close Menu?";
                textLineTwo.text = "";
                currencyCost.text = "";
                topLArrow.gameObject.SetActive(false);
                topRArrow.gameObject.SetActive(false);
                midLArrow.gameObject.SetActive(false);
                midRArrow.gameObject.SetActive(false);
            }
            if (index == 2)
            {
                topLArrow.gameObject.SetActive(false);
                topRArrow.gameObject.SetActive(false);
                midLArrow.gameObject.SetActive(false);
                midRArrow.gameObject.SetActive(false);
                spellBT.Select();
                textLineOne.text = "Permanently Increase Spell Power?";
                if (currencyIndex == 0)
                {
                    int x = party.SpellUpCounters[partyIndex];
                    int price = 100 * (x + 1);
                    currencyCost.text = "Price (Gold): " + price;
                    textLineTwo.text = "Available Gold (" + inventory.GetAvailableGold() + ")";
                }
                if (currencyIndex == 1)
                {
                    int x = party.SpellUpCounters[partyIndex];
                    int price = 100 * (x + 1);
                    currencyCost.text = "Price (XP): " + price;
                    int availXP = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "XP", 0);
                    textLineTwo.text = activeModel.modelName + " XP (" + availXP + ")";
                }
                if (currencyIndex == 2)
                {
                    currencyCost.text = "Price (Gem): " + 1;
                    textLineTwo.text = "Chaos Gem (" + inventory.keyItems[0].itemCount + ")";
                }
            }
            if (index == 1)
            {
                topLArrow.gameObject.SetActive(false);
                topRArrow.gameObject.SetActive(false);
                midLArrow.gameObject.SetActive(true);
                midRArrow.gameObject.SetActive(true);
                textLineOne.text = "Select Currency";
                currencyCost.text = "";
                if (currencyIndex == 0)
                {
                    textLineTwo.text = "Available Gold (" + inventory.GetAvailableGold() + ")";
                }
                if (currencyIndex == 1)
                {
                    int availXP = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "XP", 0);
                    textLineTwo.text = activeModel.modelName + " XP (" + availXP + ")";
                }
                if (currencyIndex == 2)
                {
                    textLineTwo.text = "Chaos Gem (" + inventory.keyItems[0].itemCount + ")";
                }
            }
            if (index == 0)
            {
                topLArrow.gameObject.SetActive(true);
                topRArrow.gameObject.SetActive(true);
                midLArrow.gameObject.SetActive(false);
                midRArrow.gameObject.SetActive(false);
                textLineOne.text = "Select Character";
                textLineTwo.text = activeModel.modelName;
                currencyCost.text = "";
            }
        }
        StartCoroutine(Toggle());
    }

    void UIRight()
    {
       
        if (index == 0) // character select
        {
            uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
            toggling = true;
            int abc = partyIndex;
            rButtons[0].Select();
            if (abc == 2)
            {
                activeModel = party.activeParty[0];
                partyIndex = 0;
                textLineOne.text = "Select Character";
                textLineTwo.text = activeModel.modelName;

                photoBooth.SayCheese(activeModel);
              

                StartCoroutine(Toggle());
                return;
            }
            if (abc == 1)
            {
                activeModel = party.activeParty[2];
                partyIndex = 2;
                textLineOne.text = "Select Character";
                textLineTwo.text = activeModel.modelName;

                photoBooth.SayCheese(activeModel);


                StartCoroutine(Toggle());
                return;
            }
            if (abc == 0)
            {
                activeModel = party.activeParty[1];
                partyIndex = 1;
                textLineOne.text = "Select Character";
                textLineTwo.text = activeModel.modelName;

                photoBooth.SayCheese(activeModel);


                StartCoroutine(Toggle());
                return;
            }
        }
        if (index == 1) // currency select
        {
            uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
            toggling = true;
            currencyIndex++;
            if (currencyIndex == 3)
            {
                currencyIndex = 0;
            }
            rButtons[1].Select();

            textLineOne.text = "Select Currency";
            if (currencyIndex == 0)
            {
                textLineTwo.text = "Available Gold (" + inventory.GetAvailableGold() + ")";
            }
            if (currencyIndex == 1)
            {   
                int availXP = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "XP", 0);
                textLineTwo.text = activeModel.modelName + " XP (" + availXP + ")";
            }
            if (currencyIndex == 2)
            {
                textLineTwo.text = "Available Gems (" + inventory.keyItems[0].itemCount + ")";
            }
            currencyImage.sprite = currencyIcons[currencyIndex];
            StartCoroutine(Toggle());
            return;
        }
    }

    void UILeft()
    {

        if (index == 0) // character select
        {
            uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
            toggling = true;
            lButtons[0].Select();
            int abc = party.activeParty.IndexOf(activeModel);

            if (abc == 2)
            {
                activeModel = party.activeParty[1];
                partyIndex = 1;
                textLineOne.text = "Select Character";
                textLineTwo.text = activeModel.modelName;
                photoBooth.SayCheese(activeModel);
             
 
                StartCoroutine(Toggle());
                return;
            }
            if (abc == 1)
            {
                activeModel = party.activeParty[0];
                partyIndex = 0;
                textLineOne.text = "Select Character";
                textLineTwo.text = activeModel.modelName;

                photoBooth.SayCheese(activeModel);

  
                StartCoroutine(Toggle());
                return;
            }
            if (abc == 0)
            {
                activeModel = party.activeParty[2];
                partyIndex = 2;
                textLineOne.text = "Select Character";
                textLineTwo.text = activeModel.modelName;

                photoBooth.SayCheese(activeModel);

    
                StartCoroutine(Toggle());
                return;
            }

        }
        if (index == 1) // currency select
        {
            uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
            toggling = true;
            currencyIndex--;
            if (currencyIndex == -1)
            {
                currencyIndex = 2;
            }
            rButtons[1].Select();
            textLineOne.text = "Select Currency";
            if (currencyIndex == 0)
            {

                textLineTwo.text = "Available Gold (" + inventory.GetAvailableGold() + ")";
            }
            if (currencyIndex == 1)
            { 
                int availXP = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "XP", 0);
                textLineTwo.text = activeModel.modelName + " XP (" + availXP + ")";
            }
            if (currencyIndex == 2)
            { 
                textLineTwo.text = "Available Gems (" + inventory.keyItems[0].itemCount + ")";
            }
            currencyImage.sprite = currencyIcons[currencyIndex];
            StartCoroutine(Toggle());
            return;
        }

    }

    private void Update()
    {
        float joystickHorizontalInput = Input.GetAxis("Joystick Horizontal");
        float joystickVerticalInput = Input.GetAxis("Joystick Vertical");

        if (!toggling && !confirmUI.gameObject.activeSelf)
        {
            if (Input.GetKey(KeyCode.W) || joystickVerticalInput < -0.1f)
            {
                UIUp();
            }

            if (Input.GetKey(KeyCode.S) || joystickVerticalInput > 0.1f)
            {
                UIDown();
            }

            if (Input.GetKey(KeyCode.D) || joystickHorizontalInput > 0.1f)
            {
                UIRight();
            }

            if (Input.GetKey(KeyCode.A) || joystickHorizontalInput < -0.1f)
            {
                UILeft();
            }
        }

        if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Joystick1Button1))
        {
            CloseUI();
        }
    }
}
