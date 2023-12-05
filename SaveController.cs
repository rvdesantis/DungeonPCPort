using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTT.PlayerPrefsEnhanced;

public class SaveController : MonoBehaviour
{
    public PartyController party;
    public UnlockController unlockController;
    public InventoryController inventory;


    public void HomeBank()
    {
        int goldCount = inventory.gold;
        int bankCount = EnhancedPrefs.GetPlayerPref("goldBank", 0);

        int newBank = goldCount + bankCount;

        EnhancedPrefs.SetPlayerPref("goldBank", newBank);
        inventory.gold = 0;

        EnhancedPrefs.SavePlayerPrefs();
    }

    public void SaveBattleStats(BattleModel battleModel)
    {
        string name = battleModel.modelName;
        EnhancedPrefs.SetPlayerPref(name + "HP", battleModel.health);
        EnhancedPrefs.SetPlayerPref(name + "Mana", battleModel.mana);
        EnhancedPrefs.SetPlayerPref(name + "XP", battleModel.XP);

        EnhancedPrefs.SavePlayerPrefs();
    }

    public void SaveAllStats(BattleModel battleModel)
    {
        string name = battleModel.modelName;

        EnhancedPrefs.SetPlayerPref(name + "HP", battleModel.health);
        EnhancedPrefs.SetPlayerPref(name + "MaxHP", battleModel.maxH);
        EnhancedPrefs.SetPlayerPref(name + "Mana", battleModel.mana);
        EnhancedPrefs.SetPlayerPref(name + "MaxMana", battleModel.maxM);
        EnhancedPrefs.SetPlayerPref(name + "XP", battleModel.XP);

        EnhancedPrefs.SetPlayerPref(name + "PowPercent", battleModel.powerBonusPercent);
        EnhancedPrefs.SetPlayerPref(name + "DefPercent", battleModel.defBonusPercent);
        EnhancedPrefs.SetPlayerPref(name + "SpellPercent", battleModel.spellBonusPercent);

        EnhancedPrefs.SavePlayerPrefs();
    }

    public void LoadBattleStats(BattleModel battleModel)
    {
        string name = battleModel.modelName;

        battleModel.health = EnhancedPrefs.GetPlayerPref(name + "HP", 0);
        battleModel.maxH = EnhancedPrefs.GetPlayerPref(name + "MaxHP", 0);
        battleModel.mana = EnhancedPrefs.GetPlayerPref(name + "Mana", 0);
        battleModel.maxM = EnhancedPrefs.GetPlayerPref(name + "MaxMana", 0);

        battleModel.powerBonusPercent = EnhancedPrefs.GetPlayerPref(name + "PowPercent", 0);
        battleModel.defBonusPercent = EnhancedPrefs.GetPlayerPref(name + "DefPercent", 0);
        battleModel.spellBonusPercent = EnhancedPrefs.GetPlayerPref(name + "SpellPercent", 0);
    }
}
