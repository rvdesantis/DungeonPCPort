using DTT.PlayerPrefsEnhanced;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class BlacksmithUI : MonoBehaviour
{
    public SceneController controller;
    public DunUIController uiController;
    public Photobooth photoBooth;
    public PartyController party;
    public BlacksmithNPC activeBlackSmith;
    public DunModel activeModel;
    public InventoryController inventory;
    public ConfirmUI confirmUI;


    public Image currencyImage;
    public List<Sprite> currencyIcons; // 0-gold, 1-XP, 2-Chaos
    public TextMeshProUGUI currencyCost;

    public Image weaponImage;
    public Image armorImage;

    public Image topLArrow;
    public Image topRArrow;
    public Image midLArrow;
    public Image midRArrow;

    public TextMeshProUGUI textLineOne;
    public TextMeshProUGUI textLineTwo;

    public int index;
    public int partyIndex;
    public int currencyIndex;

    public int upgradeIndex;

    public float activePlier;
    public float upgradeFinalPer;

    public List<Button> rButtons;
    public List<Button> lButtons;

    public Button weaponBT;
    public Button armorBT;
    public Button exitBT;

    public bool toggling;
    public List<AudioClip> uiSounds;

    void ButtonChecker()
    {
        if (EventSystem.current.currentSelectedGameObject == weaponBT.gameObject)
        {
            textLineOne.text = "Permanently Increase Power?";
            if (currencyIndex == 0)
            {
                int x = party.PowerUpCounters[partyIndex];
                int price = 100 * (x + 1);
                currencyCost.text = "(Level " + x + ") Price (Gold): " + price;
                textLineOne.text = textLineOne.text + "(L" + x + ")";
                textLineTwo.text = "Available Gold (" + inventory.GetAvailableGold() + ")";
            }
            if (currencyIndex == 1)
            {
                int x = party.PowerUpCounters[partyIndex];
                int price = 100 * (x + 1);
                currencyCost.text = "(Level " + x + ") Price (XP): " + price;
                int availXP = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "XP", 0);
                textLineOne.text = textLineOne.text + "(L" + x + ")";
                textLineTwo.text = activeModel.modelName + " XP (" + availXP + ")";
            }
            if (currencyIndex == 2)
            {
                int x = party.PowerUpCounters[partyIndex];
                currencyCost.text = "(Level " + x + ") Price (Gem): " + 1;
                textLineOne.text = textLineOne.text + "(L" + x + ")";
                textLineTwo.text = "Available Gems (" + inventory.keyItems[0].itemCount + ")";
            }   
        }
        if (EventSystem.current.currentSelectedGameObject == armorBT.gameObject)
        {
            textLineOne.text = "Permanently Increase Armor?";
            if (currencyIndex == 0)
            {
                int x = party.DEFUpCounters[partyIndex];
                int price = 100 * (x + 1);
                currencyCost.text = "(Level " + x + ") Price (Gold): " + price;
                textLineOne.text = textLineOne.text + "(L" + x + ")";
                textLineTwo.text = "Available Gold (" + inventory.GetAvailableGold() + ")";
            }
            if (currencyIndex == 1)
            {
                int x = party.DEFUpCounters[partyIndex];
                int price = 100 * (x + 1);
                currencyCost.text = "(Level " + x + ") Price (XP): " + price;
                int availXP = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "XP", 0);
                textLineOne.text = textLineOne.text + "(L" + x + ")";
                textLineTwo.text = activeModel.modelName + " XP (" + availXP + ")";
            }
            if (currencyIndex == 2)
            {
                int x = party.DEFUpCounters[partyIndex];
                currencyCost.text = "(Level " + x + ") Price (Gem): " + 1;
                textLineOne.text = textLineOne.text + "(L" + x + ")";
                textLineTwo.text = "Available Gems (" + inventory.keyItems[0].itemCount + ")";
            }            
        }
    }

    IEnumerator CloseTimer()
    {
        toggling = true;   
        weaponBT.interactable = true;
        armorBT.interactable = true;
        if (!armorImage.gameObject.activeSelf)
        {
            armorImage.gameObject.SetActive(true);
        }
        if (!weaponImage.gameObject.activeSelf)
        {
            weaponImage.gameObject.SetActive(true);
        }
        uiController.uiActive = false;
        controller.playerController.enabled = true;
        activeBlackSmith.opened = false;
        uiController.isToggling = true;

        photoBooth.ResetBooth();

        yield return new WaitForSeconds(.15f);
        toggling = false;
        uiController.RemoteToggleTimer(.2f);
        gameObject.SetActive(false);
    }
    IEnumerator Toggle()
    {
        yield return new WaitForSeconds(.25f);
        toggling = false;
    }

    public void OpenConfirm() // opened with button in UI
    {
        string conMess = "Are you sure you would like to perminently increase " + activeModel.modelName + "'s ";
        string finishMess = "";

        float increaseAmount = 3 * activePlier;
        float currentpercent = 0;
        if (currencyIndex == 2)
        {
            increaseAmount = (1.5f * increaseAmount);
        }
        if (upgradeIndex == 0)
        {
            currentpercent = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "PowPercent", 0f);
            Debug.Log("Current Power% Bonus Set to " + currentpercent + ". Current increase% set to " + increaseAmount);
            finishMess = " Power Stat by " + increaseAmount + "%? (Total Boost " + (currentpercent + increaseAmount) + "%)";
        }
        if (upgradeIndex == 1)
        {
            currentpercent = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "DefPercent", 0f);
            Debug.Log("Current DEF% Bonus Set to " + currentpercent + ". Current increase% set to " + increaseAmount);
            finishMess = " Defense Stat by " + increaseAmount + "%?  (Total Boost " + (currentpercent + increaseAmount) + "%)";
        }

        upgradeFinalPer = currentpercent + increaseAmount;

        confirmUI.gameObject.SetActive(true);
        confirmUI.ConfirmMessageUI(conMess + finishMess, true);
    }

    public void ConfirmUpgrade()
    {
        int costInt = 0;
        bool funds = true;
        string fundsMessage = "";
        Debug.Log("Checking Funds");
        if (currencyIndex == 0)
        {
            int indexPos = party.activeParty.IndexOf(activeModel);
            activeBlackSmith.characterIndex = indexPos;
            activeBlackSmith.currencyIndex = 0;

            if (upgradeIndex == 0)
            {

                int count = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "PowerUpCount", 0);
                int cost = 100 * (count + 1);
                costInt = cost;
                if (cost > inventory.GetAvailableGold())
                {
                    funds = false;
                    fundsMessage = "Not Enough Gold To Upgrade " + activeModel.modelName + "'s Attack Power"; 
                }
            }
            if (upgradeIndex == 1)
            {
                int count = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "DEFUpCount", 0);
                int cost = 100 * (count + 1);
                costInt = cost;
                if (cost > inventory.GetAvailableGold())
                {
                    funds = false;
                    fundsMessage = "Not Enough Gold To Upgrade " + activeModel.modelName + "'s Defense";
                }
            }
        }
        if (currencyIndex == 1)
        {
            int indexPos = party.activeParty.IndexOf(activeModel);
            activeBlackSmith.characterIndex = indexPos;
            activeBlackSmith.currencyIndex = 1;

            if (upgradeIndex == 0)
            {                
                int count = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "PowerUpCount", 0); 
                int cost = 100 * (count + 1);
                int XP = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "XP", 0);
                costInt = cost;
                if (cost > XP)
                {
                    Debug.Log("Not Enough Currency");
                    funds = false;
                    fundsMessage = "Not Enough Gold To Upgrade " + activeModel.modelName + "'s Attack Power\n(" + XP + " out of " + cost + ")";
                }
                if (cost <= XP)
                {
            
                }
            }
            if (upgradeIndex == 1)
            {
                int count = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "DEFUpCount", 0);
                int cost = 100 * (count + 1);
                int XP = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "XP", 0); 
                costInt = cost;
                if (cost > XP)
                {
                    Debug.Log("Not Enough Currency");
                    funds = false;
                    fundsMessage = "Not Enough Gold To Upgrade " + activeModel.modelName + "'s Defense\n(" + XP + " out of " + cost + ")";
                }
                if (cost <= XP)
                {
               
                }
            }
        }
        if (currencyIndex == 2)
        {
            int indexPos = party.activeParty.IndexOf(activeModel);
            activeBlackSmith.characterIndex = indexPos;
            activeBlackSmith.currencyIndex = 2;

            costInt = 1;
            if (upgradeIndex == 0)
            {
                if (inventory.keyItems[0].itemCount < 1)
                {
                    funds = false;
                    fundsMessage = "Not Enough Gems To Upgrade " + activeModel.modelName + "'s Attack Power\n(No Gems In Inventory)";
                }
            }
            if (upgradeIndex == 1)
            {
                if (inventory.keyItems[0].itemCount < 1)
                {
                    funds = false;
                    fundsMessage = "Not Enough Gems To Upgrade " + activeModel.modelName + "'s Defense\n(No Gems In Inventory)";
                }
            }
        }

        if (funds)
        {
       
            if (upgradeIndex == 0)
            {
                activeBlackSmith.UpgradePower(activeModel, upgradeFinalPer, costInt, currencyIndex);
            }
            if (upgradeIndex == 1)
            {
                activeBlackSmith.UpgradeArmor(activeModel, upgradeFinalPer, costInt, currencyIndex);
            }

            if (activeBlackSmith.singleUse)
            {
                activeBlackSmith.remove = true;

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


    public void OpenSmithUI(bool weapon = true, bool armor = true, float multiplier = 1, BlacksmithNPC blackSmith = null, int charDex = 0, int currencyDex = 0)
    {
        if (blackSmith == null)
        {
            Debug.Log("Error, no blacksmith attached to UI");
        }
        if (toggling)
        {
            toggling = false;
        }
        activeBlackSmith = blackSmith;
        uiController.uiActive = true;
        controller.playerController.enabled = false;


        partyIndex = charDex;
        currencyIndex = currencyDex;


        currencyImage.sprite = currencyIcons[currencyDex]; // sets to cold by default
        currencyCost.color = Color.yellow;

        topLArrow.gameObject.SetActive(true);
        topRArrow.gameObject.SetActive(true);

        midLArrow.gameObject.SetActive(false);
        midRArrow.gameObject.SetActive(false);

        index = 0;
       
        activePlier = multiplier;

        activeModel = party.activeParty[charDex];
        textLineOne.text = "Select Character";
        textLineTwo.text = activeModel.modelName;

        photoBooth.SayCheese(activeModel);
        if (weapon == false)
        {
            weaponImage.gameObject.SetActive(false);
            weaponBT.interactable = false;
        }
        if (armor == false)
        {
            armorImage.gameObject.SetActive(false);
            armorBT.interactable = false;
        }      
        int x = party.PowerUpCounters[0];
        int price = 100 * (1 + x);
        currencyCost.text = "";

        gameObject.SetActive(true);
        rButtons[0].Select();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void CloseUI()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(CloseTimer());
    }

    public void ArrowRParty()
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

    public void ArrowLParty()
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

    public void ArrowRCurrency()
    {
        uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
        toggling = true;
        currencyIndex++;
        bool armor = false;

        if (EventSystem.current.currentSelectedGameObject == armorBT.gameObject)
        {
            armor = true;
        }

        if (currencyIndex == 3)
        {
            currencyIndex = 0;
        }
        rButtons[1].Select();

        textLineOne.text = "Select Currency";
        int x = 0;
        if (!armor)
        {
            x = party.PowerUpCounters[partyIndex];
        }
        if (armor)
        {
            x = party.DEFUpCounters[partyIndex];
        }

        if (currencyIndex == 0)
        {
            int price = 100 * (x + 1);
            textLineTwo.text = "Available Gold (" + inventory.GetAvailableGold() + ")";
            currencyCost.text = "";

        }
        if (currencyIndex == 1)
        {
            int price = 100 * (x + 1);
            int availXP = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "XP", 0);
            currencyCost.text = "";

            textLineTwo.text = activeModel.modelName + " XP (" + availXP + ")";
        }
        if (currencyIndex == 2)
        {
            currencyCost.text = "";
            textLineTwo.text = "Available Gems (" + inventory.keyItems[0].itemCount + ")";
        }
        currencyImage.sprite = currencyIcons[currencyIndex];
        StartCoroutine(Toggle());
        return;
    }

    public void ArrowLCurrency()
    {
        uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
        toggling = true;
        currencyIndex--;
        bool armor = false;

        if (EventSystem.current.currentSelectedGameObject == armorBT.gameObject)
        {
            armor = true;
        }

        if (currencyIndex == -1)
        {
            currencyIndex = 2;
        }
        lButtons[1].Select();

        textLineOne.text = "Select Currency";
        if (currencyIndex == 0)
        {
            int x = 0;
            if (!armor)
            {
                x = party.PowerUpCounters[partyIndex];
            }
            if (armor)
            {
                x = party.DEFUpCounters[partyIndex];
            }
            int price = 100 * (x + 1);
            currencyCost.text = "";
            textLineTwo.text = "Available Gold (" + inventory.GetAvailableGold() + ")";
        }
        if (currencyIndex == 1)
        {
            int x = 0;
            if (!armor)
            {
                x = party.PowerUpCounters[partyIndex];
            }
            if (armor)
            {
                x = party.DEFUpCounters[partyIndex];
            }
            int price = 100 * (x + 1);
            currencyCost.text = "";
            int availXP = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "XP", 0);
            textLineTwo.text = activeModel.modelName + " XP (" + availXP + ")";
        }
        if (currencyIndex == 2)
        {
            int x = 0;
            if (!armor)
            {
                x = party.PowerUpCounters[partyIndex];
            }
            if (armor)
            {
                x = party.DEFUpCounters[partyIndex];
            }
            currencyCost.text = "";
            textLineTwo.text = "Available Gems (" + inventory.keyItems[0].itemCount + ")";
        }
        currencyImage.sprite = currencyIcons[currencyIndex];
        StartCoroutine(Toggle());
        return;
    }

     void UIUp()
    {
        toggling = true;
        uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
        if (index > 0)
        {
            index--;
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
                
                if (weaponImage.gameObject.activeSelf)
                {
                    weaponBT.Select();
                    upgradeIndex = 0;
                }

                if (!weaponImage.gameObject.activeSelf)
                {
                    armorBT.Select();
                    upgradeIndex = 1;
                }

                ButtonChecker();

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

                if (weaponImage.gameObject.activeSelf)
                {
                    weaponBT.Select();
                    upgradeIndex = 0;
                }

                if (!weaponImage.gameObject.activeSelf)
                {
                    armorBT.Select();
                    upgradeIndex = 1;
                }

                ButtonChecker();
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
            bool armor = false;

            if (EventSystem.current.currentSelectedGameObject == armorBT.gameObject)
            {
                armor = true;
            }

            if (currencyIndex == 3)
            {
                currencyIndex = 0;
            }
            rButtons[1].Select();
            
            textLineOne.text = "Select Currency";
            int x = 0;
            if (!armor)
            {
                x = party.PowerUpCounters[partyIndex];
            }
            if (armor)
            {
                x = party.DEFUpCounters[partyIndex];
            }
  
            if (currencyIndex == 0)
            {
                int price = 100 * (x + 1);
                textLineTwo.text = "Available Gold (" + inventory.GetAvailableGold() + ")";
                currencyCost.text = "";

            }
            if (currencyIndex == 1)
            {
                int price = 100 * (x + 1);
                int availXP = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "XP", 0);
                currencyCost.text = "";

                textLineTwo.text = activeModel.modelName + " XP (" + availXP + ")";
            }
            if (currencyIndex == 2)
            {
                currencyCost.text = "";
                textLineTwo.text = "Available Gems (" + inventory.keyItems[0].itemCount + ")";
            }
            currencyImage.sprite = currencyIcons[currencyIndex];
            StartCoroutine(Toggle());
            return;
        }
        if (index == 2) // power or def
        {
            if (armorBT.interactable)
            {
                if (EventSystem.current.currentSelectedGameObject != armorBT.gameObject)
                {
                    uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
                    toggling = true;
                    armorBT.Select();
                    textLineOne.text = "Permanently Increase Defense?";
                    upgradeIndex = 1;
                    ButtonChecker();
                    StartCoroutine(Toggle());

                    return;
                }

            }

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
            bool armor = false;      

            if (EventSystem.current.currentSelectedGameObject == armorBT.gameObject)
            {
                armor = true;
            }

            if (currencyIndex == -1)
            {
                currencyIndex = 2;
            }
            rButtons[1].Select();

            textLineOne.text = "Select Currency";
            if (currencyIndex == 0)
            {
                int x = 0;
                if (!armor)
                {
                    x = party.PowerUpCounters[partyIndex];
                }
                if (armor)
                {
                    x = party.DEFUpCounters[partyIndex];
                }
                int price = 100 * (x + 1);
                currencyCost.text = "";
                textLineTwo.text = "Available Gold (" + inventory.GetAvailableGold() + ")";
            }
            if (currencyIndex == 1)
            {
                int x = 0;
                if (!armor)
                {
                    x = party.PowerUpCounters[partyIndex];
                }
                if (armor)
                {
                    x = party.DEFUpCounters[partyIndex];
                }
                int price = 100 * (x + 1);
                currencyCost.text = ""; 
                int availXP = EnhancedPrefs.GetPlayerPref(activeModel.modelName + "XP", 0);
                textLineTwo.text = activeModel.modelName + " XP (" + availXP + ")";
            }
            if (currencyIndex == 2)
            {
                int x = 0;
                if (!armor)
                {
                    x = party.PowerUpCounters[partyIndex];
                }
                if (armor)
                {
                    x = party.DEFUpCounters[partyIndex];
                }
                currencyCost.text = "";  
                textLineTwo.text = "Available Gems (" + inventory.keyItems[0].itemCount + ")";
            }
            currencyImage.sprite = currencyIcons[currencyIndex];
            StartCoroutine(Toggle());
            return;
        }
        if (index == 2) // power or def
        {
            if (weaponBT.interactable)
            {
                if (EventSystem.current.currentSelectedGameObject != weaponBT.gameObject)
                {
                    uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
                    uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
                    toggling = true;
                    weaponBT.Select();
                    upgradeIndex = 0;
                    textLineOne.text = "Permanently Increase Power?";
                    ButtonChecker();

                    StartCoroutine(Toggle());

                    return;
                }
            }
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
