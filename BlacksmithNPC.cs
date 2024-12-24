using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DTT.PlayerPrefsEnhanced;

public class BlacksmithNPC : DunNPC
{
    public bool power;
    public bool def;
    public float multiplier;
    public int characterIndex;
    public int currencyIndex;

    public override void NPCTrigger()
    {
        OpenUI();
    }

    public override void OpenUI()
    {
        DunUIController uiController = FindObjectOfType<DunUIController>();
        if (!uiController.isToggling && !uiController.uiActive)
        {
            opened = true;

            BlacksmithUI blackUI = uiController.blackSmithUI;

            if (blackUI != null)
            {
                blackUI.OpenSmithUI(power, def, multiplier, this, characterIndex, currencyIndex);
            }
            if (blackUI == null)
            {
                Debug.Log("ERROR: Did not capture Blacksmith UI from uiController", gameObject);
            }
        }
    }

    public void UpgradePower(DunModel character = null, float newPercent = 0, int cost = 0, int currencyIndex = 0) // currency checked in confirm UI
    {
        InventoryController inventory = FindObjectOfType<InventoryController>();
        PartyController party = FindObjectOfType<PartyController>();

        EnhancedPrefs.SetPlayerPref(character.modelName + "PowPercent", newPercent);
        EnhancedPrefs.SavePlayerPrefs();

        Debug.Log(character.modelName + " Power ugraded to " + newPercent);

        PlayerController player = FindObjectOfType<PlayerController>();

        player.vfxLIST[0].gameObject.SetActive(true);
        player.vfxLIST[0].Play();
        player.audioSource.PlayOneShot(player.audioClips[1]);

        if (currencyIndex == 0)
        {
            inventory.ReduceGold(cost);
        }
        if (currencyIndex == 1)
        {
            int currentXP = EnhancedPrefs.GetPlayerPref(character.modelName + "XP", cost);
            int leftoverXP = currentXP - cost;
            character.SaveXP(leftoverXP);
        }
        if (currencyIndex == 2)
        {
            inventory.keyItems[0].itemCount--;
        }


        int partyIndex = 0;
        foreach (DunModel mod in party.activeParty)
        {
            if (character.modelName == mod.modelName)
            {
                partyIndex = party.activeParty.IndexOf(mod);
                break;
            }
        }
        int currentCount = EnhancedPrefs.GetPlayerPref(character.modelName + "PowerUpCount", 0);
        int newCount = currentCount + 1;
        party.PowerUpCounters[partyIndex] = newCount;
        EnhancedPrefs.SetPlayerPref(character.modelName + "PowerUpCount", newCount);
        EnhancedPrefs.SavePlayerPrefs();

        if (!player.enabled)
        {
            player.enabled = true;
        }
    }

    public void UpgradeArmor(DunModel character = null, float newPercent = 0, int cost = 0, int currencyIndex = 0)
    {
        InventoryController inventory = FindObjectOfType<InventoryController>();
        PartyController party = FindObjectOfType<PartyController>();

        EnhancedPrefs.SetPlayerPref(character.modelName + "DefPercent", newPercent);
        EnhancedPrefs.SavePlayerPrefs();

        Debug.Log(character.modelName + " DEF ugraded to " + newPercent);

        PlayerController player = FindObjectOfType<PlayerController>();

        player.vfxLIST[0].gameObject.SetActive(true);
        player.vfxLIST[0].Play();
        player.audioSource.PlayOneShot(player.audioClips[1]);

        if (currencyIndex == 0)
        {
            inventory.ReduceGold(cost);
        }
        if (currencyIndex == 1)
        {
            int currentXP = EnhancedPrefs.GetPlayerPref(character.modelName + "XP", cost);
            int leftoverXP = currentXP - cost;
            character.SaveXP(leftoverXP);
        }
        if (currencyIndex == 2)
        {
            inventory.keyItems[0].itemCount--;
        }

        int partyIndex = 0;
        foreach (DunModel mod in party.activeParty)
        {
            if (character.modelName == mod.modelName)
            {
                partyIndex = party.activeParty.IndexOf(mod);
                break;
            }
        }
        int currentCount = EnhancedPrefs.GetPlayerPref(character.modelName + "DEFUpCount", 0);
        int newCount = currentCount + 1;
        party.DEFUpCounters[partyIndex] = newCount;
        EnhancedPrefs.SetPlayerPref(character.modelName + "DEFUpCount", newCount);
        EnhancedPrefs.SavePlayerPrefs();

        if (!player.enabled)
        {
            player.enabled = true;
        }
    }

    private void Update()
    {

    }
}
