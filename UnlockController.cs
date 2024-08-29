using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTT.PlayerPrefsEnhanced;

public class UnlockController : MonoBehaviour
{
    public bool rogueUnlock;
    public bool vMageUnlock;
    public bool mapVendorUnlock;

    public SceneController controller;
    public SanctuaryCube sanctuary;
    public SaveController saveController;
    public PartyController party;
    public UnlockedUI unlockUI;
    public List<DunModel> unlockableCharacters;
    public List<DunModel> unlockableEnemies;
    public List<DunNPC> unlockableNPC;
    public List<DunItem> unlockableItems;

    public bool resetDataOnLoad;

    private void Start()
    {
        if (resetDataOnLoad)
        {
            ResetAllData();
        }
    }

    public void ToggleDataReset()
    {
        resetDataOnLoad = true;
    }

    public void ResetAllData()
    {
        resetDataOnLoad = false;
        EnhancedPrefs.SetPlayerPref("rogueUnlock", false);
        EnhancedPrefs.SetPlayerPref("voidMageUnlock", false);
        party.masterParty[4].gameObject.SetActive(false);
        party.masterParty[5].gameObject.SetActive(false);

        EnhancedPrefs.SetPlayerPref("mapvendorUnlock", false);

        foreach (DunModel model in party.masterParty)
        {
            EnhancedPrefs.SetPlayerPref(model.modelName + "PowerUpCount", 0);
            EnhancedPrefs.SetPlayerPref(model.modelName + "DEFUpCount", 0);
            EnhancedPrefs.SetPlayerPref(model.modelName + "SpellUpCount", 0);

            EnhancedPrefs.SetPlayerPref(model.modelName + "DefPercent", 0f);
            EnhancedPrefs.SetPlayerPref(model.modelName + "PowPercent", 0f);
            EnhancedPrefs.SetPlayerPref(model.modelName + "SpellPercent", 0f);

        }

        sanctuary.sanctVendors[2].gameObject.SetActive(false);

        EnhancedPrefs.SavePlayerPrefs();       
    }

    public void OpenGameLoad() // must be called before Set Next Run in SaveController
    {
        // unlocked characters check
        bool rogueCheck = EnhancedPrefs.GetPlayerPref("rogueUnlock", false);
        if (rogueCheck)
        {
            rogueUnlock = true;
            party.masterParty[4].gameObject.SetActive(true);
        }

        bool vMageCheck = EnhancedPrefs.GetPlayerPref("voidMageUnlock", false);
        if (vMageCheck)
        {
            vMageUnlock = true;
            party.masterParty[5].gameObject.SetActive(true);
        }

        // unlocked NPCs
        bool mapGobCheck = EnhancedPrefs.GetPlayerPref("mapvendorUnlock", false);
        if (mapGobCheck)
        {
            mapVendorUnlock = true;
            DunNPC mapGoblin = sanctuary.sanctVendors[2];
            mapGoblin.gameObject.SetActive(true);
            FindObjectOfType<DistanceController>().npcS.Add(mapGoblin);
        }

        // character current leveling check
        // character current XP check
        // character found artifacts check

        // unlocked rooms check
        // unlocked monsters check;
        // unlocked bosses check

        // unlocked dun items check

        int loadedGold = EnhancedPrefs.GetPlayerPref("goldBank", 0);
        Debug.Log(loadedGold + " gold loaded at game start");
    }

    public void UnlockCharacter(int masterIndex)
    {
        if (masterIndex == 4)
        {
            EnhancedPrefs.SetPlayerPref("rogueUnlock", true);
            Debug.Log("Rogue Unlocked");
            DunModel unlockModel = party.masterParty[4];
            unlockUI.photoBooth.SayCheese(unlockModel);
            unlockUI.OpenUnlockUI(unlockModel.modelInfo, true, 4, true);
            EnhancedPrefs.SavePlayerPrefs();
        }
        if (masterIndex == 5)
        {
            EnhancedPrefs.SetPlayerPref("voidMageUnlock", true);         
            Debug.Log("Void Mage Unlocked");
            DunModel unlockModel = party.masterParty[5];
            unlockUI.photoBooth.SayCheese(unlockModel);
            // set next unlock action on UnlockController - Unlocking Templar Enemies
            unlockUI.OpenUnlockUI(unlockModel.modelInfo, false, 5, true);
            EnhancedPrefs.SavePlayerPrefs();
        }
    }

    public void UnlockEnemy(int enemyUnlockIndex)
    {
        if (enemyUnlockIndex == 0)
        {
            EnhancedPrefs.SetPlayerPref("templarEnemiesUnlock", true);
            Debug.Log("Templar Enemies Unlocked"); 
            DunModel unlockModel = unlockableEnemies[0];          
            unlockUI.OpenUnlockUI(unlockModel.modelInfo, true, 0, false, false, false, true);
            EnhancedPrefs.SavePlayerPrefs();
        }
    }


    public void UnlockNPC(int npcIndex)
    {
        if (npcIndex == 0)
        {
            EnhancedPrefs.SetPlayerPref("mapvendorUnlock", true);
            unlockUI.OpenUnlockUI(unlockableNPC[0].modelInfo, true, 0, false, true);
            EnhancedPrefs.SavePlayerPrefs();
        }
    }
}
