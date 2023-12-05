using DTT.PlayerPrefsEnhanced;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BlacksmithUI : MonoBehaviour
{
    public SceneController controller;
    public DunUIController uiController;
    public PartyController party;
    public BlacksmithNPC activeBlackSmith;
    public DunModel activeModel;
    public InventoryController inventory;
    public ConfirmUI confirmUI;

    public Image characterImage;
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
        yield return new WaitForSeconds(.15f);
        toggling = false;
        uiController.RemoteToggleTimer();
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
        if (upgradeIndex == 0)
        {
            activeBlackSmith.UpgradePower(activeModel, upgradeFinalPer);
        }
        if (upgradeIndex == 1)
        {
            activeBlackSmith.UpgradeArmor(activeModel, upgradeFinalPer);
        }

        if (activeBlackSmith.singleUse)
        {            
            DistanceController distance = FindObjectOfType<DistanceController>();
            distance.npcS.Remove(activeBlackSmith);

            uiController.interactUI.gameObject.SetActive(false);
            uiController.interactUI.activeObj = null;

            uiController.rangeImage.gameObject.SetActive(false);
            uiController.customImage.gameObject.SetActive(false);
        }

        CloseUI();
    }


    public void OpenSmithUI(bool weapon = true, bool armor = true, float multiplier = 1, BlacksmithNPC blackSmith = null)
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
       
        currencyImage.sprite = currencyIcons[0]; // sets to cold by default
        currencyCost.color = Color.yellow;

        topLArrow.gameObject.SetActive(true);
        topRArrow.gameObject.SetActive(true);

        midLArrow.gameObject.SetActive(false);
        midRArrow.gameObject.SetActive(false);

        index = 0;
        partyIndex = 0;
        currencyIndex = 0;
        activePlier = multiplier;

        activeModel = party.activeParty[0];
        textLineOne.text = "Select Character";
        textLineTwo.text = "(" + activeModel.modelName + ")";

        characterImage.sprite = activeModel.modelIcon;
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
        int x = 1; // set to upgrade count for character party.active[0];
        currencyCost.text = "GOLD: " + (x * 100);

        gameObject.SetActive(true);
        rButtons[0].Select();
    }

    public void CloseUI()
    {
        StartCoroutine(CloseTimer());
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

                if (upgradeIndex == 0)
                {
                    textLineOne.text = "Permanently Increase Power?";
                    textLineTwo.text = "";
                }
                if (upgradeIndex == 1)
                {
                    textLineOne.text = "Permanently Increase Defense?";
                    textLineTwo.text = "";
                }
            }
            if (index == 1)
            {
                topLArrow.gameObject.SetActive(false);
                topRArrow.gameObject.SetActive(false);
                midLArrow.gameObject.SetActive(true);
                midRArrow.gameObject.SetActive(true);
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
                textLineTwo.text = "(" + activeModel.modelName + ")";
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

                if (upgradeIndex == 0)
                {
                    textLineOne.text = "Permanently Increase Power?";
                    textLineTwo.text = "";
                }
                if (upgradeIndex == 1)
                {
                    textLineOne.text = "Permanently Increase Defense?";
                    textLineTwo.text = "";
                }
            }
            if (index == 1)
            {
                topLArrow.gameObject.SetActive(false);
                topRArrow.gameObject.SetActive(false);
                midLArrow.gameObject.SetActive(true);
                midRArrow.gameObject.SetActive(true);
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
          textLineTwo.text = "(" + activeModel.modelName + ")";
            }
        }
        StartCoroutine(Toggle());
    }

    void UIRight()
    {
        uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
        if (index == 0) // character select
        {
            toggling = true;
            int abc = partyIndex;
            rButtons[0].Select();
            if (abc == 2)
            {
                activeModel = party.activeParty[0];
                partyIndex = 0;
                textLineOne.text = "Select Character";
                textLineTwo.text = "(" + activeModel.modelName + ")";

                characterImage.sprite = activeModel.modelIcon;
                int x = 1; // set to upgrade count for character party.active[0];
                currencyCost.text = "GOLD: " + (x * 100);
                StartCoroutine(Toggle());
                return;
            }
            if (abc == 1)
            {
                activeModel = party.activeParty[2];
                partyIndex = 2;
                textLineOne.text = "Select Character";
                textLineTwo.text = "(" + activeModel.modelName + ")";

                characterImage.sprite = activeModel.modelIcon;
                int x = 1; // set to upgrade count for character party.active[0];
                currencyCost.text = "GOLD: " + (x * 100);
                StartCoroutine(Toggle());
                return;
            }
            if (abc == 0)
            {
                activeModel = party.activeParty[1];
                partyIndex = 1;
                textLineOne.text = "Select Character";
                textLineTwo.text = "(" + activeModel.modelName + ")";

                characterImage.sprite = activeModel.modelIcon;
                int x = 1; // set to upgrade count for character party.active[0];
                currencyCost.text = "GOLD: " + (x * 100);
                StartCoroutine(Toggle());
                return;
            }
        }
        if (index == 1) // currency select
        {
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
                textLineTwo.text = "Chaos Gem (" + inventory.keyItems[0].itemCount + ")";
            }
            currencyImage.sprite = currencyIcons[currencyIndex];
            StartCoroutine(Toggle());
            return;
        }
        if (index == 2) // power or def
        {
            if (armorBT.interactable)
            {
                toggling = true;
                armorBT.Select();
                textLineOne.text = "Permanently Increase Defense?";
                textLineTwo.text = "";
                upgradeIndex = 1;
                StartCoroutine(Toggle());
                return;
            }

        }

    }

    void UILeft()
    {
        uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[0]);
        if (index == 0) // character select
        {
            toggling = true;
            lButtons[0].Select();
            int abc = party.activeParty.IndexOf(activeModel);

            if (abc == 2)
            {
                activeModel = party.activeParty[1];
                partyIndex = 1;
                textLineOne.text = "Select Character";
                textLineTwo.text = "(" + activeModel.modelName + ")";

                characterImage.sprite = activeModel.modelIcon;
                int x = 1; // set to upgrade count for character party.active[0];
                currencyCost.text = "GOLD: " + (x * 100);
                StartCoroutine(Toggle());
                return;
            }
            if (abc == 1)
            {
                activeModel = party.activeParty[0];
                partyIndex = 0;
                textLineOne.text = "Select Character";
                textLineTwo.text = "(" + activeModel.modelName + ")";

                characterImage.sprite = activeModel.modelIcon;
                int x = 1; // set to upgrade count for character party.active[0];
                currencyCost.text = "GOLD: " + (x * 100);
                StartCoroutine(Toggle());
                return;
            }
            if (abc == 0)
            {
                activeModel = party.activeParty[2];
                partyIndex = 2;
                textLineOne.text = "Select Character";
                textLineTwo.text = "(" + activeModel.modelName + ")";

                characterImage.sprite = activeModel.modelIcon;
                int x = 1; // set to upgrade count for character party.active[0];
                currencyCost.text = "GOLD: " + (x * 100);
                StartCoroutine(Toggle());
                return;
            }

        }
        if (index == 1) // currency select
        {
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
                textLineTwo.text = "Chaos Gem (" + inventory.keyItems[0].itemCount + ")";
            }
            currencyImage.sprite = currencyIcons[currencyIndex];
            StartCoroutine(Toggle());
            return;
        }
        if (index == 2) // power or def
        {
            if (weaponBT.interactable)
            {
                toggling = true;
                weaponBT.Select();
                upgradeIndex = 0;
                textLineOne.text = "Permanently Increase Power?";
                textLineTwo.text = "";
                StartCoroutine(Toggle());
                return;
            }
        }

    }

    private void Update()
    {
        float joystickHorizontalInput = Input.GetAxis("Joystick Horizontal");
        float joystickVerticalInput = Input.GetAxis("Joystick Vertical");

        if (!toggling && !confirmUI.gameObject.activeSelf)
        {
            if (Input.GetKey(KeyCode.W) || joystickVerticalInput > 0.1f)
            {
                UIUp();
            }

            if (Input.GetKey(KeyCode.S) || joystickVerticalInput < -0.1f)
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
