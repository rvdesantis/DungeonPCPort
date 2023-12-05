using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTT.PlayerPrefsEnhanced;

public class UnlockController : MonoBehaviour
{
    public bool rogueUnlock;
    public bool mapVendorUnlock;

    public SceneController controller;
    public SanctuaryCube sanctuary;
    public SaveController saveController;
    public PartyController party;
    public UnlockedUI unlockUI;
    public List<DunModel> unlockableCharacters;
    public List<DunNPC> unlockableNPC;
    public List<DunItem> unlockableItems;


    public bool resetDataOnLoad;

    private void Start()
    {
        if (resetDataOnLoad)
        {
            ResetAllData(true);
        }
    }

    public void ToggleDataReset()
    {
        resetDataOnLoad = true;
    }

    public void ResetAllData(bool resetGold = false)
    {
        resetDataOnLoad = false;
        EnhancedPrefs.SetPlayerPref("rogueUnlock", false);
        party.masterParty[4].gameObject.SetActive(false);

        EnhancedPrefs.SetPlayerPref("mapvendorUnlock", false);
        sanctuary.sanctVendors[2].gameObject.SetActive(false);
        if (resetGold)
        {
            EnhancedPrefs.SetPlayerPref("goldBank", 0);
        }
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
        if (!rogueCheck)
        {

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
            DunModel unlockModel = unlockableCharacters[0];
            unlockUI.OpenUnlockUI(unlockModel.modelInfo, 0, unlockModel.modelIcon ,true);
            EnhancedPrefs.SavePlayerPrefs();
        }
    }

    public void UnlockNPC(int npcIndex)
    {
        if (npcIndex == 0)
        {
            EnhancedPrefs.SetPlayerPref("mapvendorUnlock", true);
            unlockUI.OpenUnlockUI(unlockableNPC[0].modelInfo, 0, unlockableNPC[0].icon, false, true);
            EnhancedPrefs.SavePlayerPrefs();
        }
    }
}
