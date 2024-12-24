using DTT.PlayerPrefsEnhanced;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatSetter : MonoBehaviour
{
    public BattleModel targetModel;

    public void SaveAllStats()
    {
        string name = targetModel.modelName;

        EnhancedPrefs.SetPlayerPref(name + "HP", targetModel.health);
        EnhancedPrefs.SetPlayerPref(name + "MaxHP", targetModel.maxH);
        EnhancedPrefs.SetPlayerPref(name + "PowPercent", (float)targetModel.powerBonusPercent);
        EnhancedPrefs.SetPlayerPref(name + "DefPercent", (float)targetModel.defBonusPercent);
        EnhancedPrefs.SetPlayerPref(name + "SpellPercent", (float)targetModel.spellBonusPercent);

        EnhancedPrefs.SavePlayerPrefs();
    }

}
