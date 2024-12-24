using DTT.PlayerPrefsEnhanced;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SpellNPC : DunNPC
{
    public float multiplier;
    public int charIndex;
    public int currencyIndex;

    public void UpgradeSpellPower(DunModel character = null, float newPercent = 0, int cost = 0, int currencyIndex = 0) // currency checked in confirm UI
    {
        InventoryController inventory = FindObjectOfType<InventoryController>();
        PartyController party = FindObjectOfType<PartyController>();

        EnhancedPrefs.SetPlayerPref(character.modelName + "SpellPercent", newPercent);
        EnhancedPrefs.SavePlayerPrefs();

        Debug.Log(character.modelName + " Spell Power ugraded to " + newPercent);

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
        int currentCount = EnhancedPrefs.GetPlayerPref(character.modelName + "SpellUpCount", 0);
        int newCount = currentCount + 1;
        party.SpellUpCounters[partyIndex] = newCount;
        EnhancedPrefs.SetPlayerPref(character.modelName + "SpellUpCount", newCount);
        EnhancedPrefs.SavePlayerPrefs();

        if (!player.enabled)
        {
            player.enabled = true;
        }
    }

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

            SpellSmithUI spellSmithUI = uiController.spellSmithUI;

            if (spellSmithUI != null)
            {          
                spellSmithUI.OpenSmithUI(multiplier, this, charIndex, currencyIndex);
            }
            if (spellSmithUI == null)
            {
                Debug.Log("ERROR: Did not capture Blacksmith UI from uiController", gameObject);
            }
        }
    }
}
